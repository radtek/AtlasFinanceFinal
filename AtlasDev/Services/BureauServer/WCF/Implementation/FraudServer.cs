using System.ServiceModel;
using log4net;
using System;
using DevExpress.Xpo;
using Atlas.Domain.Model;
using System.Linq;
using Atlas.Common.Utils;
using Atlas.Enumerators;
using Atlas.Common.Extensions;
using Atlas.Common.ExceptionBase;
using Atlas.Domain.DTO;
using Atlas.Bureau.Server.Cache;
using Magnum;
using Atlas.RabbitMQ.Messages.Fraud;
using MassTransit;
using System.Diagnostics;
using System.Threading;
using Atlas.Bureau.Service.EasyNetQ;
using Ninject;
using Atlas.Bureau.Service.WCF.Interface;
using Atlas.ThirdParty.Fraud.TransUnion;

namespace Atlas.Bureau.Service.WCF.Implemenation
{
  /// <summary>
  /// Credit Server Implementation
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class FraudServer : IFraudServer
  {
    #region Private Members

    // Log4net
    private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      #endregion

    /// <summary>
    /// Performs a query against the fraud provider.
    /// </summary>
    /// <param name="doRequest"></param>
    /// <param name="errors"></param>
    /// <returns></returns>
		public FraudResult FraudEnquiry(Int64? accountId, string idNo, string firstName, string lastName, string addressLine1, string addressLine2,
														 string suburb, string city, string postalCode, string provinceCode, string homeTelCode, string homeTelNo, string workTelCode,
														 string workTelNo, string cellNo, string bankAccountNo, string bankName, string bankBranchCode, string employer)
		{
			bool timedOut = false;

      
			Guid guid = CombGuid.Generate();
			_log.Info(string.Format("FraudEnquiry() - CorrelationId {0} for IDNo {1}", guid, idNo));


			ServiceLocator.Get<AtlasServiceBus>().GetServiceBus().Publish(new FraudRequest(guid)
      {
        AccountId = (long)accountId,
        IDNumber = idNo,
        Firstname = firstName,
        Surname = lastName,
        AddressLine1 = addressLine1,
        AddressLine2 = addressLine2,
        Suburb = suburb,
        City = city,
        PostalCode = postalCode,
        Province = provinceCode,
        HomeTelCode = homeTelCode,
        HomeTelNo = homeTelNo,
        WorkTelCode = workTelCode,
        WorkTelNo = workTelNo,
        CellNo = cellNo,
        BankAccountNo = bankAccountNo,
        BankName = bankName,
        BankBranchCode = bankBranchCode,
        Employer = employer
      });
			
		

			Stopwatch timeout = new Stopwatch();
			timeout.Start();

			_log.Info(string.Format("FraudEnquiry() - Waiting for CorrelationId {0} for IDNo {1}", guid, idNo));

			while (ResponseMessageCache.CheckItem(guid) == null)
			{
				if (timeout.Elapsed.Minutes >= 2)
				{
					timedOut = true;
					timeout.Stop();
					break;
				}
				Thread.Sleep(30);
			}

			if (timedOut)
				return null;

			if (ResponseMessageCache.CheckItem(guid) != null)
			{
				var msg = ResponseMessageCache.CheckItem(guid);

				_log.Info(string.Format("FraudEnquiry() -  CorrelationId {0}.", guid));

        ResponseMessageCache.Remove(guid);

				return new FraudResult()
				{
					FraudScoreId = msg.FraudScoreId,
					Rating = msg.Rating,
					ReasonCodes = msg.ReasonCodes,
					EnquiryStatus = (EnquiryStatus)msg.EnquiryStatus,
					Status = msg.Status,
					StatusReason = msg.StatusReason,
					SubStatusReason = msg.SubStatusReason
				};
			}
			return null;
		}


    public FraudResult GetEnquiryForAccount(long accountId)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(p => p.AccountId == accountId);

        if (account == null)
          throw new Exception(string.Format("Account {0} was not found in the database.", accountId));

        var enquiry = new XPQuery<BUR_Enquiry>(uow).OrderByDescending(p => p.CreateDate).FirstOrDefault(o => o.Account.AccountId == account.AccountId
                                                            && o.IsSucess && o.EnquiryType == Risk.RiskEnquiryType.Fraud);

        if (enquiry == null)
        {
          _log.Warn(string.Format("No recent fraud enquiry found for account {0}.", account.AccountId));
          return null;
        }

        var fpm = new XPQuery<FPM_FraudScore>(uow).FirstOrDefault(f => f.Enquiry.EnquiryId == enquiry.EnquiryId);

        if (fpm == null)
        {
          _log.Warn(string.Format("No FPM record was found for enquiry {0}", enquiry.EnquiryId));
          return null;
        }

        var storage = new XPQuery<BUR_Storage>(uow).FirstOrDefault(s => s.Enquiry.EnquiryId == enquiry.EnquiryId);

        if (storage == null)
        {
          _log.Warn(string.Format("Storage for enquiry {0} was not found in the database.", enquiry.EnquiryId));
          return null;
        }

        return Xml.DeSerialize<FraudResult>(Compression.Decompress(storage.ResponseMessage)) as FraudResult;
      }
    }
  }
}