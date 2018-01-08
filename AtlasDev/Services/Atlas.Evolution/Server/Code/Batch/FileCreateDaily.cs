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
  internal static class FileCreateDaily
  {
    /// <summary>
    /// Fleshes out from a 'snapshot' for a single branch, to a full Evolution fixed length file.
    /// No calculations, just merges data from the snapshot and company.client
    /// </summary>
    /// <param name="srn"></param>
    /// <param name="isFinal">Is final file or test file</param>
    /// <param name="batchDate">Date for which the batch is being taken</param>
    /// <param name="legacyBranchNum"></param>
    /// <returns>null if no data, else *temporary* file with Evolution data</returns>
    public static string CreateFile(ILogging log, IConfigSettings config, string srn, DateTime batchDate, string legacyBranchNum, string path, long batchId)
    {
      var tempFile = Path.Combine(path, string.Format("{0}.txt", Guid.NewGuid().ToString("N")));
      using (var assRepos = new AssRepository(config.GetAssConnectionString()))
      using (var uow = new UnitOfWork())
      {
        var batch = uow.Query<EVO_UploadBatch>().First(s => s.UploadBatchId == batchId);
        var branch = uow.Query<BRN_Branch>().First(s => s.LegacyBranchNum.PadLeft(3, '0') == legacyBranchNum);
        var burBranch = uow.Query<BUR_Service>().FirstOrDefault(s => s.Branch.BranchId == branch.BranchId && s.ServiceType == Risk.ServiceType.Credit);
        if (burBranch == null)
        {
          log.Warning("No bureau entry for branch- skipping generation: {Branch}-{BranchName}", branch.LegacyBranchNum, branch.Company.Name);
          return null;
        }

        var snapShots = batch.LoanTrackSnapshots.Where(s => s.LoanTrack.Branch.BranchId == branch.BranchId).ToList();
        //var snapShots = batch.LoanTrackSnapshots.Where(s => s.LoanTrack.Branch == branch).ToList();
        // (s => s.LoanTrack.Branch == branch) does not work?!?!?!

        if (snapShots.Any())
        {
          var clientRecIds = snapShots.Select(s => s.LoanTrack.ClientRecId).ToList();
          var clients = assRepos.GetClients($"SELECT * FROM company.client WHERE recid IN ({string.Join(",", clientRecIds)})");
          var rows = new List<Daily_Data>();

          // Get occupations descriptions for look-up
          var occupationsLookUp = assRepos.GetOccupations()
            .Select(s => new Data.ASS_Models.occupations { lrep_brnum = s.lrep_brnum.PadLeft(3, '0'), occup = s.occup.Trim(), name = s.name.Trim() })
            .ToList();

          foreach (var snapShot in snapShots)
          {
            var client = clients.First(s => s.recid == snapShot.LoanTrack.ClientRecId);

            #region Determine DOB & gender, with preference to use values decoded from the ID number
            DateTime birthDate;
            IdValidator2.GenderTypes gender;
            IdValidator2.CitizenTypes citizenType;
            IdValidator2.ErrorTypes error;
            var id = client.identno.Trim();
            var bValidId = IdValidator2.IsValidSouthAfricanId(id, out birthDate, out gender, out citizenType, out error);
            var genderChar = (bValidId) ? gender == IdValidator2.GenderTypes.Male ? "M" : "F" : client.gender;
            if (!bValidId)
            {
              birthDate = (client.birthdate ?? DateTime.Now.Subtract(TimeSpan.FromDays(18 * 366)));
              gender = client.gender == "F" ? IdValidator2.GenderTypes.Female : IdValidator2.GenderTypes.Male;
            }
            #endregion

            var isClosure = snapShot.Reason == EvolutionEnums.SnapshotReasonTypes.Closure;
            var isRegistration = snapShot.Reason == EvolutionEnums.SnapshotReasonTypes.Registration;
            var closedStatusCode = isClosure ? EvoMapping.ClosedLoanStatusAssToEvoStatusCode(snapShot.LoanStatus) : "";
            var okClosure = EvoStringUtils.IsPositiveOutcome(closedStatusCode);

            var occupation = occupationsLookUp.FirstOrDefault(s =>
                    s.lrep_brnum == snapShot.LoanTrack.Branch.LegacyBranchNum.PadLeft(3, '0') &&
                    s.occup == client.occup.Trim());

            if (closedStatusCode != "W" && closedStatusCode != "L" && closedStatusCode != "P" &&
              closedStatusCode != "H" && closedStatusCode != "D") // no daily submission for these codes...
            {
              var workNameClean = EvoStringUtils.CleanCompanyName(client.workname, 60);
              var surnameClean = EvoStringUtils.CleanPersonName(client.surname, 25, 2, false, true);
              var foreName1 = EvoStringUtils.CleanPersonName(client.firstname, 14, 2, false, true);
              var foreName2 = EvoStringUtils.CleanPersonName(client.othname, 14, 2, true, true);

              // Terms must be 1 for account type M
              uint terms = 1;
              if (EvoCalcs.LoanIsMoreThan1Month(snapShot.PayFrequency, snapShot.LoanPeriod))
              {
                terms = EvoCalcs.GetLoanPeriodInMonths(snapShot.PayFrequency, snapShot.LoanPeriod);
              }

              // zero for codes 'C', 'V', 'T', ZERO FOR REGS
              var overdueAmt = (isClosure && !okClosure) ? snapShot.OverdueAmount : 0;
              if (overdueAmt < 100)
              {
                if (isClosure && !okClosure) // bad status
                {
                  log.Error("Overdue <R100 for hand-over {@LoanTrack}",
                        new { snapShot.LoanTrack.Branch.LegacyBranchNum, snapShot.LoanTrack.AssClient, snapShot.LoanTrack.AssLoan,
                              overdueAmt, snapShot.Reason });
                  continue;
                }
                overdueAmt = 0;
              }

              rows.Add(new Daily_Data()
              {
                /* 1*/
                DATA = isClosure ? "C" : "R",
                /* 2*/
                SA_ID_NUMBER = (bValidId) ? id : null,
                /* 3*/
                NON_SA_ID_NUMBER = (!bValidId) ? EvoStringUtils.CleanPassport(id, 16) : "",
                /* 4*/
                DATE_OF_BIRTH = birthDate.ToYyyyMmDd(),
                /* 5*/
                GENDER = (gender == IdValidator2.GenderTypes.Male) ? "M" : "F",
                /* 6*/
                BRANCH_CODE = burBranch.BranchCode.Trim(), // Do not zero fill or pad
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

                // TODO: Formatting is strict, but if data entry is poor, should we skip? Hard to fix entered data...
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
                TYPE_OF_ACCOUNT = EvoCalcs.LoanIsMoreThan1Month(snapShot.PayFrequency, snapShot.LoanPeriod) ? "P" : "M", // "P"- Personal loan // "M"- One Month Personal Loan       
                /*29*/
                DATE_ACCOUNT_OPENED = snapShot.LoanTrack.AssLoanStartDate.ToYyyyMmDd(),
                /*30 Deferred payment date*/
                /*31*/
                DATE_OF_LAST_PAYMENT = snapShot.LastReceipt.ToYyyyMmDd(),
                /*32*/
                OPENING_BALANCE_CREDIT_LIMIT = (uint)snapShot.LoanTrack.LoanAmount, // always provide
                /*33*/
                CURRENT_BALANCE = isRegistration || (isClosure && !okClosure) ? (uint)snapShot.CurrentBalance : 0, // zero for C,P,S,T,V,F,M,H,G,K,S
                /*34*/
                CURRENT_BALANCE_INDICATOR = "D", //for registrations and closures, must be set to 'D'                                               
                /*35*/
                AMOUNT_OVERDUE = (uint)overdueAmt,
                /*36*/
                INSTALMENT_AMOUNT = isRegistration || (isClosure && !okClosure) ? (uint)snapShot.InstalmentAmount : 0, // zero for codes 'C', 'V', 'T'
                /*37*/
                MONTHS_IN_ARREARS = (isClosure && !okClosure) && snapShot.OverdueSince != null ?
                   ((uint)(Math.Floor(batchDate.Subtract(snapShot.OverdueSince.Value).TotalDays / 30))) : 0, // zero for codes 'C', 'V', 'T' + registrations

                /*38*/
                STATUS_CODE = closedStatusCode,
                /*39*/
                REPAYMENT_FREQUENCY = EvoMapping.FrequencyAssToEvoRepayFreq(snapShot.PayFrequency),
                /*40*/
                TERMS = terms,
                /*41*/
                STATUS_DATE = !string.IsNullOrEmpty(closedStatusCode) ? batchDate.ToYyyyMmDd() : 0, // zero fill if no status code
                /*42-45 Old supplier*/
                /*46*/
                HOME_TELEPHONE = EvoStringUtils.CleanTelephone(client.hometel),
                /*47*/
                CELLULAR_TELEPHONE = EvoStringUtils.CleanTelephone(client.cell),
                /*48*/
                WORK_TELEPHONE = EvoStringUtils.CleanTelephone(client.worktel),
                /*49 companyname must not be same as forename*/
                EMPLOYER_DETAIL = workNameClean != foreName1 && workNameClean != foreName2 && workNameClean != surnameClean ? workNameClean : null,
                /*50*/
                INCOME = (uint)snapShot.GrossSalary,
                /*51*/
                INCOME_FREQUENCY = EvoMapping.FrequencyAssToEvolution(snapShot.PayFrequency),
                /*52 */
                OCCUPATION = EvoStringUtils.CleanOccupation(occupation?.name, 20),
                /*                
                  53 THIRD PARTY NAME
                  54 ACCOUNT SOLD TO THIRD PARTY
                  55 NO OF PARTICIPANTS IN JOINT LOAN
                  56 FILLER*/
                /*57*/
                SUPPLIER_REFERENCE_NUMBER = srn,
                /*58*/
                TRANSACTION_DATE = snapShot.SnapshotDate.ToYyyyMmDd()
              });
            }
          }

          var engine = new FileHelperEngine<Daily_Data>();
          using (var sw = new StreamWriter(tempFile, true, System.Text.Encoding.ASCII))
          {
            engine.WriteStream(sw, rows);
          }
          var fi = new FileInfo(tempFile);
          log.Information("Successfully created daily file for {Branch}-{Name}, bytes: {Size}", branch.LegacyBranchNum, branch.Company.Name, fi.Length);
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
