using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using DevExpress.Xpo;
using FileHelpers;

using Atlas.Common.Utils;
using Atlas.Domain.Model;
using Atlas.Domain.Model.Evolution;
using Atlas.Evolution.Server.Code.Data;
using Atlas.Evolution.Server.Code.Layout;
using Atlas.Evolution.Server.Code.Utils;
using Atlas.Enumerators;
using EvolutionEnums = Atlas.Enumerators.Evolution;
using Atlas.Common.Interface;


namespace Atlas.Evolution.Server.Code.Batch
{
  internal class FileCreateMonthly
  {
    /// <summary>
    /// Fleshes out from a 'snapshot' for a month, to a full Evolution fixed length file.
    /// No calculations, just merge data from daily snapshot and company.client
    /// </summary>
    /// <param name="log"></param>
    /// <param name="srn"></param>
    /// <param name="collateDate"></param>
    /// <param name="monthEndDate"></param>
    /// <param name="path"></param>
    /// <param name="batchId"></param>
    /// <returns>null if no data, else *temporary* file with Evolution data</returns>
    public static string CreateFile(ILogging log, IConfigSettings config,
      string srn, DateTime collateDate, DateTime monthEndDate, string path, long batchId)
    {
      log.Information("CreateFile started: {CollateDate:yyyy-MM-dd}, {MonthEndDate:yyyy-MM-dd}", collateDate, monthEndDate);
      var tempFile = Path.Combine(path, string.Format("{0}.txt", Guid.NewGuid().ToString("N")));
      using (var uow = new UnitOfWork())
      {
        var snapShots = uow.Query<EVO_UploadBatch>().First(s => s.UploadBatchId == batchId).LoanTrackSnapshots.ToList();
        if (snapShots.Any())
        {
          log.Information("CreateFile: Snapshots found: {Snapshots}", snapShots.Count);

          using (var assRepos = new AssRepository(config.GetAssConnectionString()))
          {
            var burBranchList = uow.Query<BUR_Service>()
              .Where(s => s.ServiceType == Risk.ServiceType.Credit)
              .ToDictionary(k => k.Branch.BranchId, v => v.BranchCode);

            var totalRows = 0;

            // Get client details to fill in the blanks
            log.Information("CreateFile getting client details..");
            var clientRecIds = snapShots.Select(s => s.LoanTrack.ClientRecId).Distinct().ToList();
            var clients = assRepos.GetClients($"SELECT * FROM company.client WHERE recid IN ({string.Join(",", clientRecIds)})");
            log.Information("CreateFile got {Clients} 'client' details", clientRecIds.Count);

            // Get loan details to fill in other criteria
            //var loanRecIds = snapShots.Select(s => s.LoanTrack.LoanRecId).Distinct().ToList();
            //var loans  = assRepos.GetLoans($"SELECT * FROM company.loans WHERE recid IN ({string.Join(",", loanRecIds)})");
            //log.Information("CreateFile got {Loans} 'loans' details", loanRecIds.Count);

            // Get occupations descriptions for look-up
            var allOccupations = assRepos.GetOccupations()
              .Select(s => new Data.ASS_Models.occupations { lrep_brnum = s.lrep_brnum.PadLeft(3, '0'), occup = s.occup.Trim(), name = s.name.Trim() })
              .ToList();

            using (var sw = new StreamWriter(tempFile, true, System.Text.Encoding.ASCII))
            {
              #region Header
              var header = new FileHelperEngine<Monthly_Header>();
              var headerRow = new Monthly_Header
              {
                FILE_CREATION_DATE = collateDate.ToYyyyMmDd(),
                MONTH_END_DATE = monthEndDate.ToYyyyMmDd(),
                SUPPLIER_REFERENCE_NUMBER = srn,
              };
              header.WriteStream(sw, new List<Monthly_Header> { headerRow });
              #endregion

              #region Rows
              var rows = new List<Monthly_Data>();
              foreach (var snapShot in snapShots)
              {
                if (burBranchList.ContainsKey(snapShot.LoanTrack.Branch.BranchId))
                {
                  if (snapShot.SnapshotDate < monthEndDate.Subtract(TimeSpan.FromDays(31)))
                  {
                    throw new Exception("Bad date");
                  }
                  var client = clients.First(s => s.recid == snapShot.LoanTrack.ClientRecId);
                  //var loan = loans.First(s => s.recid == snapShot.LoanTrack.LoanRecId);

                  #region Determine DOB, gender with preference from ID number decoding
                  DateTime birthDate;
                  IdValidator2.GenderTypes gender;
                  IdValidator2.CitizenTypes citizenType;
                  IdValidator2.ErrorTypes error;
                  var id = client.identno.Trim();
                  var bValidId = IdValidator2.IsValidSouthAfricanId(id, out birthDate, out gender, out citizenType, out error);
                  var genderChar = (bValidId) ? gender == IdValidator2.GenderTypes.Male ? "M" : "F" : client.gender;
                  if (!bValidId)
                  {
                    birthDate = client.birthdate ?? DateTime.Now.Subtract(TimeSpan.FromDays(18 * 366));
                    gender = client.gender == "F" ? IdValidator2.GenderTypes.Female : IdValidator2.GenderTypes.Male;
                  }
                  #endregion
                  
                  var isClosure = snapShot.Reason == EvolutionEnums.SnapshotReasonTypes.Closure;
                  var isRegistration = snapShot.Reason == EvolutionEnums.SnapshotReasonTypes.Registration;
                  var isStatus = snapShot.Reason == EvolutionEnums.SnapshotReasonTypes.Active;

                  var evoClosedStatusCode = isClosure ? EvoMapping.ClosedLoanStatusAssToEvoStatusCode(snapShot.LoanStatus) : "";
                  var okClosure = EvoStringUtils.IsPositiveOutcome(evoClosedStatusCode);

                  var burBranch = burBranchList[snapShot.LoanTrack.Branch.BranchId];
                  var workNameClean = EvoStringUtils.CleanCompanyName(client.workname, 60);
                  var surnameClean = EvoStringUtils.CleanPersonName(client.surname, 25, 2, false, true);
                  var foreName1 = EvoStringUtils.CleanPersonName(client.firstname, 14, 2, false, true);
                  var foreName2 = EvoStringUtils.CleanPersonName(client.othname, 14, 2, true, true);                  
                  if (string.IsNullOrEmpty(foreName1) && !string.IsNullOrEmpty(foreName2))
                  {
                    foreName1 = foreName2;
                    foreName2 = null;
                  }
                  if (!string.IsNullOrEmpty(foreName2) && string.Compare(foreName1, foreName2) == 0 || string.Compare(surnameClean, foreName2) == 0)
                  {
                    foreName2 = null;
                  }

                  // Terms must be 1 for account type M
                  var terms = (EvoCalcs.LoanIsMoreThan1Month(snapShot.PayFrequency, snapShot.LoanPeriod)) ?
                               EvoCalcs.GetLoanPeriodInMonths(snapShot.PayFrequency, snapShot.LoanPeriod) : 1; // Manual: "Must be supplied as 0001 in the case of a 1 month loan"

                  var occupation = allOccupations.FirstOrDefault(s =>
                    s.lrep_brnum == snapShot.LoanTrack.Branch.LegacyBranchNum.PadLeft(3, '0') &&
                    s.occup == client.occup.Trim());

                  // zero for C,P,S,T,V,F,M,H,G,K,S    
                  var currentBalance = isRegistration || isStatus ? snapShot.CurrentBalance :
                        (isClosure && !okClosure) ? Math.Max(snapShot.OverdueAmount, snapShot.CurrentBalance) : 0;

                  //// Balance cannot be less R100, unless a normal closure
                  if (currentBalance < 100)
                  {
                    if (!isClosure || (isClosure && !okClosure))
                    {
                      log.Error("Balance <R100 for non-closed/bad loan- skipping: {@LoanTrack}",
                        new { snapShot.LoanTrack.Branch.LegacyBranchNum, snapShot.LoanTrack.AssClient, snapShot.LoanTrack.AssLoan, currentBalance, snapShot.Reason });
                      continue;
                    }
                    currentBalance = 0;
                  }
                                
                  // Overdue not on normal closed
                  var overdueAmount = (isClosure && !okClosure) || isStatus ? snapShot.OverdueAmount : 0;
                  if (overdueAmount != 0 && overdueAmount < 100)
                  {
                    log.Warning("Overdue R0.01 - 99.99: {@Details}",
                     new { snapShot.LoanTrack.Branch.LegacyBranchNum, snapShot.LoanTrack.AssClient, snapShot.LoanTrack.AssLoan, snapShot.OverdueAmount, snapShot.OverdueSince, snapShot.Reason });

                    overdueAmount = 0;
                  }

                  var daysOverdue = snapShot.OverdueSince != null ? monthEndDate.Subtract(snapShot.OverdueSince.Value).TotalDays : 0;
                  // zero for codes 'C', 'V', 'T' + registrations
                  var monthsInArrears = ((isClosure && !okClosure) || isStatus) ? ((uint)(Math.Floor(daysOverdue / 30))) : 0;
                  if (monthsInArrears > 9)
                  {
                    monthsInArrears = 9;
                  }
                  // if months in arrears == 0, overdue?
                  if (monthsInArrears == 0 && overdueAmount > 0)
                  {
                    // This is common, could be 1 day overdue... if less than 30 days, we must report as not overdue

                    //log.Warning("Months in arrears == 0, but overdue > R0.00: {@Details}",
                    //  new { snapShot.LoanTrack.Branch.LegacyBranchNum, snapShot.LoanTrack.AssClient, snapShot.LoanTrack.AssLoan, snapShot.OverdueAmount, snapShot.OverdueSince, snapShot.Reason });
                    overdueAmount = 0;
                  }

                  if (monthsInArrears > 0 && overdueAmount == 0)
                  {
                    log.Error("Overdue == R0.00, but months in arrears > 0: {@Details}",
                      new { snapShot.LoanTrack.Branch.LegacyBranchNum, snapShot.LoanTrack.AssClient, snapShot.LoanTrack.AssLoan, snapShot.OverdueAmount, snapShot.OverdueSince, snapShot.Reason });
                    monthsInArrears = 0;
                  }
                                   
                  var dateOfLastPayment = ((isClosure && okClosure) || isStatus) ? snapShot.LastReceipt : snapShot.OverdueSince;
                  // For bad closures, use oldest date we can...
                  if (isClosure && !okClosure && snapShot.OverdueSince != null && dateOfLastPayment > snapShot.OverdueSince)
                  {
                    dateOfLastPayment = snapShot.OverdueSince;
                  }

                  if (!isClosure && dateOfLastPayment != null && monthEndDate.Subtract(dateOfLastPayment.Value).TotalDays > 92)
                  {
                    log.Error("dateOfLastPayment > 92 days: {@Details}",
                      new { snapShot.LoanTrack.Branch.LegacyBranchNum, snapShot.LoanTrack.AssClient, snapShot.LoanTrack.AssLoan, snapShot.OverdueAmount, snapShot.OverdueSince, snapShot.Reason, dateOfLastPayment });
                  }

                  try
                  {
                    rows.Add(new Monthly_Data()
                    {
                      /* 1*/
                      //DATA = isClosure ? "C" : "R",
                      /* 2*/
                      SA_ID_NUMBER = (bValidId) ? id : null,
                      /* 3*/
                      NON_SA_ID_NUMBER = (!bValidId) ? EvoStringUtils.CleanPassport(id, 16) : "",
                      /* 4*/
                      DATE_OF_BIRTH = birthDate.ToYyyyMmDd(),
                      /* 5*/
                      GENDER = (gender == IdValidator2.GenderTypes.Male) ? "M" : "F",
                      /* 6*/
                      BRANCH_CODE = burBranch.Trim(),                                 // Do not zero fill or pad
                                                                                      /* 7*/
                      ACCOUNT_NO = snapShot.LoanTrack.AssClient.Trim(),               // Do not zero fill or pad
                                                                                      /* 8*/
                      SUB_ACCOUNT_NO = snapShot.LoanTrack.AssLoan.Trim(),             // Do not zero fill or pad

                      /* 9*/
                      SURNAME = surnameClean,
                      /*10*/
                      TITLE = EvoStringUtils.CleanTitle(client.title, genderChar == "M" ? General.Gender.Male : General.Gender.Female),
                      /*11*/
                      FORENAME_OR_INITIAL_1 = foreName1,
                      /*12*/
                      FORENAME_OR_INITIAL_2 = foreName2,

                      // TODO: SACCRA formatting is strict, but if data entry is poor, should we skip? Hard to fix entered data...
                      /*14*/
                      RESIDENTIAL_ADDRESS_LINE_1 = EvoStringUtils.CleanAddress(client.haddr1, 25, 2),
                      /*15*/
                      RESIDENTIAL_ADDRESS_LINE_2 = EvoStringUtils.CleanAddress(client.haddr2, 25, 5),
                      /*16*/
                      RESIDENTIAL_ADDRESS_LINE_3 = EvoStringUtils.CleanAddress(client.haddr3, 25, 5),
                      /*18*/
                      POSTAL_CODE_OF_RESIDENTIAL_ADDRESS = EvoStringUtils.CleanPostalCode(client.hpostcode, 6) ?? "0001",
                      /*19*/
                      OWNER_TENANT = client.home_owner == "Y" ? "O" /* owner */ : "T" /* Tenant */,
                      /*20-24 - Postal */
                      /*25*/
                      OWNERSHIP_TYPE = "00", // Other- this is the default ownership type
                                             /*26*/
                      LOAN_REASON_CODE = EvoMapping.LoanReasonAssToEvolution(snapShot.LoanTrack.AssLoanReason),
                      /*27*/
                      PAYMENT_TYPE = "00",  // Use if not listed below "Payment Types and priority table"                                          
                                            /*28*/
                      TYPE_OF_ACCOUNT = EvoCalcs.LoanIsMoreThan1Month(snapShot.PayFrequency, snapShot.LoanPeriod) ? "P" : "M", // Personal loan // "M" - One Month Personal Loan                                     
                                                                                                                               /*29*/
                      DATE_ACCOUNT_OPENED = snapShot.LoanTrack.AssLoanStartDate.ToYyyyMmDd(),
                      /*30 Deferred payment date*/
                      /*31*/
                      DATE_OF_LAST_PAYMENT = dateOfLastPayment.ToYyyyMmDd(),
                      /*32*/
                      OPENING_BALANCE_CREDIT_LIMIT = (uint)snapShot.LoanTrack.LoanAmount, // always provide                                                                                        
                                                                                          /*33*/
                      CURRENT_BALANCE = (uint)Math.Abs(currentBalance),
                      /*34*/
                      CURRENT_BALANCE_INDICATOR = isRegistration || isClosure ? "D" :
                        currentBalance >= 0 ? "D" : "C", //for registrations and closures, must be set to 'D' , d- debit, c-credit                                                    
                      /*35*/
                      AMOUNT_OVERDUE = (uint)overdueAmount, // zero for codes 'C', 'V', 'T', ZERO FOR REGS                                                                                                   
                                                            /*36*/
                      INSTALMENT_AMOUNT = isRegistration || (isClosure && !okClosure) || isStatus ? (uint)snapShot.InstalmentAmount : 0, // zero for codes 'C', 'V', 'T'                                                                                                                           
                                                                                                                                         /*37*/
                      MONTHS_IN_ARREARS = monthsInArrears, // zero for codes 'C', 'V', 'T' + registrations
                                                           /*38*/
                      STATUS_CODE = evoClosedStatusCode,
                      /*39*/
                      REPAYMENT_FREQUENCY = EvoMapping.FrequencyAssToEvoRepayFreq(snapShot.PayFrequency),
                      /*40*/
                      TERMS = terms,
                      /*41*/
                      STATUS_DATE = !string.IsNullOrEmpty(evoClosedStatusCode) ? monthEndDate.ToYyyyMmDd() : 0, // zero fill if no status code
                                                                                                                /*42-45 Old supplier*/
                                                                                                                /*46*/
                      HOME_TELEPHONE = EvoStringUtils.CleanTelephone(client.hometel),
                      /*47*/
                      CELLULAR_TELEPHONE = EvoStringUtils.CleanTelephone(client.cell),
                      /*48*/
                      WORK_TELEPHONE = EvoStringUtils.CleanTelephone(client.worktel),
                      /*49*/
                      EMPLOYER_DETAIL = workNameClean != foreName1 && workNameClean != foreName2 && workNameClean != surnameClean ? workNameClean : null,
                      /*50*/
                      INCOME = (uint)snapShot.GrossSalary,
                      /*51*/
                      INCOME_FREQUENCY = EvoMapping.FrequencyAssToEvolution(snapShot.PayFrequency),

                      OCCUPATION = EvoStringUtils.CleanOccupation(occupation?.name, 20)
                    });

                    if (rows.Count % 100 == 0)
                    {
                      log.Information("CreateFile has written {rows} rows", rows.Count);
                    }
                  }
                  catch (Exception err)
                  {
                    log.Error(err, "[CreateMonthlyFile.CreateFile]- rows.Add()");
                    throw;
                  }
                }
                else
                {
                  log.Error("Failed to locate BUR_Service entry for: {LegacyBranchNum}", snapShot.LoanTrack.Branch.LegacyBranchNum);
                }
              }

              var data = new FileHelperEngine<Monthly_Data>();
              data.WriteStream(sw, rows);
              totalRows += rows.Count;
              #endregion

              #region Footer           
              var trailer = new FileHelperEngine<Monthly_Trailer>();
              var trailerRow = new Monthly_Trailer { NUMBER_OF_RECORDS = (uint)totalRows + 2 }; // count must include header/trailer
              trailer.WriteStream(sw, new List<Monthly_Trailer> { trailerRow });             
              #endregion
            }
          }
        }
        else
        {
          tempFile = null;
        }
      }

      return tempFile;
    }

  }
}
