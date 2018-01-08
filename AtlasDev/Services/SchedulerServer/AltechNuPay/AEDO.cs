using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

using DevExpress.Xpo;
using Npgsql;

using Atlas.Common.Extensions;
using SchedulerServer.AltechNuPay.Report.AEDO.Structures;
using SchedulerServer.AltechNuPay.Report.AEDO;
using SchedulerServer.AltechNuPay.Report;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Common.Interface;


namespace SchedulerServer.AltechNuPay
{
  public class AEDO
  {
    /// <summary>
    /// How far into the future to run the future report
    /// </summary>
    private const int FUTURE_REPORT_DAYS_MAX = 365 + 31;

    /// <summary>
    /// Retrieves Report from NuPay and stores into DB depending on type of report and date range
    /// </summary>
    public static void ImportReports(ILogging log, IConfigSettings config, bool futureReportRun = false)
    {
      var logins = new List<AEDOLoginDTO>();
      var reporting = Atlas.Enumerators.Credentials.CredentialPurpose.Report.ToInt();
      using (var uow = new UnitOfWork())
      {
        // Get all AEDO logins and add to 'logins' List to Loop through
        var aedoLogins = new XPQuery<AEDOLogin>(uow).Where(l => l.DeletedDT == null).ToList();
        aedoLogins.ForEach(r =>
        {
          if ((r.CredentialPurposeFlags & reporting) == reporting)
          {
            logins.Add(new AEDOLoginDTO
            {
              AEDOLoginId = r.AEDOLoginId,
              MerchantNum = r.MerchantNum,
              Password = r.Password,
              DeletedDT = r.DeletedDT
            });
          }
        });
      }

      log.Information("[AEDO] Found {Logins} AEDO logins", logins.Count);

      foreach (Enumerators.AEDO.ReportType reportType in Enum.GetValues(typeof(Enumerators.AEDO.ReportType)))
      {
        log.Information("[AEDO] Running report: {reportType}...", reportType);
        if (reportType == Enumerators.AEDO.ReportType.FutureTransactions && futureReportRun)
        {
          log.Information("[AEDO] Truncating AEDOReportFuture...");
          var connStrCore = config.GetAtlasCoreConnectionString();// ConfigurationManager.ConnectionStrings["atlas_core"].ConnectionString;
          using (var conn = new NpgsqlConnection(connStrCore))
          {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
              cmd.CommandType = CommandType.Text;
              cmd.CommandTimeout = (int)TimeSpan.FromMinutes(20).TotalSeconds;
              cmd.CommandText = "TRUNCATE TABLE \"AEDOReportFuture\"";
              cmd.ExecuteNonQuery();

              cmd.CommandText = string.Format("DELETE FROM \"AEDOReportBatch\" WHERE \"ReportType\" = {0}", reportType.ToInt());
              cmd.ExecuteNonQuery();
            }
          }
          log.Information("[NAEDO] Truncate NAEDOReportFuture complete");
        }
        else if ((reportType != Enumerators.AEDO.ReportType.FutureTransactions && futureReportRun) ||
                 (reportType == Enumerators.AEDO.ReportType.FutureTransactions && !futureReportRun))
        {
          continue;
        }

        // loop through every login to get reports and import
        foreach (var login in logins)
        {
          DateTime startDate;
          var endDate = DateTime.Now;
          var canQuery = true;

          if (reportType == Enumerators.AEDO.ReportType.FutureTransactions)
          {
            startDate = DateTime.Today;
            endDate = startDate.AddDays(2);
          }
          else
          {
            using (var uow = new UnitOfWork())
            {
              // Get Last Report End Date Range for new report start date range
              startDate = (new XPQuery<AEDOReportBatch>(uow)
                .Where(r => r.ReportType == reportType.ToInt() && r.IsSuccess && r.AEDOLoginId == login.AEDOLoginId)
                .Max(r => r.EndDT)).Date;

              // If there was never a report pulled before, set default to 1 year ago
              if (startDate == null || startDate.Year == 1)
              {
                startDate = DateTime.Today.AddYears(-1);
              }

              if ((endDate - startDate).Days > 2)
              {
                endDate = startDate.AddDays(2);
              }
            }
          }

          while (canQuery)
          {
            log.Information("[AEDO] Running report for {reportType}, {Start:yyyy-MM-dd HH:mm:ss}-{End:yyyy-MM-dd HH:mm:ss}...", reportType, startDate, endDate);
            // call method to do report import 
            var errorMessage = string.Empty;
            var isSuccess = true;
            var report = Enquiry.GetReport(log, login, (int)reportType, reportType.ToStringEnum(),
              Enumerators.AEDO.UserType.Group.ToStringEnum(),
              startDate, endDate, ref isSuccess, ref errorMessage);

            log.Information("[AEDO] Adding report for {reportType}, {Start:yyyy-MM-dd HH:mm:ss}-{End:yyyy-MM-dd HH:mm:ss}...", reportType, startDate, endDate);
            AddReportToDB(log, login, reportType, report, startDate, endDate, isSuccess, errorMessage);
            log.Information("[AEDO] Completed {reportType}, {Start:yyyy-MM-dd HH:mm:ss}-{End:yyyy-MM-dd HH:mm:ss}...", reportType, startDate, endDate);

            if (reportType == Enumerators.AEDO.ReportType.FutureTransactions)
            {
              if ((endDate - DateTime.Today).Days > FUTURE_REPORT_DAYS_MAX)
              {
                canQuery = false;
              }
              else
              {
                startDate = endDate;
                endDate = endDate.AddDays(2);
                canQuery = true;
              }
            }
            else
            {
              if ((DateTime.Today - endDate).Days > 0)
              {
                startDate = endDate;
                endDate = DateTime.Today;

                if ((endDate - startDate).Days > 2)
                  endDate = startDate.AddDays(2);
                canQuery = true;
              }
              else
              {
                canQuery = false;
              }
            }
          }
        }
      }
    }


    private static void AddReportToDB(ILogging log, AEDOLoginDTO aedoLogin, Enumerators.AEDO.ReportType reportType, XElement xmlReport, DateTime startDate, DateTime endDate,
        bool isSuccess, string errorMessage)
    {
      if (xmlReport != null)
      {
        var reportHeaderElement = xmlReport.Element("ReportHeader");
        if (reportHeaderElement != null)
        {
          using (var unitOfWork = new UnitOfWork())
          {
            FileStructureHelper.TrimElementValues(ref reportHeaderElement);

            var reportHeader = new ReportHeader(reportHeaderElement);
            var reportBatch = AddBatch(reportHeader, reportType, aedoLogin, unitOfWork, startDate, endDate, isSuccess, errorMessage);
            var repeatBlockNumber = reportHeader.AEDOReportBatch.BlockNum;

            var transactions = xmlReport.Elements(reportType.ToStringEnum());

            for (var j = repeatBlockNumber; j > 0; j--)
            {
              if (j < repeatBlockNumber)
              {
                var report = Enquiry.GetReport(log, aedoLogin, (int)reportType, reportType.ToStringEnum(),
                    Enumerators.AEDO.UserType.Group.ToStringEnum(),
                    startDate, endDate, ref isSuccess, ref errorMessage, reportHeader.AEDOReportBatch.TokenNum,
                    int.Parse(j.ToString()), allowedRetryAttempts: 20);
                if (report != null)
                {
                  transactions = report.Elements(reportType.ToStringEnum());
                }
              }

              // Remove Duplicate Records that are passed through from NuPay
              transactions = transactions.Distinct(new FileStructureHelper.XElementValueEqualityComparer());

              var i = 0;
              foreach (var transaction in transactions)
              {
                i++;
                var tran = transaction;
                FileStructureHelper.TrimElementValues(ref tran);
                switch (reportType)
                {
                  case Enumerators.AEDO.ReportType.SuccessfulTransactions:
                    AddSuccessReport(new SuccessReport(tran), unitOfWork, reportBatch);
                    break;

                  case Enumerators.AEDO.ReportType.FailedTransactions:
                    AddFailedReport(new FailedReport(tran), unitOfWork, reportBatch);
                    break;

                  case Enumerators.AEDO.ReportType.RetryTransactions:
                    AddRetryReport(new RetryReport(tran), unitOfWork, reportBatch);
                    break;

                  case Enumerators.AEDO.ReportType.FutureTransactions:
                    AddFutureReport(new FutureReport(tran), unitOfWork, reportBatch);
                    break;

                  case Enumerators.AEDO.ReportType.CancelledTransactions:
                    AddCancelledReport(new CancelledReport(tran), unitOfWork, reportBatch);
                    break;

                  case Enumerators.AEDO.ReportType.NewTransactionsLoaded:
                    AddNewTransactionReport(new NewTransactionReport(tran), unitOfWork, reportBatch);
                    break;

                  case Enumerators.AEDO.ReportType.SettledTransaction:
                    AddSettledReport(new SettledReport(tran), unitOfWork, reportBatch);
                    break;

                  case Enumerators.AEDO.ReportType.UnsettledTransaction:
                    AddUnsettledReport(new UnsettledReport(tran), unitOfWork, reportBatch);
                    break;

                  case Enumerators.AEDO.ReportType.UnmatchedTransaction:
                    AddUnmatched(new UnmatchedReport(tran), unitOfWork, reportBatch);
                    break;
                }
                // Commit to DB every 100 records
                if (i >= 100)
                {
                  i = 0;
                  unitOfWork.CommitChanges();
                }
              }
            }

            unitOfWork.CommitChanges();
          }
        }
        else
        {
          using (var unitOfWork = new UnitOfWork())
          {
            AddBatch(null, reportType, aedoLogin, unitOfWork, startDate, endDate, isSuccess, errorMessage);
            unitOfWork.CommitChanges();
          }
        }
      }
      else
      {
        using (var unitOfWork = new UnitOfWork())
        {
          AddBatch(null, reportType, aedoLogin, unitOfWork, startDate, endDate, isSuccess, errorMessage);
          unitOfWork.CommitChanges();
        }
      }
    }


    private static AEDOReportBatch AddBatch(ReportHeader reportHeader, Enumerators.AEDO.ReportType reportType,
      AEDOLoginDTO aedoLogin, UnitOfWork unitOfWork, DateTime startDate, DateTime endDate,
        bool isSuccess, string errorrMessage)
    {
      var reportBatch = new AEDOReportBatch(unitOfWork)
            {
              AEDOLoginId = aedoLogin.AEDOLoginId,
              IsSuccess = isSuccess,
              ErrorMessage = errorrMessage,
              StartDT = startDate,
              EndDT = endDate,
              ReportType = (int)reportType
            };

      if (reportHeader != null)
      {
        reportBatch.BlockNum = reportHeader.AEDOReportBatch.BlockNum;
        reportBatch.CreatedDT = reportHeader.AEDOReportBatch.CreatedDT;
        reportBatch.ReportFromDT = reportHeader.AEDOReportBatch.ReportFromDT;
        reportBatch.ReportToDT = reportHeader.AEDOReportBatch.ReportToDT;
        reportBatch.MerchantNum = reportHeader.AEDOReportBatch.MerchantNum;
        reportBatch.ReportGenerationDT = reportHeader.AEDOReportBatch.ReportGenerationDT;
        reportBatch.TokenNum = reportHeader.AEDOReportBatch.TokenNum;
      }
      else
      {
        reportBatch.CreatedDT = DateTime.Now;
      }

      return reportBatch;
    }


    private static void AddUnmatched(UnmatchedReport report, UnitOfWork unitOfWork, AEDOReportBatch reportBatch)
    {
      var aedoReport = unitOfWork.Query<AEDOReportUnmatched>().FirstOrDefault(n => n.SettlementDT == report.aedoReport.SettlementDT &&
                  n.TermId == report.aedoReport.TermId &&
                  n.MerchantNum == report.aedoReport.MerchantNum &&
                  n.Pan == report.aedoReport.Pan &&
                  n.TransactionAmount == report.aedoReport.TransactionAmount &&
                  n.ContractRef == report.aedoReport.ContractRef);

      if (aedoReport == null)
      {
        aedoReport = unitOfWork.GetObjectsToSave().OfType<AEDOReportUnmatched>().FirstOrDefault(n => n.SettlementDT == report.aedoReport.SettlementDT &&
                n.TermId == report.aedoReport.TermId &&
                n.MerchantNum == report.aedoReport.MerchantNum &&
                n.Pan == report.aedoReport.Pan &&
                n.TransactionAmount == report.aedoReport.TransactionAmount &&
                n.ContractRef == report.aedoReport.ContractRef);
      }

      if (aedoReport == null)
      {
        aedoReport = new AEDOReportUnmatched(unitOfWork);
        aedoReport.SettlementDT = report.aedoReport.SettlementDT;
        aedoReport.TermId = report.aedoReport.TermId;
        aedoReport.MerchantNum = report.aedoReport.MerchantNum;
        aedoReport.Pan = report.aedoReport.Pan;
        aedoReport.TransactionAmount = report.aedoReport.TransactionAmount;
        aedoReport.ContractRef = report.aedoReport.ContractRef;

        aedoReport.ReportBatch = reportBatch;

        aedoReport.Save();
      }
    }


    private static void AddUnsettledReport(UnsettledReport report, UnitOfWork unitOfWork, AEDOReportBatch reportBatch)
    {
      var aedoReport = unitOfWork.Query<AEDOReportUnsettled>().FirstOrDefault(n => n.ReportUnsettled.TransactionId == report.aedoReport.ReportUnsettled.TransactionId &&
                  n.ReportUnsettled.ValueDT == report.aedoReport.ReportUnsettled.ValueDT);

      if (aedoReport == null)
      {
        aedoReport = unitOfWork.GetObjectsToSave().OfType<AEDOReportUnsettled>().FirstOrDefault(n => n.ReportUnsettled.TransactionId == report.aedoReport.ReportUnsettled.TransactionId &&
                n.ReportUnsettled.ValueDT == report.aedoReport.ReportUnsettled.ValueDT);
      }

      if (aedoReport == null)
      {
        aedoReport = new AEDOReportUnsettled(unitOfWork)
        {
          ReportUnsettled = new AEDOReportUnsettled.ReportUnsettledKey()
          {
            TransactionId = report.aedoReport.ReportUnsettled.TransactionId,
            ValueDT = report.aedoReport.ReportUnsettled.ValueDT
          }
        };
        aedoReport.TermId = report.aedoReport.TermId;
        aedoReport.MerchantNum = report.aedoReport.MerchantNum;
        aedoReport.Pan = report.aedoReport.Pan;
        aedoReport.Instalment = report.aedoReport.Instalment;
        aedoReport.ContractAmount = report.aedoReport.ContractAmount;
        aedoReport.ActualDT = report.aedoReport.ActualDT;
        aedoReport.ContractNum = report.aedoReport.ContractNum;
        aedoReport.StartDT = report.aedoReport.StartDT;
        aedoReport.CurrencyCode = report.aedoReport.CurrencyCode;
        aedoReport.Amount = report.aedoReport.Amount;
        aedoReport.Frequency = report.aedoReport.Frequency;
        aedoReport.EmployerCode = report.aedoReport.EmployerCode;
        aedoReport.ContractRef = report.aedoReport.ContractRef;
        aedoReport.IdNumber = report.aedoReport.IdNumber;

        aedoReport.ReportBatch = reportBatch;

        aedoReport.Save();
      }
    }


    private static void AddSettledReport(SettledReport report, UnitOfWork unitOfWork, AEDOReportBatch reportBatch)
    {
      var aedoReport = unitOfWork.Query<AEDOReportSettled>().FirstOrDefault(n => n.ReportSettled.TransactionId == report.aedoReport.ReportSettled.TransactionId &&
                  n.ReportSettled.SettlementDT == report.aedoReport.ReportSettled.SettlementDT &&
                  n.ReportSettled.Instalment == report.aedoReport.ReportSettled.Instalment);

      if (aedoReport == null)
      {
        aedoReport = unitOfWork.GetObjectsToSave().OfType<AEDOReportSettled>().FirstOrDefault(n => n.ReportSettled.TransactionId == report.aedoReport.ReportSettled.TransactionId &&
                n.ReportSettled.SettlementDT == report.aedoReport.ReportSettled.SettlementDT &&
                n.ReportSettled.Instalment == report.aedoReport.ReportSettled.Instalment);
      }

      if (aedoReport == null)
      {
        aedoReport = new AEDOReportSettled(unitOfWork)
        {
          ReportSettled = new AEDOReportSettled.ReportSettledKey()
          {
            TransactionId = report.aedoReport.ReportSettled.TransactionId,
            SettlementDT = report.aedoReport.ReportSettled.SettlementDT,
            Instalment = report.aedoReport.ReportSettled.Instalment
          }
        };
        aedoReport.TermId = report.aedoReport.TermId;
        aedoReport.MerchantNum = report.aedoReport.MerchantNum;
        aedoReport.Pan = report.aedoReport.Pan;
        aedoReport.TransmitDT = report.aedoReport.TransmitDT;
        aedoReport.ContractNum = report.aedoReport.ContractNum;
        aedoReport.TransactionAmount = report.aedoReport.TransactionAmount;
        aedoReport.ContractRef = report.aedoReport.ContractRef;
        aedoReport.IdNumber = report.aedoReport.IdNumber;

        aedoReport.ReportBatch = reportBatch;

        aedoReport.Save();
      }
    }


    private static void AddNewTransactionReport(NewTransactionReport report, UnitOfWork unitOfWork, AEDOReportBatch reportBatch)
    {
      var aedoReport = unitOfWork.Query<AEDOReportNewTransaction>().FirstOrDefault(n => n.ReportNewTransaction.TransactionId == report.aedoReport.ReportNewTransaction.TransactionId &&
                  n.ReportNewTransaction.ContractRef == report.aedoReport.ReportNewTransaction.ContractRef &&
                  n.ReportNewTransaction.ServiceType == report.aedoReport.ReportNewTransaction.ServiceType &&
                  n.ReportNewTransaction.ValueDT == report.aedoReport.ReportNewTransaction.ValueDT &&
                  n.ReportNewTransaction.SubmitDT == report.aedoReport.ReportNewTransaction.SubmitDT);

      if (aedoReport == null)
      {
        aedoReport = unitOfWork.GetObjectsToSave().OfType<AEDOReportNewTransaction>().FirstOrDefault(n => n.ReportNewTransaction.TransactionId == report.aedoReport.ReportNewTransaction.TransactionId &&
                n.ReportNewTransaction.ContractRef == report.aedoReport.ReportNewTransaction.ContractRef &&
                n.ReportNewTransaction.ServiceType == report.aedoReport.ReportNewTransaction.ServiceType &&
                n.ReportNewTransaction.ValueDT == report.aedoReport.ReportNewTransaction.ValueDT &&
                n.ReportNewTransaction.SubmitDT == report.aedoReport.ReportNewTransaction.SubmitDT);
      }

      if (aedoReport == null)
      {
        aedoReport = new AEDOReportNewTransaction(unitOfWork)
        {
          ReportNewTransaction = new AEDOReportNewTransaction.ReportNewTransactionKey()
          {
            TransactionId = report.aedoReport.ReportNewTransaction.TransactionId,
            ContractRef = report.aedoReport.ReportNewTransaction.ContractRef,
            ServiceType = report.aedoReport.ReportNewTransaction.ServiceType,
            ValueDT = report.aedoReport.ReportNewTransaction.ValueDT,
            SubmitDT = report.aedoReport.ReportNewTransaction.SubmitDT
          }
        };
        aedoReport.StartDT = report.aedoReport.StartDT;
        aedoReport.LastSubmissionDT = report.aedoReport.LastSubmissionDT;
        aedoReport.SubmitCount = report.aedoReport.SubmitCount;
        aedoReport.RetryReason = report.aedoReport.RetryReason;
        aedoReport.ContractNum = report.aedoReport.ContractNum;
        aedoReport.ContractAmount = report.aedoReport.ContractAmount;
        aedoReport.Term = report.aedoReport.Term;
        aedoReport.Instalments = report.aedoReport.Instalments;
        aedoReport.InstalmentNum = report.aedoReport.InstalmentNum;
        aedoReport.InstalmentAmount = report.aedoReport.InstalmentAmount;
        aedoReport.EmployerCode = report.aedoReport.EmployerCode;
        aedoReport.Frequency = report.aedoReport.Frequency;
        aedoReport.DateAdjustRule = report.aedoReport.DateAdjustRule;
        aedoReport.TrackingIndicator = report.aedoReport.TrackingIndicator;
        aedoReport.Pan = report.aedoReport.Pan;
        aedoReport.TerminalNum = report.aedoReport.TerminalNum;
        aedoReport.CardAcceptor = report.aedoReport.CardAcceptor;
        aedoReport.Active = report.aedoReport.Active;
        aedoReport.InstitutionId = report.aedoReport.InstitutionId;
        aedoReport.IdNumber = report.aedoReport.IdNumber;

        aedoReport.ReportBatch = reportBatch;

        aedoReport.Save();
      }
    }

    private static void AddFutureReport(FutureReport report, UnitOfWork unitOfWork, AEDOReportBatch reportBatch)
    {
      var aedoReport = unitOfWork.Query<AEDOReportFuture>().FirstOrDefault(n => n.ReportFuture.TransactionId == report.aedoReport.ReportFuture.TransactionId &&
                  n.ReportFuture.ContractRef == report.aedoReport.ReportFuture.ContractRef &&
                  n.ReportFuture.ServiceType == report.aedoReport.ReportFuture.ServiceType &&
                  n.ReportFuture.ValueDT == report.aedoReport.ReportFuture.ValueDT &&
                  n.ReportFuture.SubmitDT == report.aedoReport.ReportFuture.SubmitDT);

      if (aedoReport == null)
      {
        aedoReport = unitOfWork.GetObjectsToSave().OfType<AEDOReportFuture>().FirstOrDefault(n => n.ReportFuture.TransactionId == report.aedoReport.ReportFuture.TransactionId &&
                n.ReportFuture.ContractRef == report.aedoReport.ReportFuture.ContractRef &&
                n.ReportFuture.ServiceType == report.aedoReport.ReportFuture.ServiceType &&
                n.ReportFuture.ValueDT == report.aedoReport.ReportFuture.ValueDT &&
                n.ReportFuture.SubmitDT == report.aedoReport.ReportFuture.SubmitDT);
      }

      if (aedoReport == null)
      {
        aedoReport = new AEDOReportFuture(unitOfWork)
        {
          ReportFuture = new AEDOReportFuture.ReportFutureKey()
          {
            TransactionId = report.aedoReport.ReportFuture.TransactionId,
            ContractRef = report.aedoReport.ReportFuture.ContractRef,
            ServiceType = report.aedoReport.ReportFuture.ServiceType,
            ValueDT = report.aedoReport.ReportFuture.ValueDT,
            SubmitDT = report.aedoReport.ReportFuture.SubmitDT
          }
        };

        aedoReport.StartDT = report.aedoReport.StartDT;
        aedoReport.LastSubmissionDT = report.aedoReport.LastSubmissionDT;
        aedoReport.SubmitCount = report.aedoReport.SubmitCount;
        aedoReport.RetryReason = report.aedoReport.RetryReason;
        aedoReport.ContractNum = report.aedoReport.ContractNum;
        aedoReport.ContractAmount = report.aedoReport.ContractAmount;
        aedoReport.Term = report.aedoReport.Term;
        aedoReport.Instalments = report.aedoReport.Instalments;
        aedoReport.InstalmentNum = report.aedoReport.InstalmentNum;
        aedoReport.InstalmentAmount = report.aedoReport.InstalmentAmount;
        aedoReport.EmployerCode = report.aedoReport.EmployerCode;
        aedoReport.Frequency = report.aedoReport.Frequency;
        aedoReport.DateAdjustRule = report.aedoReport.DateAdjustRule;
        aedoReport.TrackingIndicator = report.aedoReport.TrackingIndicator;
        aedoReport.Pan = report.aedoReport.Pan;
        aedoReport.TerminalNum = report.aedoReport.TerminalNum;
        aedoReport.Active = report.aedoReport.Active;
        aedoReport.CardAcceptor = report.aedoReport.CardAcceptor;
        aedoReport.InstitutionId = report.aedoReport.InstitutionId;
        aedoReport.IdNumber = report.aedoReport.IdNumber;

        aedoReport.ReportBatch = reportBatch;

        aedoReport.Save();
      }
    }


    private static void AddRetryReport(RetryReport report, UnitOfWork unitOfWork, AEDOReportBatch reportBatch)
    {
      var aedoReport = unitOfWork.Query<AEDOReportRetry>().FirstOrDefault(n => n.ReportRetry.TransactionId == report.aedoReport.ReportRetry.TransactionId &&
                  n.ReportRetry.ContractRef == report.aedoReport.ReportRetry.ContractRef &&
                  n.ReportRetry.ServiceType == report.aedoReport.ReportRetry.ServiceType &&
                  n.ReportRetry.ValueDT == report.aedoReport.ReportRetry.ValueDT &&
                  n.ReportRetry.SubmitDT == report.aedoReport.ReportRetry.SubmitDT);

      if (aedoReport == null)
      {
        aedoReport = unitOfWork.GetObjectsToSave().OfType<AEDOReportRetry>().FirstOrDefault(n => n.ReportRetry.TransactionId == report.aedoReport.ReportRetry.TransactionId &&
                n.ReportRetry.ContractRef == report.aedoReport.ReportRetry.ContractRef &&
                n.ReportRetry.ServiceType == report.aedoReport.ReportRetry.ServiceType &&
                n.ReportRetry.ValueDT == report.aedoReport.ReportRetry.ValueDT &&
                n.ReportRetry.SubmitDT == report.aedoReport.ReportRetry.SubmitDT);
      }

      if (aedoReport == null)
      {
        aedoReport = new AEDOReportRetry(unitOfWork)
        {
          ReportRetry = new AEDOReportRetry.ReportRetryKey()
          {
            TransactionId = report.aedoReport.ReportRetry.TransactionId,
            ContractRef = report.aedoReport.ReportRetry.ContractRef,
            ServiceType = report.aedoReport.ReportRetry.ServiceType,
            ValueDT = report.aedoReport.ReportRetry.ValueDT,
            SubmitDT = report.aedoReport.ReportRetry.SubmitDT
          }
        };
        aedoReport.StartDT = report.aedoReport.StartDT;
        aedoReport.LastSubmissionDT = report.aedoReport.LastSubmissionDT;
        aedoReport.SubmitCount = report.aedoReport.SubmitCount;
        aedoReport.RetryReason = report.aedoReport.RetryReason;
        aedoReport.ContractNum = report.aedoReport.ContractNum;
        aedoReport.ContractAmount = report.aedoReport.ContractAmount;
        aedoReport.Term = report.aedoReport.Term;
        aedoReport.Instalments = report.aedoReport.Instalments;
        aedoReport.InstalmentNum = report.aedoReport.InstalmentNum;
        aedoReport.InstalmentAmount = report.aedoReport.InstalmentAmount;
        aedoReport.EmployerCode = report.aedoReport.EmployerCode;
        aedoReport.Frequency = report.aedoReport.Frequency;
        aedoReport.DateAdjustRule = report.aedoReport.DateAdjustRule;
        aedoReport.TrackingIndicator = report.aedoReport.TrackingIndicator;
        aedoReport.Pan = report.aedoReport.Pan;
        aedoReport.TerminalNum = report.aedoReport.TerminalNum;
        aedoReport.CardAcceptor = report.aedoReport.CardAcceptor;
        aedoReport.InstitutionId = report.aedoReport.InstitutionId;
        aedoReport.IdNumber = report.aedoReport.IdNumber;

        aedoReport.ReportBatch = reportBatch;

        aedoReport.Save();
      }
    }


    private static void AddCancelledReport(CancelledReport report, UnitOfWork unitOfWork, AEDOReportBatch reportBatch)
    {
      var aedoReport = unitOfWork.Query<AEDOReportCancelled>().FirstOrDefault(n => n.ReportCancelled.TransactionId == report.aedoReport.ReportCancelled.TransactionId &&
                  n.ReportCancelled.ContractRef == report.aedoReport.ReportCancelled.ContractRef &&
                  n.ReportCancelled.ServiceType == report.aedoReport.ReportCancelled.ServiceType &&
                  n.ReportCancelled.ValueDT == report.aedoReport.ReportCancelled.ValueDT &&
                  n.ReportCancelled.CancelDT == report.aedoReport.ReportCancelled.CancelDT);

      if (aedoReport == null)
      {
        aedoReport = unitOfWork.GetObjectsToSave().OfType<AEDOReportCancelled>().FirstOrDefault(n => n.ReportCancelled.TransactionId == report.aedoReport.ReportCancelled.TransactionId &&
                n.ReportCancelled.ContractRef == report.aedoReport.ReportCancelled.ContractRef &&
                n.ReportCancelled.ServiceType == report.aedoReport.ReportCancelled.ServiceType &&
                n.ReportCancelled.ValueDT == report.aedoReport.ReportCancelled.ValueDT &&
                n.ReportCancelled.CancelDT == report.aedoReport.ReportCancelled.CancelDT);
      }

      if (aedoReport == null)
      {
        aedoReport = new AEDOReportCancelled(unitOfWork)
        {
          ReportCancelled = new AEDOReportCancelled.ReportCancelledKey()
          {
            TransactionId = report.aedoReport.ReportCancelled.TransactionId,
            ContractRef = report.aedoReport.ReportCancelled.ContractRef,
            ServiceType = report.aedoReport.ReportCancelled.ServiceType,
            ValueDT = report.aedoReport.ReportCancelled.ValueDT,
            CancelDT = report.aedoReport.ReportCancelled.CancelDT
          }
        };

        aedoReport.TrackingIndicator = report.aedoReport.TrackingIndicator;
        aedoReport.ContractNum = report.aedoReport.ContractNum;
        aedoReport.CancellationType = report.aedoReport.CancellationType;
        aedoReport.CancelMerchant = report.aedoReport.CancelMerchant;
        aedoReport.EmployerCode = report.aedoReport.EmployerCode;
        aedoReport.Pan = report.aedoReport.Pan;
        aedoReport.TerminalNum = report.aedoReport.TerminalNum;
        aedoReport.InstitutionId = report.aedoReport.InstitutionId;
        aedoReport.IdNumber = report.aedoReport.IdNumber;

        aedoReport.ReportBatch = reportBatch;

        aedoReport.Save();
      }
    }


    private static void AddSuccessReport(SuccessReport report, UnitOfWork unitOfWork, AEDOReportBatch reportBatch)
    {
      var aedoReport = unitOfWork.Query<AEDOReportSuccess>().FirstOrDefault(n => n.ReportSuccess.TransactionId == report.aedoReport.ReportSuccess.TransactionId &&
                  n.ReportSuccess.ContractRef == report.aedoReport.ReportSuccess.ContractRef &&
                  n.ReportSuccess.ServiceType == report.aedoReport.ReportSuccess.ServiceType &&
                  n.ReportSuccess.ValueDT == report.aedoReport.ReportSuccess.ValueDT &&
                  n.ReportSuccess.SuccessDT == report.aedoReport.ReportSuccess.SuccessDT);

      if (aedoReport == null)
      {
        aedoReport = unitOfWork.GetObjectsToSave().OfType<AEDOReportSuccess>().FirstOrDefault(n => n.ReportSuccess.TransactionId == report.aedoReport.ReportSuccess.TransactionId &&
                n.ReportSuccess.ContractRef == report.aedoReport.ReportSuccess.ContractRef &&
                n.ReportSuccess.ServiceType == report.aedoReport.ReportSuccess.ServiceType &&
                n.ReportSuccess.ValueDT == report.aedoReport.ReportSuccess.ValueDT &&
                n.ReportSuccess.SuccessDT == report.aedoReport.ReportSuccess.SuccessDT);
      }

      if (aedoReport == null)
      {
        aedoReport = new AEDOReportSuccess(unitOfWork)
        {
          ReportSuccess = new AEDOReportSuccess.ReportSuccessKey()
          {
            TransactionId = report.aedoReport.ReportSuccess.TransactionId,
            ContractRef = report.aedoReport.ReportSuccess.ContractRef,
            ServiceType = report.aedoReport.ReportSuccess.ServiceType,
            ValueDT = report.aedoReport.ReportSuccess.ValueDT,
            SuccessDT = report.aedoReport.ReportSuccess.SuccessDT
          }
        };
        aedoReport.StartDT = report.aedoReport.StartDT;
        aedoReport.ContractNum = report.aedoReport.ContractNum;
        aedoReport.ContractAmount = report.aedoReport.ContractAmount;
        aedoReport.InstalmentNum = report.aedoReport.InstalmentNum;
        aedoReport.InstalmentAmount = report.aedoReport.InstalmentAmount;
        aedoReport.EmployerCode = report.aedoReport.EmployerCode;
        aedoReport.TrackingIndicator = report.aedoReport.TrackingIndicator;
        aedoReport.Frequency = report.aedoReport.Frequency;
        aedoReport.Pan = report.aedoReport.Pan;
        aedoReport.TerminalNum = report.aedoReport.TerminalNum;
        aedoReport.CardAcceptor = report.aedoReport.CardAcceptor;
        aedoReport.IdNumber = report.aedoReport.IdNumber;
        aedoReport.InstitutionId = report.aedoReport.InstitutionId;

        aedoReport.ReportBatch = reportBatch;

        aedoReport.Save();
      }
    }


    private static void AddFailedReport(FailedReport report, UnitOfWork unitOfWork, AEDOReportBatch reportBatch)
    {
      var aedoReport = unitOfWork.Query<AEDOReportFailed>().FirstOrDefault(n => n.ReportFailed.TransactionId == report.aedoReport.ReportFailed.TransactionId &&
                  n.ReportFailed.ContractRef == report.aedoReport.ReportFailed.ContractRef &&
                  n.ReportFailed.ServiceType == report.aedoReport.ReportFailed.ServiceType &&
                  n.ReportFailed.ValueDT == report.aedoReport.ReportFailed.ValueDT &&
                  n.ReportFailed.FailDT == report.aedoReport.ReportFailed.FailDT);

      if (aedoReport == null)
      {
        aedoReport = unitOfWork.GetObjectsToSave().OfType<AEDOReportFailed>().FirstOrDefault(n => n.ReportFailed.TransactionId == report.aedoReport.ReportFailed.TransactionId &&
                n.ReportFailed.ContractRef == report.aedoReport.ReportFailed.ContractRef &&
                n.ReportFailed.ServiceType == report.aedoReport.ReportFailed.ServiceType &&
                n.ReportFailed.ValueDT == report.aedoReport.ReportFailed.ValueDT &&
                n.ReportFailed.FailDT == report.aedoReport.ReportFailed.FailDT);
      }

      if (aedoReport == null)
      {
        aedoReport = new AEDOReportFailed(unitOfWork)
        {
          ReportFailed = new AEDOReportFailed.ReportFailedKey()
          {
            TransactionId = report.aedoReport.ReportFailed.TransactionId,
            ContractRef = report.aedoReport.ReportFailed.ContractRef,
            ServiceType = report.aedoReport.ReportFailed.ServiceType,
            ValueDT = report.aedoReport.ReportFailed.ValueDT,
            FailDT = report.aedoReport.ReportFailed.FailDT
          }
        };

        aedoReport.StartDT = report.aedoReport.StartDT;
        aedoReport.ContractNum = report.aedoReport.ContractNum;
        aedoReport.Reason = report.aedoReport.Reason;
        aedoReport.ContractAmount = report.aedoReport.ContractAmount;
        aedoReport.InstalmentNum = report.aedoReport.InstalmentNum;
        aedoReport.InstalmentAmount = report.aedoReport.InstalmentAmount;
        aedoReport.EmployerCode = report.aedoReport.EmployerCode;
        aedoReport.TrackingIndicator = report.aedoReport.TrackingIndicator;
        aedoReport.Frequency = report.aedoReport.Frequency;
        aedoReport.Pan = report.aedoReport.Pan;
        aedoReport.TerminalNum = report.aedoReport.TerminalNum;
        aedoReport.Resubmit = report.aedoReport.Resubmit;
        aedoReport.CardAcceptor = report.aedoReport.CardAcceptor;
        aedoReport.InstitutionId = report.aedoReport.InstitutionId;
        aedoReport.IdNumber = report.aedoReport.IdNumber;

        aedoReport.ReportBatch = reportBatch;

        aedoReport.Save();
      }
    }

  }
}