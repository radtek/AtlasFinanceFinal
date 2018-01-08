using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using Dapper;
using FileHelpers;
using Renci.SshNet;

using Atlas.Evolution.Server.Code.Data.ASS_Models;
using Atlas.Evolution.Server.Code.Utils;
using Atlas.Evolution.Server.Code.Layout;
using Atlas.Common.Utils;
using Atlas.Enumerators;
using Atlas.Evolution.Server.Code.PGP;


namespace CreateQEFile
{
  class Program
  {
    static void Main(string[] args)
    {
      var startDate = DateTime.Today.Subtract(TimeSpan.FromDays(366 * 3));
      var startDay = new DateTime(startDate.Year, startDate.Month, 1);
      var endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month - 1, 1).Subtract(TimeSpan.FromDays(1));
      var path = @"D:\Temp";
      var tempFile = Path.Combine(path, $"AT0001_ALL_T702_A_{endDate:yyyyMMdd}_1_1.txt");
      var pgpTempFile = Path.Combine(path, $"AT0001_ALL_T702_A_{endDate:yyyyMMdd}_1_1.txt.pgp");

      #region Get ass branch -> bureau lookup      
      Console.WriteLine("Connecting to atlas_core...");
      ILookup<string, string> bureaus;
      using (var conn = new Npgsql.NpgsqlConnection(ConfigurationManager.ConnectionStrings["Atlas_Core_Npgsql"].ConnectionString))
      {
        conn.Open();
        bureaus = conn.Query<bureau>(
                "SELECT s.\"BranchCode\" AS cs_branch, b.\"LegacyBranchNum\" as brnum " +
                "FROM \"BUR_Service\" s " +
                "JOIN \"BRN_Branch\" b ON b.\"BranchId\" = s.\"BranchId\" " +
                "WHERE \"ServiceTypeId\" = 0 ").ToLookup(k => k.brnum, v => v.cs_branch);

        Console.WriteLine($"Loaded {bureaus.Count} bureau rows");
      }
      #endregion

      #region Get ASS data. Load all into RAM for performance, else this will take hours (uses 9GB)
      Console.WriteLine("Connecting to ass...");
      ILookup<string, trans> transactions;
      ILookup<string, clients> clients;
      List<loans> allLoans;

      using (var conn = new Npgsql.NpgsqlConnection(ConfigurationManager.ConnectionStrings["Ass_Npgsql"].ConnectionString))
      {
        conn.Open();

        Console.WriteLine("Loading loans...");
        // Get all loans for past 36 months
        allLoans = conn.Query<loans>(
          "SELECT brnum, client, loan, outamnt, cheque, loandate, loanmeth, loan, hovrdate, payno, status, basic, popup_lr, outamnt " +
          "FROM company.loans " +
          "WHERE nctrantype IN ('USE', 'N/A') AND " +
          "loandate BETWEEN @startDay AND @endDate", new { startDay, endDate }).ToList();
        Console.WriteLine($"Loaded {allLoans.Count} loans");
        // 370MB

        // Get all loans since 2012 -> filtering on multiple br/cli/loan is too slow
        Console.WriteLine("Loading trans...");
        transactions = conn.Query<trans>($"SELECT brnum, client, loan, trdate, trtype, " +
          "instinitfe, instinitva, interest, servicefee, servicevat, inspremval, inspremvat, inspolival, inspolivat " +
          $"FROM company.trans WHERE trdate >= '2012-08-01'", null, null, true, 500)
          .ToLookup(k => $"{k.brnum}{k.client}{k.loan}", v => v);
        Console.WriteLine($"Loaded {transactions.Count} transactions");
        // 12GB

        Console.WriteLine("Loading clients...");
        clients = conn.Query<clients>(
          $"SELECT identno, title, firstname, surname, othname, salaryfreq, gender, brnum, client,birthdate, workname, haddr1, haddr2, haddr3, hpostcode, home_owner, hometel, cell, worktel " +
          "FROM company.client", null, null, true, 180)
          .ToLookup(k => $"{k.brnum}{k.client}", v => v);
        Console.WriteLine($"Loaded {clients.Count} clients");
      }
      #endregion

      using (var sw = new StreamWriter(tempFile, false, Encoding.ASCII))
      {
        #region Header
        var header = new FileHelperEngine<Monthly_Header>();
        var headerRow = new Monthly_Header
        {
          FILE_CREATION_DATE = DateTime.Today.ToYyyyMmDd(),
          MONTH_END_DATE = endDate.ToYyyyMmDd(),
          SUPPLIER_REFERENCE_NUMBER = "AT0001",
        };
        header.WriteStream(sw, new List<Monthly_Header> { headerRow });
        #endregion

        var totalRows = 0;

        #region Rows
        var data = new FileHelperEngine<Monthly_Data>();
             
        foreach (var loan in allLoans)
        {
          #region Get data & clean
          var client = clients[$"{loan.brnum}{loan.client}"]?.FirstOrDefault();
          if (client == null)
          {
            Console.WriteLine($"Missing client {loan.brnum}-{loan.client}");
            continue;
          }

          var loanTrans = transactions[$"{loan.brnum}{loan.client}{loan.loan}"]
            .OrderBy(s => s.trdate)
            .ToList();

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

          var isClosure = (loan.outamnt == 0) && (loan.status != "P" && loan.status != "N" && !string.IsNullOrEmpty(loan.status));
          var evoClosedStatusCode = isClosure ? EvoMapping.ClosedLoanStatusAssToEvoStatusCode(loan.status.Trim()) : null;
          var okClosure = isClosure ? EvoStringUtils.IsPositiveOutcome(evoClosedStatusCode) : true;

          var overdueAmount = EvoCalcs.OverdueAmount(loanTrans, endDate);
          var lastReceipt = EvoCalcs.LastReceipt(loanTrans, endDate);
          var currentBalance = EvoCalcs.CurrentBalance(loan.cheque, loanTrans, endDate);
          var overdueSince = EvoCalcs.OverdueSince(loanTrans, endDate);
          var daysOverdue = (overdueSince != null) ? endDate.Subtract(overdueSince.Value).TotalDays : 0;

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
          #endregion

          #region Checks
          // Terms must be 1 for account type M
          var terms = (EvoCalcs.LoanIsMoreThan1Month(loan.loanmeth.Trim(), (int)loan.payno.Value)) ?
                       EvoCalcs.GetLoanPeriodInMonths(loan.loanmeth.Trim(), (int)loan.payno.Value) : 1; // Manual: "Must be supplied as 0001 in the case of a 1 month loan"

          // zero for C,P,S,T,V,F,M,H,G,K,S    
          if (isClosure && okClosure)
          {
            currentBalance = 0;
            overdueAmount = 0;
            overdueSince = null;
            daysOverdue = 0;
          }

          //// Balance cannot be less R100, unless a normal closure
          if (currentBalance < 100)
          {
            if (!isClosure || (isClosure && !okClosure))
            {
              Console.WriteLine("Balance <R100 for open/bad loan- skipping");
              continue;
            }
            currentBalance = 0;
          }

          // Overdue not on normal closed             
          if (overdueAmount != 0 && overdueAmount < 100)
          {
            Console.WriteLine("Overdue <100");
            overdueAmount = 0;
          }

          // zero for codes 'C', 'V', 'T' + registrations
          var monthsInArrears = daysOverdue > 30 && ((isClosure && !okClosure) || !isClosure) ? ((uint)(Math.Floor(daysOverdue / 30))) : 0;
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

          var dateOfLastPayment = (isClosure && okClosure) ? lastReceipt : overdueSince;
          // For bad closures, use oldest date we can...
          if (isClosure && !okClosure && overdueSince != null && dateOfLastPayment > overdueSince)
          {
            dateOfLastPayment = overdueSince;
          }
          #endregion

          var row = new Monthly_Data
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
            BRANCH_CODE = bureaus[client.brnum.Trim()].First(),                                 // Do not zero fill or pad
                                                                                                /* 7*/
            ACCOUNT_NO = client.client.Trim(),               // Do not zero fill or pad
                                                             /* 8*/
            SUB_ACCOUNT_NO = loan.loan.Trim(),             // Do not zero fill or pad

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
            LOAN_REASON_CODE = EvoMapping.LoanReasonAssToEvolution(loan.popup_lr.Trim()),
            /*27*/
            PAYMENT_TYPE = "00",  // Use if not listed below "Payment Types and priority table"                                          
                                  /*28*/
            TYPE_OF_ACCOUNT = EvoCalcs.LoanIsMoreThan1Month(loan.loanmeth.Trim(), (int)loan.payno.Value) ? "P" : "M", // Personal loan // "M" - One Month Personal Loan                                     
                                                                                                                      /*29*/
            DATE_ACCOUNT_OPENED = loan.loandate.ToYyyyMmDd(),
            /*30 Deferred payment date*/
            /*31*/
            DATE_OF_LAST_PAYMENT = dateOfLastPayment.ToYyyyMmDd(),
            /*32*/
            OPENING_BALANCE_CREDIT_LIMIT = (uint)loan.cheque, // always provide                                                                                        
                                                              /*33*/
            CURRENT_BALANCE = (uint)Math.Abs(currentBalance),
            /*34*/
            CURRENT_BALANCE_INDICATOR = isClosure ? "D" :
                   currentBalance >= 0 ? "D" : "C", //for registrations and closures, must be set to 'D' , d- debit, c-credit                                                    
                                                    /*35*/
            AMOUNT_OVERDUE = (uint)overdueAmount, // zero for codes 'C', 'V', 'T', ZERO FOR REGS                                                                                                   
                                                  /*36*/
            INSTALMENT_AMOUNT = (isClosure && !okClosure) ? EvoMapping.InstallmentPerMonth(loan.loanmeth.Trim(), loan.tramount) : 0, // zero for codes 'C', 'V', 'T'                                                                                                                           
                                                                                                                                     /*37*/
            MONTHS_IN_ARREARS = monthsInArrears, // zero for codes 'C', 'V', 'T' + registrations
                                                 /*38*/
            STATUS_CODE = evoClosedStatusCode,
            /*39*/
            REPAYMENT_FREQUENCY = EvoMapping.FrequencyAssToEvoRepayFreq(loan.loanmeth.Trim()),
            /*40*/
            TERMS = terms,
            /*41*/
            STATUS_DATE = !string.IsNullOrEmpty(evoClosedStatusCode) ? endDate.ToYyyyMmDd() : 0, // zero fill if no status code
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
            INCOME = (uint)EvoCalcs.GetMonthSalary(loan.loanmeth.Trim(), (loan.basic ?? 0)), // loan.monthly2= nett salary, loan.basic = gross salary,
                                                                                             /*51*/
            INCOME_FREQUENCY = EvoMapping.FrequencyAssToEvolution(loan.loanmeth.Trim()),

            OCCUPATION = ""//EvoStringUtils.CleanOccupation(occupation?.name, 20)
          };

          data.WriteStream(sw, new[] { row });
          if (totalRows > 0 && totalRows++ % 5000 == 0)
          {
            Console.WriteLine($"{totalRows - 1} rows written...");
          }
        }
        #endregion

        #region Footer           
        var trailer = new FileHelperEngine<Monthly_Trailer>();
        var trailerRow = new Monthly_Trailer { NUMBER_OF_RECORDS = (uint)totalRows + 2 }; // count must include header/trailer
        trailer.WriteStream(sw, new List<Monthly_Trailer> { trailerRow });
        #endregion

        Console.WriteLine($"Completed. ${totalRows} rows written");
      }

      #region Encrypt and upload
      Console.WriteLine("Encrypting file...");
      var encrypted = PgpEncrypt.EncryptFile(
        Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), tempFile, pgpTempFile);

      if (encrypted)
      {
        Console.WriteLine("Uploading file...");
        var pgpFileInfo = new FileInfo(pgpTempFile);

        var timer = Stopwatch.StartNew();
        using (var ftpClient = new SftpClient(GetSftpConnection))
        {
          ftpClient.Connect();
          var rootDir = "/AT0001";
          if (!string.IsNullOrEmpty(rootDir))
          {
            ftpClient.ChangeDirectory(rootDir);
          }
          using (var source = File.OpenRead(pgpTempFile))
          {
            ftpClient.UploadFile(source, Path.GetFileName(pgpTempFile));
          }
        }
        timer.Stop();
        Console.WriteLine($"File uploaded in {timer.ElapsedMilliseconds:N0}ms");
        File.Delete(pgpTempFile);
      }
      #endregion

      File.Delete(tempFile);

      Console.WriteLine("Done. Press a key...");
      Console.ReadKey();
    }


    /// <summary>
    /// SFTP connection parameters
    /// </summary>
    private static ConnectionInfo GetSftpConnection
    {
      get
      {
        var config = System.Configuration.ConfigurationManager.AppSettings;

        var host = config["Sftp_Host"];
        var port = 22;
        var userName = config["Sftp_UserName"];
        var password = config["Sftp_Password"];
        return new ConnectionInfo(host, port, userName, new AuthenticationMethod[] { new PasswordAuthenticationMethod(userName, password) });
      }
    }

  }

}
