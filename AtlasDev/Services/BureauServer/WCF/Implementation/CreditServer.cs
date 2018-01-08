using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using log4net;
using Magnum;

using Atlas.Bureau.Server.Cache;
using Atlas.Bureau.Service.EasyNetQ;
using Atlas.RabbitMQ.Messages.Credit;
using Atlas.Bureau.Service.WCF.Interface;


namespace Atlas.Bureau.Service.WCF.Implemenation
{

  /// <summary>
  /// Credit Server Implementation
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class CreditServer : ICreditServer
  {
    #region Private Members

    // Log4net
    private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    #endregion

    /// <summary>
    /// Performs a query against the credit provider returning required data.
    /// </summary>
    /// <param name="doRequest"></param>
    /// <param name="errors"></param>   
    public void QueueEnquiry(string legacyBranchNum, string firstName, string surname, string IdNumber, string gender, DateTime dateOfBirth,
                             string addressLine1, string addressLine2, string addressLine3, string addressLine4, string postalCode,
                             string homeTelCode, string homeTelNo, string workTelCode, string workTelNo, string cellNo, bool isIDPassportNo,
                             bool isExistingClient, string requestUser, out decimal totalCPAAccount, out decimal totalNLRAccount, out string nlrEnquiryNo,
                             out string enquiryDate, out int score, out string riskType, out string[] reasons, out string file,
                             out List<Atlas.RabbitMQ.Messages.Credit.Product> products,
                             out List<Atlas.RabbitMQ.Messages.Credit.NLRCPAAccount> accounts,
                             out string[] errors)
    {
      errors = null;
      totalCPAAccount = 0.0M;
      totalNLRAccount = 0.0M;
      nlrEnquiryNo = string.Empty;
      enquiryDate = DateTime.Now.ToShortDateString();
      score = -1;
      riskType = null;
      reasons = null;
      file = null;
      products = null;
      accounts = null;

      Guid guid = CombGuid.Generate();
      bool timedOut = false;

      _log.Info(string.Format("QueueEnquiry() - CorrelationId {0} for IDNo {1}", guid, IdNumber));

      try
      {
				ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new CreditRequestLegacy(guid)
          {
            LegacyBranchNo = legacyBranchNum,
            AccountId = null,
            Firstname = firstName,
            Surname = surname,
            IDNumber = IdNumber,
            Gender = gender,
            DateOfBirth = dateOfBirth,
            AddressLine1 = addressLine1,
            AddressLine2 = addressLine2,
            Suburb = addressLine3,
            City = addressLine4,
            PostalCode = postalCode,
            HomeTelCode = homeTelCode,
            HomeTelNo = homeTelNo,
            WorkTelCode = workTelCode,
            WorkTelNo = workTelNo,
            CellNo = cellNo,
            IsIDPassportNo = isIDPassportNo,
            IsExistingClient = isExistingClient,
            RequestUser = 0,
            IsQueryOnly = null
          });

        var timeout = Stopwatch.StartNew();
        _log.Info(string.Format("QueueEnquiry() - Waiting for CorrelationId {0} for IDNo {1}", guid, IdNumber));
        while (ResponseMessageCache.CheckItem(guid) == null)
        {
          if (timeout.Elapsed.Minutes >= 2)
          {
            timedOut = true;
            timeout.Stop();
            break;
          }
          Thread.Sleep(50);
        }

        if (timedOut)
        {
          errors = new string[] { "Enquiry timeout" };
          totalCPAAccount = 0.0M;
          totalNLRAccount = 0.0M;
          nlrEnquiryNo = string.Empty;
          enquiryDate = DateTime.Now.ToShortDateString();
          score = -1;
          riskType = "Unknown";
          reasons = null;
          file = null;
          return;
        }

        if (ResponseMessageCache.CheckItem(guid) != null)
        {
          var msg = ResponseMessageCache.CheckItem(guid);

          _log.Info(string.Format("QueueEnquiry() - Response CorrelationId Score {0}, Decision {1}, Reasons {2} ",
            msg.Score, msg.Decision, msg.Reasons == null ? string.Empty : String.Join(",", msg.Reasons.ToArray())));
          totalCPAAccount = msg.TotalCPAAccount;
          totalNLRAccount = msg.TotalNLRAccount;
          riskType = msg.RiskType;
          score = Convert.ToInt32(msg.Score);
          file = msg.File;
          reasons = msg.Reasons == null ? new string[0] : msg.Reasons.ToArray();
          nlrEnquiryNo = msg.NLREnquiryNo;
          products = msg.Products;
          //accounts = msg.Accounts;
          ResponseMessageCache.Remove(guid);
        }
      }
      catch (Exception ex)
      {
        _log.Error(string.Format("{0} - {1}", ex.Message, ex.StackTrace));
      }
    }

    public CreditResponse Enquiry(long accountId, string firstName, string surname, string IdNumber, string gender, DateTime dateOfBirth, string addressLine1,
                             string addressLine2, string addressLine3, string addressLine4, string postalCode, string homeTelCode,
                             string homeTelNo, string workTelCode, string workTelNo, string cellNo, bool isIDPassportNo, bool isExistingClient,
                             string requestUser)
    {
      Guid guid = CombGuid.Generate();
      bool timedOut = false;

      _log.Info(string.Format("Enquiry() - CorrelationId {0} for IDNo {1}", guid, IdNumber));

			ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new CreditRequest(guid)
        {
          AccountId = accountId,
          Firstname = firstName,
          Surname = surname,
          IDNumber = IdNumber,
          Gender = gender,
          DateOfBirth = dateOfBirth,
          AddressLine1 = addressLine1,
          AddressLine2 = addressLine2,
          Suburb = addressLine3,
          City = addressLine4,
          PostalCode = postalCode,
          HomeTelCode = homeTelCode,
          HomeTelNo = homeTelNo,
          WorkTelCode = workTelCode,
          WorkTelNo = workTelNo,
          CellNo = cellNo,
          IsIDPassportNo = isIDPassportNo,
          IsExistingClient = isExistingClient,
          RequestUser = 0
        });

      var timeout = Stopwatch.StartNew();
      _log.Info(string.Format("Enquiry() - Waiting for CorrelationId {0} for IDNo {1}", guid, IdNumber));
      while (ResponseMessageCache.CheckItem(guid) == null)
      {
        if (timeout.Elapsed.Minutes >= 2)
        {
          timedOut = true;
          timeout.Stop();
          break;
        }
        Thread.Sleep(50);
      }

      if (timedOut)
        return null;

      if (ResponseMessageCache.CheckItem(guid) != null)
      {
        var msg = ResponseMessageCache.CheckItem(guid);

        if (msg.Decision != Enumerators.Account.AccountStatus.Technical_Error)
        {
          _log.Info(string.Format("Enquiry() - Response CorrelationId Score {0}, Decision {1}, Reasons {2} ", msg.Score, msg.Decision, String.Join(",", msg.Reasons.ToArray())));
        }
        else
        {
          _log.Info("Enquiry() - Response CorrelationId with Status Technical Error");
        }

        ResponseMessageCache.Remove(guid);

        return msg;
      }
      return null;
    }


    public string GetReport(long enquiryId)
    {
      Guid guid = CombGuid.Generate();
      bool timedOut = false;

      _log.Info(string.Format("GetReport() - CorrelationId {0} for EnquiryId {1}", guid, enquiryId));
			ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new ReportRequest(guid)
      {
        EnquiryId = enquiryId
      });

      var timeout = Stopwatch.StartNew();      
      _log.Info(string.Format("GetReport() - Waiting for CorrelationId {0} for EnquiryId {1}", guid, enquiryId));
      while (ResponseMessageCache.CheckItem(guid) == null)
      {
        if (timeout.Elapsed.Minutes >= 2)
        {
          timedOut = true;
          timeout.Stop();
          break;
        }
        Thread.Sleep(50);
      }

      if (timedOut)
        return null;

      if (ResponseMessageCache.CheckItem(guid) != null)
      {
        var msg = ResponseMessageCache.CheckItem(guid);
        ResponseMessageCache.Remove(guid);
        return msg.Report;
      }

      return string.Empty;
    }
  }
}