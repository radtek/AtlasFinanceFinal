using Atlas.Common.Utils;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.RabbitMQ.Messages.Fraud;
using DevExpress.Xpo;
using log4net;
using System;
using System.Configuration;
using System.Linq;
using Atlas.Common.Extensions;
using Atlas.Common.ExceptionBase;
using Atlas.ThirdParty.Fraud.TransUnion;
using EasyNetQ;

namespace Atlas.ThirdParty.Fraud
{
	public sealed class Functions : IDisposable
	{
		#region Static Members

		private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		#endregion

		#region Private Memembers

		private readonly IBus _bus;
		const int Delay = 16;
		private string _subscriberCode = string.Empty;
		private string _securityCode = string.Empty;

		#endregion

		public Functions(IBus bus)
		{
			_bus = bus;
		}

		public Functions()
		{
		}

		#region Public


		public void Do(FraudRequest req)
		{
			using (var uoW = new UnitOfWork())
			{
				var activeRiskService = new XPQuery<BUR_Service>(uoW).FirstOrDefault(s => s.Enabled && s.ServiceType == Risk.ServiceType.Fraud);

				if (activeRiskService == null)
					throw new Exception("No service profile found for fraud");

				_subscriberCode = activeRiskService.Username;
				_securityCode = activeRiskService.Password;

				// Get all the fraud risk records for the client.
				var riskRec = (from o in new XPQuery<BUR_Enquiry>(uoW)
											 where o.IdentityNum == req.IDNumber
											 && o.EnquiryType == Risk.RiskEnquiryType.Fraud && o.PreviousEnquiry == null && o.IsSucess
											 orderby o.EnquiryDate descending
											 select new { o.EnquiryId, o.EnquiryDate }).FirstOrDefault();

				var activeRisk = AutoMapper.Mapper.Map<BUR_Service, BUR_ServiceDTO>(activeRiskService);

				// Check to make sure we got a recent rec.
				if (riskRec != null)
				{
					var fraudScore = new XPQuery<FPM_FraudScore>(uoW).FirstOrDefault(f => f.Enquiry.EnquiryId == riskRec.EnquiryId);

					if (fraudScore == null)
					{
						Log.Info($":: Enquiry {req.CorrelationId} with ID No {req.IDNumber} does not exist. Performing...");

						Respond(DoEnquiry(req, activeRisk, req.AccountId, req.IDNumber, req.Firstname, req.Surname, req.AddressLine1, req.AddressLine2,
												 req.Suburb, req.City, req.PostalCode, req.Province, req.HomeTelCode, req.HomeTelNo, req.WorkTelCode,
												 req.WorkTelNo, req.CellNo, req.BankAccountNo, req.BankName, req.BankBranchCode, req.Employer, null), req);
					}else if (fraudScore.CellNo == req.CellNo && fraudScore.BankAccountNo == req.BankAccountNo && fraudScore.IDNumber == req.IDNumber)
					{
						if ((DateTime.Now - riskRec.EnquiryDate).TotalDays > Delay)
						{
							Log.Info($":: Enquiry {req.CorrelationId} with ID No found {req.IDNumber} expired. Performing...");

							Respond(DoEnquiry(req, activeRisk, req.AccountId, req.IDNumber, req.Firstname, req.Surname, req.AddressLine1, req.AddressLine2,
																					 req.Suburb, req.City, req.PostalCode, req.Province, req.HomeTelCode, req.HomeTelNo, req.WorkTelCode,
																					 req.WorkTelNo, req.CellNo, req.BankAccountNo, req.BankName, req.BankBranchCode, req.Employer, null), req);
						}
						else
						{
							Log.Info(
								$":: Enquiry {req.CorrelationId} with ID No found {req.IDNumber} with day age of {Math.Round((DateTime.Now - riskRec.EnquiryDate).TotalDays, MidpointRounding.ToEven)}");

							var fraud = GetExistingEnquiry(riskRec.EnquiryId);
							Respond(fraud, req);

							Log.Info($":: Enquiry {riskRec.EnquiryId} Rating: {fraud.Rating}, Status: {fraud.Status.ToStringEnum()}");
						}
					}
					else
					{
						Log.Info(
							$":: Enquiry {req.CorrelationId} details have changed found for fraudscore {fraudScore.FraudScoreId} with ID No: {req.IDNumber}. Performing...");

						Respond(DoEnquiry(req, activeRisk, req.AccountId, req.IDNumber, req.Firstname, req.Surname, req.AddressLine1, req.AddressLine2,
														req.Suburb, req.City, req.PostalCode, req.Province, req.HomeTelCode, req.HomeTelNo, req.WorkTelCode,
														req.WorkTelNo, req.CellNo, req.BankAccountNo, req.BankName, req.BankBranchCode, req.Employer, null), req);
					}
				}
				else
				{
					Log.Info($":: Enquiry {req.CorrelationId} with ID No {req.IDNumber}. Performing...");

					Respond(DoEnquiry(req, activeRisk, req.AccountId, req.IDNumber, req.Firstname, req.Surname, req.AddressLine1, req.AddressLine2,
													 req.Suburb, req.City, req.PostalCode, req.Province, req.HomeTelCode, req.HomeTelNo, req.WorkTelCode,
													 req.WorkTelNo, req.CellNo, req.BankAccountNo, req.BankName, req.BankBranchCode, req.Employer, null), req);
				}
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Publishes response message to queue
		/// </summary>
		private void Respond(FraudResult result, FraudRequest request)
		{
			_bus.Publish(new FraudResponse(request.CorrelationId)
			{
				FraudScoreId = result.FraudScoreId,
				EnquiryStatus = (int)result.EnquiryStatus,
				Rating = result.Rating,
				ReasonCodes = result.ReasonCodes,
				Status = result.Status,
				StatusReason = result.StatusReason,
				SubStatusReason = result.SubStatusReason
			});

			//if (EnquiryCache.CheckItem(resp.CorrelationId) != null)
			//  EnquiryCache.Update(req.CorrelationId, req, resp);
		}

		#endregion

		#region Internal

		internal FraudResult GetExistingEnquiry(long riskEnquiryId)
		{
			using (var uow = new UnitOfWork())
			{
				var riskRec = new XPQuery<BUR_Enquiry>(uow).FirstOrDefault(e => e.EnquiryId == riskEnquiryId);

				if (riskRec == null)
				{
					Log.Fatal(
						$":: Risk enquiry record {riskEnquiryId} is missing, but is used during a process. Possible data loss has occurred");
					return new FraudResult() { EnquiryStatus = EnquiryStatus.Error };
				}

				var storage = new XPQuery<BUR_Storage>(uow).FirstOrDefault(s => s.Enquiry.EnquiryId == riskEnquiryId);
				if (storage == null)
				{
					Log.Fatal(
						$":: Risk enquiry storage record {riskEnquiryId} is missing, but is used during a process. Possible data loss has occurred");
					return new FraudResult() { EnquiryStatus = EnquiryStatus.Error };
				}
				return Xml.DeSerialize<FraudResult>(Compression.Decompress(storage.ResponseMessage)) as FraudResult;
			}
		}


		internal FraudResult DoEnquiry(FraudRequest req, BUR_ServiceDTO riskService, long? accountId, string idNo, string firstName, string lastName, string addressLine1, string addressLine2,
																	 string suburb, string city, string postalCode, string provinceCode, string homeTelCode, string homeTelNo, string workTelCode,
																	 string workTelNo, string cellNo, string bankAccountNo, string bankName, string bankBranchCode, string employer, long? previousEnquiryId)
		{
			using (var uow = new UnitOfWork())
			{
				var riskEnquiry = new BUR_Enquiry(uow);
				riskEnquiry.CreateDate = DateTime.Now;
				riskEnquiry.EnquiryType = Risk.RiskEnquiryType.Fraud;
				riskEnquiry.LastName = lastName;
				riskEnquiry.FirstName = firstName;
				riskEnquiry.IdentityNum = idNo;
				riskEnquiry.EnquiryDate = DateTime.Now;
				riskEnquiry.Service = new XPQuery<BUR_Service>(uow).FirstOrDefault(s => s.ServiceId == riskService.ServiceId);
				riskEnquiry.Account = accountId == null ? null : new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
				riskEnquiry.CorrelationId = req.CorrelationId;
				riskEnquiry.CreatedUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == req.RequestUser);

				uow.CommitChanges();

				//riskEnquiryId = riskEnquiry.RiskEnquiryId;

				var bureauEnquiry42 = BuildFraudPayLoad(idNo, firstName, lastName, addressLine1, addressLine2, suburb,
															 city, postalCode, provinceCode, homeTelCode, homeTelNo, workTelCode,
															 workTelNo, cellNo, bankAccountNo, bankName, bankBranchCode, employer);

				var fraudImpl = new FraudEnquiryImpl();
				Log.Info($":: DoEnquiry {req.CorrelationId} ID No {idNo}...");
				return CheckResult(fraudImpl.FraudCheck(bureauEnquiry42, riskEnquiry.EnquiryId, null), riskEnquiry.EnquiryId);
			}
		}

		internal BureauEnquiry42 BuildFraudPayLoad(string idno, string firstName, string lastName, string addressLine1, string addressLine2,
														 string suburb, string city, string postalCode, string provinceCode, string homeTelCode, string homeTelNo, string workTelCode,
														 string workTelNo, string cellNo, string bankAccountNo, string bankName, string bankBranchCode, string employer)
		{
			var bureauEnquiry42 = new BureauEnquiry42();
			bureauEnquiry42.SubscriberCode = _subscriberCode;// "73376"; // Production
			bureauEnquiry42.SecurityCode = _securityCode; //"ALF33"; // Production
			bureauEnquiry42.IdentityNo1 = idno;
			bureauEnquiry42.Forename1 = firstName;
			bureauEnquiry42.Surname = lastName;
			bureauEnquiry42.AddressLine1 = addressLine1;
			bureauEnquiry42.AddressLine2 = addressLine2;
			bureauEnquiry42.Suburb = suburb;
			bureauEnquiry42.City = city;
			bureauEnquiry42.PostalCode = postalCode;
			bureauEnquiry42.ProvinceCode = provinceCode;
			bureauEnquiry42.HomeTelCode = homeTelCode;
			bureauEnquiry42.HomeTelNo = homeTelNo;
			bureauEnquiry42.WorkTelCode = workTelCode;
			bureauEnquiry42.WorkTelNo = workTelNo;
			bureauEnquiry42.CellNo = cellNo;
			bureauEnquiry42.BankAccountNumber = bankAccountNo;
			bureauEnquiry42.BankName = bankName;
			bureauEnquiry42.BankBranchCode = bankBranchCode;
			bureauEnquiry42.Employer = employer;

			bureauEnquiry42.EnquirerContactName = "Atlas Finance Pty (Ltd)";
			bureauEnquiry42.EnquirerContactPhoneNo = "087 701 8665";

			return bureauEnquiry42;
		}

		internal FraudResult CheckResult(FraudResult result, long riskEnquiryId)
		{
			using (var uow = new UnitOfWork())
			{

				var riskRec = new XPQuery<BUR_Enquiry>(uow).FirstOrDefault(e => e.EnquiryId == riskEnquiryId);

				if (riskRec == null)
				{
					Log.Fatal($":: CheckResult {riskEnquiryId} is missing. It was used during the process. Possible data corruption.");
					return new FraudResult() { EnquiryStatus = EnquiryStatus.Error };
				}

				if (result.EnquiryStatus == EnquiryStatus.Error)
				{
					Log.Info($":: CheckResult {riskEnquiryId} Technical Error has occurred.");
					result.Status = Account.AccountStatus.Technical_Error;
					result.StatusReason = Account.AccountStatusReason.TechnicalError;
				}

				if (result.EnquiryStatus == EnquiryStatus.Success)
				{
					if (result.Rating == 4)
					{
						Log.Info($":: CheckResult {riskEnquiryId} Review, Rating [{result.Rating}]");
						result.Status = Account.AccountStatus.Review;
						result.StatusReason = Account.AccountStatusReason.Fraud;
					}
					else if (result.Rating > 4)
					{
						Log.Info($":: CheckResult {riskEnquiryId} Declined, Rating [{result.Rating}].");
						result.Status = Account.AccountStatus.Declined;
						result.StatusReason = Account.AccountStatusReason.Fraud;
					}
					else if (result.Rating < 3)
					{
						Log.Info($":: CheckResult {riskEnquiryId} Pending, Rating [{result.Rating}]");
						result.Status = Account.AccountStatus.Inactive;
					}

					var foundCode = false;

					foreach (var item in result.ReasonCodes)
					{
						if (ReasonCodeCache.GetCache().ContainsKey(item))
						{
							if (ReasonCodeCache.GetCache()[item] == FPM.DecisionOutCome.Reject)
							{
								foundCode = true;
								Log.Info(
									$":: CheckResult {riskEnquiryId} Declined, {Account.AccountStatusSubReason.PersonalConfirmed.ToStringEnum()} ");
								result.SubStatusReason = Account.AccountStatusSubReason.PersonalConfirmed;
								result.Status = Account.AccountStatus.Declined;
							}
							else if (ReasonCodeCache.GetCache()[item] == FPM.DecisionOutCome.Review)
							{
								foundCode = true;
								Log.Info($":: CheckResult {riskEnquiryId} Review.");
								result.SubStatusReason = Account.AccountStatusSubReason.PersonalSuspect;
								result.Status = Account.AccountStatus.Review;
							}
						}
					}

					if (!foundCode && result.Rating >= 4)
						result.SubStatusReason = Account.AccountStatusSubReason.PersonalSuspect;

					if (ConfigurationManager.AppSettings["override-success"] == "true")
					{
						Log.Warn($":: --- OVERRIDE!!!! --- CheckResult {riskEnquiryId} Pending, Rating [{result.Rating}]");
						result.Status = Account.AccountStatus.Inactive;
					}

					var riskStorage = new XPQuery<BUR_Storage>(uow).FirstOrDefault(s => s.Enquiry.EnquiryId == riskRec.EnquiryId);

					if (riskStorage != null)
					{
						riskStorage.ResponseMessage = Compression.Compress(Xml.Serialize(result));
					}

					var fpm = new XPQuery<FPM_FraudScore>(uow).FirstOrDefault(f => f.FraudScoreId == result.FraudScoreId);

					if (fpm == null)
						throw new RecordNotFoundException(
							$":: FraudScore Record {result.FraudScoreId} is missing possible data corruption");

					fpm.Passed = result.Status == Account.AccountStatus.Inactive;

					uow.CommitChanges();
				}
			}
			return result;
		}


		#endregion

		public void Dispose()
		{

		}
	}
}
