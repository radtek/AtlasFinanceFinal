using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

using DevExpress.Xpo;
using Npgsql;

using SchedulerServer.AltechNuPay.Report.NAEDO;
using SchedulerServer.AltechNuPay.Report;
using SchedulerServer.AltechNuPay.Report.NAEDO.Structures;

using Atlas.Common.Extensions;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Common.Interface;


namespace SchedulerServer.AltechNuPay
{
  public class NAEDO
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
      var logins = new List<NAEDOLoginDTO>();

      using (var uow = new UnitOfWork())
      {
        var reporting = Atlas.Enumerators.Credentials.CredentialPurpose.Report.ToInt();
        var naedoLogins = new XPQuery<NAEDOLogin>(uow).Where(l => l.DeletedDT == null).ToList();
        naedoLogins.ForEach(r =>
        {
          if ((r.CredentialPurposeFlags & reporting) == reporting)
          {
            logins.Add(new NAEDOLoginDTO
            {
              NAEDOLoginId = r.NAEDOLoginId,
              MerchantId = r.MerchantId,
              Username = r.Username,
              Password = r.Password,
              DeletedDT = r.DeletedDT
            });
          }
        });
      }

      log.Information("[NAEDO] Found {Logins} NAEDO logins", logins.Count);

      foreach (Enumerators.NAEDO.ReportType reportType in Enum.GetValues(typeof(Enumerators.NAEDO.ReportType)))
      {
        log.Information("[NAEDO] Running report: {reportType}...", reportType);

        if (reportType == Enumerators.NAEDO.ReportType.FutureTransactions && futureReportRun)
        {
          log.Information("[NAEDO] Truncating NAEDOReportFuture...");
          var connStrCore = config.GetAtlasCoreConnectionString();// System.Configuration.ConfigurationManager.ConnectionStrings["atlas_core"].ConnectionString;
          using (var conn = new NpgsqlConnection(connStrCore))
          {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
              cmd.CommandType = CommandType.Text;
              cmd.CommandTimeout = (int)TimeSpan.FromMinutes(60).TotalSeconds;
              cmd.CommandText = "TRUNCATE TABLE \"NAEDOReportFuture\"";
              cmd.ExecuteNonQuery();

              cmd.CommandText = string.Format("DELETE FROM \"NAEDOReportBatch\" WHERE \"ReportType\" = {0}", reportType.ToInt());
              cmd.ExecuteNonQuery();
            }
          }
          log.Information("[NAEDO] Truncate NAEDOReportFuture complete");
        }
        else if ((reportType != Enumerators.NAEDO.ReportType.FutureTransactions && futureReportRun) ||
                 (reportType == Enumerators.NAEDO.ReportType.FutureTransactions && !futureReportRun))
        {
          continue;
        }

        // loop through every login to get reports and import
        foreach (var login in logins)
        {
          DateTime startDate;
          var endDate = DateTime.Now;
          var canQuery = true;

          if (reportType == Enumerators.NAEDO.ReportType.FutureTransactions)
          {
            startDate = DateTime.Today;
            endDate = startDate.AddDays(2);
          }
          else
          {
            using (var uow = new UnitOfWork())
            {
              // Get Last Report End Date Range for new report start date range
              startDate = (new XPQuery<NAEDOReportBatch>(uow)
                .Where(r => r.ReportType == reportType.ToInt() && r.IsSuccess && r.NAEDOLoginId == login.NAEDOLoginId)
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
            // call method to do report import 
            var errorMessage = string.Empty;
            var isSuccess = true;
            log.Information("[NAEDO] Running report for {reportType}, {Start:yyyy-MM-dd HH:mm:ss}-{End:yyyy-MM-dd HH:mm:ss}...", reportType, startDate, endDate);
            var report = Enquiry.GetReport(log, login, (int)reportType, reportType.ToStringEnum(),
              (int)Enumerators.NAEDO.ServiceType.Naedo, startDate, endDate, ref isSuccess, ref errorMessage);

            log.Information("[NAEDO] Adding report for {reportType}, {Start:yyyy-MM-dd HH:mm:ss}-{End:yyyy-MM-dd HH:mm:ss}...", reportType, startDate, endDate);
            AddReportToDB(log, login, reportType, report, startDate, endDate, isSuccess, errorMessage);
            log.Information("[NAEDO] Completed {reportType}, {Start:yyyy-MM-dd HH:mm:ss}-{End:yyyy-MM-dd HH:mm:ss}...", reportType, startDate, endDate);

            if (reportType == Enumerators.NAEDO.ReportType.FutureTransactions)
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
                {
                  endDate = startDate.AddDays(2);
                }
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

    #region Private methods

    //private XElement GetReport(NAEDOLoginDTO naedoLogin, Enumerators.NAEDO.ReportType reportType, DateTime startDate,
    //    DateTime endDate, ref bool isSuccess, ref string errorMessage, long tokenId = 0, int blockId = 0, int allowedRetryAttempts = 3)
    //{
    //    using (var client = new NAEDOTSPNupayReportService.wsNaedoSoapClient())
    //    {
    //        var retryAttempt = 1;
    //        var isError = true;
    //        XElement xmlReport = null;

    //        // Get Report from webservice and if error persists for <allowedRetryAttempts> retries, then throw exception
    //        while (isError && retryAttempt <= allowedRetryAttempts)
    //        {
    //            try
    //            {
    //                xmlReport = client.getReport(
    //                                   naedoLogin.MerchantId,
    //                                   naedoLogin.Username,
    //                                   naedoLogin.Password,
    //                                   (int)Enumerators.NAEDO.ServiceType.Naedo,
    //                                   (int)reportType,
    //                                   startDate,
    //                                   endDate,
    //                                   int.Parse(tokenId.ToString()), blockId);  // BlockId = 0 & TokenId = 0: Fresh Report
    //            }
    //            catch (Exception exception)
    //            {
    //                errorMessage = exception.Message;
    //                retryAttempt++;
    //                isError = true;
    //                continue;
    //            }

    //            if (FileStructureHelper.IsError(xmlReport, ref errorMessage))
    //            {
    //                retryAttempt++;
    //                isError = true;
    //            }
    //            else
    //            {
    //                isError = false;
    //            }
    //        }
    //        if (!isError)
    //        {
    //            isSuccess = true;
    //            if (!FileStructureHelper.IsEmpty(xmlReport.Element(EnumStringExtension.ToStringEnum(reportType))))
    //            {
    //                errorMessage = string.Empty;
    //                return xmlReport;
    //            }
    //            else
    //            {
    //                errorMessage = "Empty Result";
    //                return null;
    //            }
    //        }
    //        else
    //        {
    //            isSuccess = false;
    //            return null;
    //        }
    //    }
    //}

    private static void AddReportToDB(ILogging log, NAEDOLoginDTO naedoLogin, Enumerators.NAEDO.ReportType reportType, XElement xmlReport, DateTime startDate, DateTime endDate,
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
            var reportBatch = AddBatch(reportHeader, reportType, naedoLogin, unitOfWork, startDate, endDate, isSuccess, errorMessage);
            var repeatBlockNumber = reportHeader.NAEDOReportBatch.BlockNum;

            var transactions = xmlReport.Elements(reportType.ToStringEnum());

            for (var j = repeatBlockNumber; j > 0; j--)
            {
              if (j < repeatBlockNumber)
              {
                var report = Enquiry.GetReport(log, naedoLogin, (int)reportType, reportType.ToStringEnum(),
                  (int)Enumerators.NAEDO.ServiceType.Naedo, startDate, endDate, ref isSuccess, ref errorMessage,
                  reportHeader.NAEDOReportBatch.TokenNum, int.Parse(j.ToString()), allowedRetryAttempts: 20);
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
                  case Enumerators.NAEDO.ReportType.CancelledTransactions:
                    AddCancelledReport(new CancelledReport(tran), unitOfWork, reportBatch);
                    break;

                  case Enumerators.NAEDO.ReportType.DisputedTransactions:
                    AddDisputedReport(new DisputedReport(tran), unitOfWork, reportBatch);
                    break;

                  case Enumerators.NAEDO.ReportType.FailedTransactions:
                    AddFailedReport(new FailedReport(tran), unitOfWork, reportBatch);
                    break;

                  case Enumerators.NAEDO.ReportType.FutureTransactions:
                    AddFutureReport(new FutureReport(tran), unitOfWork, reportBatch);
                    break;

                  case Enumerators.NAEDO.ReportType.SuccessfulTransactions:
                    AddSuccessReport(new SuccessReport(tran), unitOfWork, reportBatch);
                    break;

                  case Enumerators.NAEDO.ReportType.TransactionsInProgress:
                    AddInProcessReport(new InProcessReport(tran), unitOfWork, reportBatch);
                    break;

                  case Enumerators.NAEDO.ReportType.TransactionsUploaded:
                    AddTransactionsUploadedReport(new TransactionsUploaded(tran), unitOfWork, reportBatch);
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
            AddBatch(null, reportType, naedoLogin, unitOfWork, startDate, endDate, isSuccess, errorMessage);
            unitOfWork.CommitChanges();
          }
        }
      }
      else
      {
        using (var unitOfWork = new UnitOfWork())
        {
          AddBatch(null, reportType, naedoLogin, unitOfWork, startDate, endDate, isSuccess, errorMessage);
          unitOfWork.CommitChanges();
        }
      }
    }


    private static NAEDOReportBatch AddBatch(ReportHeader reportHeader, Enumerators.NAEDO.ReportType reportType,
      NAEDOLoginDTO naedoLogin, UnitOfWork unitOfWork, DateTime startDate, DateTime endDate,
        bool isSuccess, string errorrMessage)
    {
      var reportBatch = new NAEDOReportBatch(unitOfWork)
      {
        NAEDOLoginId = naedoLogin.NAEDOLoginId,
        IsSuccess = isSuccess,
        ErrorMessage = errorrMessage.Length > 150 ? errorrMessage.Substring(0, 150) : errorrMessage,
        StartDT = startDate,
        EndDT = endDate,
        ReportType = (int)reportType
      };

      if (reportHeader != null)
      {
        reportBatch.BlockNum = reportHeader.NAEDOReportBatch.BlockNum;
        reportBatch.CreateDT = reportHeader.NAEDOReportBatch.CreateDT;
        reportBatch.ReportFromDT = reportHeader.NAEDOReportBatch.ReportFromDT;
        reportBatch.ReportToDT = reportHeader.NAEDOReportBatch.ReportToDT;
        reportBatch.MerchantNum = reportHeader.NAEDOReportBatch.MerchantNum;
        reportBatch.ReportGenerationDT = reportHeader.NAEDOReportBatch.ReportGenerationDT;
        reportBatch.ServiceType = reportHeader.NAEDOReportBatch.ServiceType;
        reportBatch.TokenNum = reportHeader.NAEDOReportBatch.TokenNum;
      }
      else
      {
        reportBatch.CreateDT = DateTime.Now;
      }

      return reportBatch;
    }


    private static void AddDisputedReport(DisputedReport report, UnitOfWork unitOfWork, NAEDOReportBatch reportBatch)
    {
      var naedoReport = unitOfWork.Query<NAEDOReportDisputed>().FirstOrDefault(n => n.ReportDisputed.TransactionId == report.naedoReport.ReportDisputed.TransactionId &&
        n.ReportDisputed.ActionDT == report.naedoReport.ReportDisputed.ActionDT &&
        n.ReportDisputed.ClientRef1 == report.naedoReport.ReportDisputed.ClientRef1 &&
        n.ReportDisputed.ClientRef2 == report.naedoReport.ReportDisputed.ClientRef2 &&
        n.ReportDisputed.ProcessMerchant == report.naedoReport.ReportDisputed.ProcessMerchant &&
        n.ReportDisputed.TransactionTypeId == report.naedoReport.ReportDisputed.TransactionTypeId);

      if (naedoReport == null)
      {
        naedoReport = unitOfWork.GetObjectsToSave().OfType<NAEDOReportDisputed>().FirstOrDefault(n => n.ReportDisputed.TransactionId == report.naedoReport.ReportDisputed.TransactionId &&
          n.ReportDisputed.ActionDT == report.naedoReport.ReportDisputed.ActionDT &&
          n.ReportDisputed.ClientRef1 == report.naedoReport.ReportDisputed.ClientRef1 &&
          n.ReportDisputed.ClientRef2 == report.naedoReport.ReportDisputed.ClientRef2 &&
          n.ReportDisputed.ProcessMerchant == report.naedoReport.ReportDisputed.ProcessMerchant &&
          n.ReportDisputed.TransactionTypeId == report.naedoReport.ReportDisputed.TransactionTypeId);
      }

      if (naedoReport == null)
      {
        naedoReport = new NAEDOReportDisputed(unitOfWork)
        {
          ReportDisputed = new NAEDOReportDisputed.ReportDisputedKey()
          {
            TransactionId = report.naedoReport.ReportDisputed.TransactionId,
            ActionDT = report.naedoReport.ReportDisputed.ActionDT,
            ClientRef1 = report.naedoReport.ReportDisputed.ClientRef1,
            ClientRef2 = report.naedoReport.ReportDisputed.ClientRef2,
            ProcessMerchant = report.naedoReport.ReportDisputed.ProcessMerchant,
            TransactionTypeId = report.naedoReport.ReportDisputed.TransactionTypeId
          }
        };
        naedoReport.AccountType = report.naedoReport.AccountType;
        naedoReport.Amount = report.naedoReport.Amount;
        naedoReport.CCardNum = report.naedoReport.CCardNum;
        naedoReport.HomingAccountName = report.naedoReport.HomingAccountName;
        naedoReport.HomingBranch = report.naedoReport.HomingBranch;
        naedoReport.NumInstallments = report.naedoReport.NumInstallments;
        naedoReport.QCode = report.naedoReport.QCode;
        naedoReport.RCode = report.naedoReport.RCode;
        naedoReport.ReplyDT = report.naedoReport.ReplyDT;

        naedoReport.ReportBatch = reportBatch;

        naedoReport.Save();
      }
    }


    private static void AddCancelledReport(CancelledReport report, UnitOfWork unitOfWork, NAEDOReportBatch reportBatch)
    {
      var naedoReport = unitOfWork.Query<NAEDOReportCancelled>().FirstOrDefault(n => n.ReportCancelled.TransactionId == report.naedoReport.ReportCancelled.TransactionId &&
        n.ReportCancelled.ActionDT == report.naedoReport.ReportCancelled.ActionDT &&
        n.ReportCancelled.ClientRef1 == report.naedoReport.ReportCancelled.ClientRef1 &&
        n.ReportCancelled.ClientRef2 == report.naedoReport.ReportCancelled.ClientRef2 &&
        n.ReportCancelled.ProcessMerchant == report.naedoReport.ReportCancelled.ProcessMerchant &&
        n.ReportCancelled.TransactionTypeId == report.naedoReport.ReportCancelled.TransactionTypeId);

      if (naedoReport == null)
      {
        naedoReport = unitOfWork.GetObjectsToSave().OfType<NAEDOReportCancelled>().FirstOrDefault(n => n.ReportCancelled.TransactionId == report.naedoReport.ReportCancelled.TransactionId &&
          n.ReportCancelled.ActionDT == report.naedoReport.ReportCancelled.ActionDT &&
          n.ReportCancelled.ClientRef1 == report.naedoReport.ReportCancelled.ClientRef1 &&
          n.ReportCancelled.ClientRef2 == report.naedoReport.ReportCancelled.ClientRef2 &&
          n.ReportCancelled.ProcessMerchant == report.naedoReport.ReportCancelled.ProcessMerchant &&
          n.ReportCancelled.TransactionTypeId == report.naedoReport.ReportCancelled.TransactionTypeId);
      }

      if (naedoReport == null)
      {
        naedoReport = new NAEDOReportCancelled(unitOfWork)
        {
          ReportCancelled = new NAEDOReportCancelled.ReportCancelledKey()
          {
            TransactionId = report.naedoReport.ReportCancelled.TransactionId,
            ActionDT = report.naedoReport.ReportCancelled.ActionDT,
            ClientRef1 = report.naedoReport.ReportCancelled.ClientRef1,
            ClientRef2 = report.naedoReport.ReportCancelled.ClientRef2,
            ProcessMerchant = report.naedoReport.ReportCancelled.ProcessMerchant,
            TransactionTypeId = report.naedoReport.ReportCancelled.TransactionTypeId
          }
        };
        naedoReport.Amount = report.naedoReport.Amount;
        naedoReport.CCardNum = report.naedoReport.CCardNum;
        naedoReport.HomingAccountName = report.naedoReport.HomingAccountName;
        naedoReport.ReplyDT = report.naedoReport.ReplyDT;

        naedoReport.ReportBatch = reportBatch;

        naedoReport.Save();
      }
    }


    private static void AddFailedReport(FailedReport report, UnitOfWork unitOfWork, NAEDOReportBatch reportBatch)
    {
      var naedoReport = unitOfWork.Query<NAEDOReportFailed>().FirstOrDefault(n => n.ReportFailed.TransactionId == report.naedoReport.ReportFailed.TransactionId &&
        n.ReportFailed.ActionDT == report.naedoReport.ReportFailed.ActionDT &&
        n.ReportFailed.ClientRef1 == report.naedoReport.ReportFailed.ClientRef1 &&
        n.ReportFailed.ClientRef2 == report.naedoReport.ReportFailed.ClientRef2 &&
        n.ReportFailed.ProcessMerchant == report.naedoReport.ReportFailed.ProcessMerchant &&
        n.ReportFailed.TransactionTypeId == report.naedoReport.ReportFailed.TransactionTypeId);

      if (naedoReport == null)
      {
        naedoReport = unitOfWork.GetObjectsToSave().OfType<NAEDOReportFailed>().FirstOrDefault(n => n.ReportFailed.TransactionId == report.naedoReport.ReportFailed.TransactionId &&
          n.ReportFailed.ActionDT == report.naedoReport.ReportFailed.ActionDT &&
          n.ReportFailed.ClientRef1 == report.naedoReport.ReportFailed.ClientRef1 &&
          n.ReportFailed.ClientRef2 == report.naedoReport.ReportFailed.ClientRef2 &&
          n.ReportFailed.ProcessMerchant == report.naedoReport.ReportFailed.ProcessMerchant &&
          n.ReportFailed.TransactionTypeId == report.naedoReport.ReportFailed.TransactionTypeId);
      }

      if (naedoReport == null)
      {
        naedoReport = new NAEDOReportFailed(unitOfWork)
        {
          ReportFailed = new NAEDOReportFailed.ReportFailedKey()
          {
            TransactionId = report.naedoReport.ReportFailed.TransactionId,
            ActionDT = report.naedoReport.ReportFailed.ActionDT,
            ClientRef1 = report.naedoReport.ReportFailed.ClientRef1,
            ClientRef2 = report.naedoReport.ReportFailed.ClientRef2,
            ProcessMerchant = report.naedoReport.ReportFailed.ProcessMerchant,
            TransactionTypeId = report.naedoReport.ReportFailed.TransactionTypeId
          }
        };
        naedoReport.AccountType = report.naedoReport.AccountType;
        naedoReport.Amount = report.naedoReport.Amount;
        naedoReport.CCardNum = report.naedoReport.CCardNum;
        naedoReport.HomingAccountName = report.naedoReport.HomingAccountName;
        naedoReport.HomingBranch = report.naedoReport.HomingBranch;
        naedoReport.NumInstallments = report.naedoReport.NumInstallments;
        naedoReport.QCode = report.naedoReport.QCode;
        naedoReport.RCode = report.naedoReport.RCode;
        naedoReport.ReplyDT = report.naedoReport.ReplyDT;

        naedoReport.ReportBatch = reportBatch;

        naedoReport.Save();
      }
    }


    private static void AddInProcessReport(InProcessReport report, UnitOfWork unitOfWork, NAEDOReportBatch reportBatch)
    {

      var naedoReport = unitOfWork.Query<NAEDOReportInProcess>().FirstOrDefault(n => n.ReportInProcess.TransactionId == report.naedoReport.ReportInProcess.TransactionId &&
        n.ReportInProcess.ActionDT == report.naedoReport.ReportInProcess.ActionDT &&
        n.ReportInProcess.ClientRef1 == report.naedoReport.ReportInProcess.ClientRef1 &&
        n.ReportInProcess.ClientRef2 == report.naedoReport.ReportInProcess.ClientRef2 &&
        n.ReportInProcess.ProcessMerchant == report.naedoReport.ReportInProcess.ProcessMerchant &&
        n.ReportInProcess.TransactionTypeId == report.naedoReport.ReportInProcess.TransactionTypeId);

      if (naedoReport == null)
      {
        naedoReport = unitOfWork.GetObjectsToSave().OfType<NAEDOReportInProcess>().FirstOrDefault(n => n.ReportInProcess.TransactionId == report.naedoReport.ReportInProcess.TransactionId &&
          n.ReportInProcess.ActionDT == report.naedoReport.ReportInProcess.ActionDT &&
          n.ReportInProcess.ClientRef1 == report.naedoReport.ReportInProcess.ClientRef1 &&
          n.ReportInProcess.ClientRef2 == report.naedoReport.ReportInProcess.ClientRef2 &&
          n.ReportInProcess.ProcessMerchant == report.naedoReport.ReportInProcess.ProcessMerchant &&
          n.ReportInProcess.TransactionTypeId == report.naedoReport.ReportInProcess.TransactionTypeId);
      }

      if (naedoReport == null)
      {
        naedoReport = new NAEDOReportInProcess(unitOfWork)
        {
          ReportInProcess = new NAEDOReportInProcess.ReportInProcessKey()
          {
            TransactionId = report.naedoReport.ReportInProcess.TransactionId,
            ActionDT = report.naedoReport.ReportInProcess.ActionDT,
            ClientRef1 = report.naedoReport.ReportInProcess.ClientRef1,
            ClientRef2 = report.naedoReport.ReportInProcess.ClientRef2,
            ProcessMerchant = report.naedoReport.ReportInProcess.ProcessMerchant,
            TransactionTypeId = report.naedoReport.ReportInProcess.TransactionTypeId
          }
        };
        naedoReport.NumInstallments = report.naedoReport.NumInstallments;
        naedoReport.HomingAccountName = report.naedoReport.HomingAccountName;
        naedoReport.Amount = report.naedoReport.Amount;
        naedoReport.CCardNum = report.naedoReport.CCardNum;
        naedoReport.Tracking = report.naedoReport.Tracking;
        naedoReport.TrackDT = report.naedoReport.TrackDT;
        naedoReport.InstStatus = report.naedoReport.InstStatus;

        naedoReport.ReportBatch = reportBatch;

        naedoReport.Save();
      }
    }


    private static void AddFutureReport(FutureReport report, UnitOfWork unitOfWork, NAEDOReportBatch reportBatch)
    {
      var naedoReport = unitOfWork.Query<NAEDOReportFuture>().FirstOrDefault(n =>
        n.ReportFuture.TransactionId == report.naedoReport.ReportFuture.TransactionId &&
        n.ReportFuture.ActionDT == report.naedoReport.ReportFuture.ActionDT &&
        n.ReportFuture.ClientRef1 == report.naedoReport.ReportFuture.ClientRef1 &&
        n.ReportFuture.ClientRef2 == report.naedoReport.ReportFuture.ClientRef2 &&
        n.ReportFuture.ProcessMerchant == report.naedoReport.ReportFuture.ProcessMerchant &&
        n.ReportFuture.TransactionTypeId == report.naedoReport.ReportFuture.TransactionTypeId);

      if (naedoReport == null)
      {
        naedoReport = unitOfWork.GetObjectsToSave().OfType<NAEDOReportFuture>().FirstOrDefault(n =>
          n.ReportFuture.TransactionId == report.naedoReport.ReportFuture.TransactionId &&
          n.ReportFuture.ActionDT == report.naedoReport.ReportFuture.ActionDT &&
          n.ReportFuture.ClientRef1 == report.naedoReport.ReportFuture.ClientRef1 &&
          n.ReportFuture.ClientRef2 == report.naedoReport.ReportFuture.ClientRef2 &&
          n.ReportFuture.ProcessMerchant == report.naedoReport.ReportFuture.ProcessMerchant &&
          n.ReportFuture.TransactionTypeId == report.naedoReport.ReportFuture.TransactionTypeId);
      }

      if (naedoReport == null)
      {
        naedoReport = new NAEDOReportFuture(unitOfWork)
        {
          ReportFuture = new NAEDOReportFuture.ReportFutureKey()
          {
            TransactionId = report.naedoReport.ReportFuture.TransactionId,
            ActionDT = report.naedoReport.ReportFuture.ActionDT,
            ClientRef1 = report.naedoReport.ReportFuture.ClientRef1,
            ClientRef2 = report.naedoReport.ReportFuture.ClientRef2,
            ProcessMerchant = report.naedoReport.ReportFuture.ProcessMerchant,
            TransactionTypeId = report.naedoReport.ReportFuture.TransactionTypeId
          }
        };
        naedoReport.NumInstallments = report.naedoReport.NumInstallments;
        naedoReport.HomingAccountName = report.naedoReport.HomingAccountName;
        naedoReport.Amount = report.naedoReport.Amount;
        naedoReport.CCardNum = report.naedoReport.CCardNum;
        naedoReport.Tracking = report.naedoReport.Tracking;

        naedoReport.ReportBatch = reportBatch;

        naedoReport.Save();
      }
    }


    private static void AddTransactionsUploadedReport(TransactionsUploaded report, UnitOfWork unitOfWork, NAEDOReportBatch reportBatch)
    {
      var naedoReport = unitOfWork.Query<NAEDOReportTransactionUploaded>().FirstOrDefault(n => n.ReportTransactionUploaded.TransactionId == report.naedoReport.ReportTransactionUploaded.TransactionId &&
        n.ReportTransactionUploaded.ActionDT == report.naedoReport.ReportTransactionUploaded.ActionDT &&
        n.ReportTransactionUploaded.ClientRef1 == report.naedoReport.ReportTransactionUploaded.ClientRef1 &&
        n.ReportTransactionUploaded.ClientRef2 == report.naedoReport.ReportTransactionUploaded.ClientRef2 &&
        n.ReportTransactionUploaded.ProcessMerchant == report.naedoReport.ReportTransactionUploaded.ProcessMerchant &&
        n.ReportTransactionUploaded.TransactionTypeId == report.naedoReport.ReportTransactionUploaded.TransactionTypeId);
      if (naedoReport == null)
      {
        naedoReport = unitOfWork.GetObjectsToSave().OfType<NAEDOReportTransactionUploaded>().FirstOrDefault(n => n.ReportTransactionUploaded.TransactionId == report.naedoReport.ReportTransactionUploaded.TransactionId &&
          n.ReportTransactionUploaded.ActionDT == report.naedoReport.ReportTransactionUploaded.ActionDT &&
          n.ReportTransactionUploaded.ClientRef1 == report.naedoReport.ReportTransactionUploaded.ClientRef1 &&
          n.ReportTransactionUploaded.ClientRef2 == report.naedoReport.ReportTransactionUploaded.ClientRef2 &&
          n.ReportTransactionUploaded.ProcessMerchant == report.naedoReport.ReportTransactionUploaded.ProcessMerchant &&
          n.ReportTransactionUploaded.TransactionTypeId == report.naedoReport.ReportTransactionUploaded.TransactionTypeId);
      }
      if (naedoReport == null)
      {
        naedoReport = new NAEDOReportTransactionUploaded(unitOfWork)
        {
          ReportTransactionUploaded = new NAEDOReportTransactionUploaded.ReportTransactionUploadedKey()
          {
            TransactionId = report.naedoReport.ReportTransactionUploaded.TransactionId,
            ActionDT = report.naedoReport.ReportTransactionUploaded.ActionDT,
            ClientRef1 = report.naedoReport.ReportTransactionUploaded.ClientRef1,
            ClientRef2 = report.naedoReport.ReportTransactionUploaded.ClientRef2,
            ProcessMerchant = report.naedoReport.ReportTransactionUploaded.ProcessMerchant,
            TransactionTypeId = report.naedoReport.ReportTransactionUploaded.TransactionTypeId
          }
        };
        naedoReport.Amount = report.naedoReport.Amount;
        naedoReport.CCardNum = report.naedoReport.CCardNum;
        naedoReport.HomingAccountName = report.naedoReport.HomingAccountName;
        naedoReport.HomingAccountNum = report.naedoReport.HomingAccountNum;
        naedoReport.HomingBranch = report.naedoReport.HomingBranch;
        naedoReport.QCode = report.naedoReport.QCode;
        naedoReport.RCode = report.naedoReport.RCode;
        naedoReport.ReplyDT = report.naedoReport.ReplyDT;

        naedoReport.ReportBatch = reportBatch;

        naedoReport.Save();
      }
    }


    private static void AddSuccessReport(SuccessReport report, UnitOfWork unitOfWork, NAEDOReportBatch reportBatch)
    {
      var naedoReport = unitOfWork.Query<NAEDOReportSuccess>().FirstOrDefault(n => n.ReportSuccess.TransactionId == report.naedoReport.ReportSuccess.TransactionId &&
        n.ReportSuccess.ActionDT == report.naedoReport.ReportSuccess.ActionDT &&
        n.ReportSuccess.ClientRef1 == report.naedoReport.ReportSuccess.ClientRef1 &&
        n.ReportSuccess.ClientRef2 == report.naedoReport.ReportSuccess.ClientRef2 &&
        n.ReportSuccess.ProcessMerchant == report.naedoReport.ReportSuccess.ProcessMerchant &&
        n.ReportSuccess.TransactionTypeId == report.naedoReport.ReportSuccess.TransactionTypeId);

      if (naedoReport == null)
      {
        naedoReport = unitOfWork.GetObjectsToSave().OfType<NAEDOReportSuccess>().FirstOrDefault(n => n.ReportSuccess.TransactionId == report.naedoReport.ReportSuccess.TransactionId &&
          n.ReportSuccess.ActionDT == report.naedoReport.ReportSuccess.ActionDT &&
          n.ReportSuccess.ClientRef1 == report.naedoReport.ReportSuccess.ClientRef1 &&
          n.ReportSuccess.ClientRef2 == report.naedoReport.ReportSuccess.ClientRef2 &&
          n.ReportSuccess.ProcessMerchant == report.naedoReport.ReportSuccess.ProcessMerchant &&
          n.ReportSuccess.TransactionTypeId == report.naedoReport.ReportSuccess.TransactionTypeId);
      }

      if (naedoReport == null)
      {
        naedoReport = new NAEDOReportSuccess(unitOfWork)
        {
          ReportSuccess = new NAEDOReportSuccess.ReportSuccessKey()
          {
            TransactionId = report.naedoReport.ReportSuccess.TransactionId,
            ActionDT = report.naedoReport.ReportSuccess.ActionDT,
            ClientRef1 = report.naedoReport.ReportSuccess.ClientRef1,
            ClientRef2 = report.naedoReport.ReportSuccess.ClientRef2,
            ProcessMerchant = report.naedoReport.ReportSuccess.ProcessMerchant,
            TransactionTypeId = report.naedoReport.ReportSuccess.TransactionTypeId
          }
        };
        naedoReport.Amount = report.naedoReport.Amount;
        naedoReport.CCardNum = report.naedoReport.CCardNum;
        naedoReport.HomingAccountName = report.naedoReport.HomingAccountName;
        naedoReport.NumInstallments = report.naedoReport.NumInstallments;
        naedoReport.ReplyDT = report.naedoReport.ReplyDT;

        naedoReport.ReportBatch = reportBatch;

        naedoReport.Save();
      }
    }

    #endregion
  }
}