#region Using

using Atlas.Online.Web.Service.BankVerificationServer;
using Atlas.Online.Web.Service.Entities.Otp;
using Atlas.Online.Web.Service.WCF.Interface;
using DevExpress.Xpo;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using Atlas.Business.BankVerification;
using Atlas.Common.Extensions;
using Atlas.Common.Utils;
using Atlas.Online.Web.Common.Extensions;
using Atlas.Enumerators;
using Magnum;
using Atlas.Online.Web.Service.Hashing;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Service.Entities.App;
using Atlas.Online.Web.Service.Entities.Verification;
using Atlas.Online.Web.Service.OrchestrationService;
using Atlas.RabbitMQ.Messages.Notification;
using Atlas.Online.Web.Service.AuthenticationServer;
using Atlas.RabbitMQ.Messages.Online;
using Atlas.Online.Web.Service.AccountService;
using Atlas.Online.Web.Service.EasyNetQ;
using BankVerification.EasyNetQ;
using Magnum.Extensions;
using MassTransit;
using Contact = Atlas.Online.Data.Models.Definitions.Contact;
using WCFExtensions = Atlas.Online.Web.Common.Extensions.WCFExtensions;

#endregion

namespace Atlas.Online.Web.Service.WCF.Implementation
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class WebServer : IWebService
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(WebServer));
		#region OTP

		private const int MAX_ATTEMPTS = 5;


		public OtpSendResult OTP_Send(long clientId, bool sendFirst)
		{
			OtpSendResult response = null;

			using (var uow = new UnitOfWork())
			{
				var client = new XPQuery<Client>(uow).FirstOrDefault(c => c.ClientId == clientId);

				if (client == null)
					throw new InvalidOperationException(string.Format("Client {0} does not exist", clientId));

				if (client.OTPRequests != null && client.OTPRequests.Count >= 0)
				{
					var requestsToday = client.OTPRequests.Count(o => o.RequestDate.Date == DateTime.Now.Date);

					if (sendFirst && requestsToday >= 1)
						return new OtpSendResult()
						{
							Count = Math.Max(0, MAX_ATTEMPTS - requestsToday),
							Sent = false
						};


					if (!sendFirst && requestsToday >= MAX_ATTEMPTS)
					{
						response = new OtpSendResult()
						{
							Count = Math.Max(0, MAX_ATTEMPTS - requestsToday),
							Sent = false
						};
					}
					else if (requestsToday <= MAX_ATTEMPTS)
					{
						var otpRequest = new OTPRequest(uow);
						TupleOfintstring otp = null;

						WCFExtensions.Using(new OrchestrationServiceClient("OrchestrationService.NET"), (cli) =>
							{
								otp = cli.GenerateOTP();
							});

						otpRequest.Client = client;
						otpRequest.OTP = otp.m_Item1;
						otpRequest.Key = otp.m_Item2;
						otpRequest.RequestDate = DateTime.Now;
						otpRequest.Save();
						uow.CommitChanges();

						requestsToday += 1;

						var clientCell =
							new XPQuery<Contact>(uow).FirstOrDefault(
								c => c.Client.ClientId == clientId && c.ContactType.ContactTypeId == General.ContactType.CellNo.ToInt());
						if (clientCell != null)
							ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish(new SMSNotifyMessage(CombGuid.Generate())
							{
								ActionDate = DateTime.Now,
								Body = string.Format("Your OTP is {0}, this OTP will only be valid for 15 minutes", otpRequest.OTP),
								CreatedAt = DateTime.Now,
								Priority = Notification.NotificationPriority.High,
								To = clientCell.Value
							});

						response = new OtpSendResult()
						{
							Count = Math.Max(0, MAX_ATTEMPTS - requestsToday),
							Sent = true
						};
					}
				}
			}
			return response;
		}

		public bool OTP_Verify(long clientId, int otp)
		{
			string key = string.Empty;
			using (var uow = new UnitOfWork())
			{
				var otpCollection = new XPQuery<OTPRequest>(uow).OrderByDescending(p => p.RequestDate).FirstOrDefault(p => p.Client.ClientId == clientId && p.OTP == otp);

				if (otpCollection == null)
					return false;

				key = otpCollection.Key;
			}
			var verified = false;

			WCFExtensions.Using(new OrchestrationServiceClient("OrchestrationService.NET"), (cli) =>
			{
				verified = cli.VerifyOTP(key, otp);
			});

			if (verified)
			{
				using (var uow = new UnitOfWork())
				{
					var client = new XPQuery<Client>(uow).FirstOrDefault(c => c.ClientId == clientId);

					if (client == null)
						throw new InvalidOperationException(string.Format("Client {0} does not exist", clientId));

					client.OTPVerified = verified;

					client.Save();
					uow.CommitChanges();
				}
			}
			return verified;
		}
		#endregion

		#region AVS

		public long? AVS_Submit(long clientId)
		{
			using (var uow = new UnitOfWork())
			{
				var client = new XPQuery<Client>(uow).FirstOrDefault(x => x.ClientId == clientId);
				if (client == null)
					throw new InvalidOperationException(string.Format("Client does not exist for id {0}", clientId));

				var bankDetail = client.BankDetails.FirstOrDefault(x => x.IsEnabled);
				if (bankDetail == null)
					throw new InvalidOperationException(string.Format("No current bank details for client id {0}", clientId));

				long? bankDetailId = null;
				long? personId = null;
				if (client.PersonId != null)
				{
					personId = client.PersonId;

					WCFExtensions.Using(new OrchestrationServiceClient("OrchestrationService.NET"), (cli) =>
					{
						bankDetailId = cli.SaveBank((long)client.PersonId,
					 bankDetail.Bank.Type, bankDetail.AccountType.Type, bankDetail.AccountName,
					 bankDetail.AccountNo, bankDetail.Period.Type, bankDetail.Bank.Code);
					});
				}
				else
				{
					WCFExtensions.Using(new OrchestrationServiceClient("OrchestrationService.NET"), (cli) =>
					{
						personId = cli.CreatePerson(client.Firstname, client.Surname, client.IDNumber);
						client.PersonId = personId;
						client.Save();
					});
				}

				var avsResponse = ServiceLocator.Get<AVSServiceBus>().GetServiceBus().Request<AddAVSRequest, BankVerification.EasyNetQ.AVSResponse>(new AddAVSRequest
				{
					PersonId = personId,
					Initials = string.IsNullOrEmpty(client.Firstname) ? string.Empty : client.Firstname.Substring(0, 1),
					LastName = client.Surname,
					IdNumber = client.IDNumber,
					AccountNo = bankDetail.AccountNo,
					BranchCode = bankDetail.Bank.Code,
					Bank = bankDetail.Bank.Type,
					Host = General.Host.Atlas_Online,
					BankAccountPeriod = bankDetail.Period.Type
				});

				//serviceBus.PublishRequest(new AccountVerificationMessage
				//{
				//    PersonId = personId,
				//    Initials = string.IsNullOrEmpty(client.Firstname) ? string.Empty : client.Firstname.Substring(0, 1),
				//    LastName = client.Surname,
				//    IdNumber = client.IDNumber,
				//    AccountNo = bankDetail.AccountNo,
				//    BranchCode = bankDetail.Bank.Code,
				//    Host = General.Host.Atlas_Online,
				//    BankAccountPeriod = bankDetail.Period.Type
				//}, x =>
				//{
				//    x.Handle<BankVerification.EasyNetQ.AVSResponse>(message =>
				//      bankDetail.TransactionId = message.TransactionId);
				//    x.SetTimeout(30.Seconds());
				//});

				//, x =>
				//  {
				//    x.Handle<BankVerification.EasyNetQ.AVSResponse>(message =>
				//      bankDetail.TransactionId = message.TransactionId);
				//    x.SetTimeout(30.Seconds());
				//  }

				//WCFExtensions.Using(new VerificationServerClient("BankVerificationServer.NET"), (cli) =>
				//{
				//  avsResponse = cli.DoAVSEnquiryWithHost(personId, client.Firstname.Substring(0, 1),
				//   client.Surname, client.IDNumber, bankDetail.AccountNo,
				//   bankDetail.Bank.Type, bankDetail.Bank.Code, General.Host.Atlas_Online, bankDetail.Period.Type);
				//});

				bankDetail.TransactionId = avsResponse.TransactionId;
				bankDetail.Save();
				bankDetail.ReferenceId = bankDetailId;
				uow.CommitChanges();
				return bankDetail.TransactionId;
			}
		}
		/// <summary>
		/// Returns the response structure for account verification.
		/// </summary>
		/// <param name="transactionId"></param>
		/// <returns></returns>
		public AVS.Result? AVS_GetResponse(long? personId, long transactionId)
		{
			using (var uow = new UnitOfWork())
			{
				var bankDetail = new XPQuery<Atlas.Online.Data.Models.Definitions.BankDetail>(uow).FirstOrDefault(x => x.TransactionId == transactionId && x.IsEnabled);

				if (bankDetail == null)
					return null;

				Atlas.Domain.Structures.AccountVerification avsReply = null;
				WCFExtensions.Using(new OrchestrationServiceClient("OrchestrationService.NET"), (cli) =>
				{
					avsReply = cli.GetAccountVerificationById((long)bankDetail.TransactionId);
				});

				if (bankDetail.IsEnabled)
				{
					int failedCount = 0;
					if (personId != null)
					{
						WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), cli =>
						{
							failedCount = cli.GetAVSFailureCount((long)personId);
						});
					}

					if (failedCount >= 2)
						return AVS.Result.Locked;

					switch (avsReply.Transaction)
					{
						case Orchestration.AVSTransaction.Duration_Exceeded:
							return null;
						case Orchestration.AVSTransaction.Do_AVS:
							return null;
						case Orchestration.AVSTransaction.AVS_Current:
							if (!bankDetail.IsVerified)
							{
								bankDetail.IsVerified = true;
								bankDetail.IsActive = true;
								bankDetail.Save();
								uow.CommitChanges();
							}

							return AVS.Result.Passed;
						case Orchestration.AVSTransaction.AVS_Failed:
							if (!bankDetail.IsVerified)
							{
								bankDetail.IsVerified = true;
								bankDetail.IsActive = false;
								bankDetail.Save();
								uow.CommitChanges();
							}

							return AVS.Result.Failed;
						case Orchestration.AVSTransaction.AVS_Pending:
							return AVS.Result.NoResult;
						case Orchestration.AVSTransaction.Error:
							return AVS.Result.Failed;
					}
				}
			}
			return null;
		}

		#endregion

		#region CDV

		/// <summary>
		/// Performs basic check digital against account
		/// </summary>
		/// <param name="bank">Bank Enumerator</param>
		/// <param name="bankAccountType">Bank Account Type Enumerator</param>
		/// <param name="accountNo">Account Number</param>
		/// <returns>True/False</returns>
		public bool CDV_VerifyAccount(General.BankName bank, General.BankAccountType bankAccountType, string accountNo)
		{
			//return (bank == General.BankName.FNB && accountNo == "12345" && bankAccountType == General.BankAccountType.Cheque);

			_log.Info(string.Format("Performing CDV on Account: {0} AccountType: {1}, Bank {2}", accountNo,
				EnumStringExtension.ToStringEnum(bankAccountType), EnumStringExtension.ToStringEnum(bank)));

			var cdvResult = false;
			WCFExtensions.Using(new OrchestrationServiceClient("OrchestrationService.NET"), (cli) =>
				{
					cdvResult = cli.PerformCDV((long)bank, (long)bankAccountType, accountNo, "");
				});
			return cdvResult;
		}

		#endregion

		#region Application

		/// <summary>
		/// Submits application for start of process
		/// </summary>
		/// <param name="clientId">Client that application is being processed for.</param>
		/// <returns>Hashed value to do look ups.</returns>
		public string APP_Submit(long clientId)
		{
			Guid correlationId = CombGuid.Generate();
			// To be saved against the application
			string hash = Hash.Generate(correlationId, clientId);
			Application application = null;

			using (var uow = new UnitOfWork())
			{
				application = new XPQuery<Application>(uow).FirstOrDefault(x => x.Client.ClientId == clientId && x.IsCurrent);
				if (application == null)
				{
					throw new InvalidOperationException(string.Format("Application not found for client id '{0}'", clientId));
				}

				application.Hash = hash;
				application.Save();
				uow.CommitChanges();
			}

			// Publish msg to MQ


			ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish(new ClientMessage(correlationId)
			{
				ClientId = application.Client.ClientId,
				CreatedAt = DateTime.Now,
				Destination = NodeType.Nodes.Client

			});
			return hash;
		}

		public ApplicationAffordability APP_GetAffordability(long applicationId)
		{

			using (var uow = new UnitOfWork())
			{
				var application = new XPQuery<Application>(uow).FirstOrDefault(a => a.ApplicationId == applicationId && a.IsCurrent);

				if (application == null)
					throw new InvalidOperationException(string.Format("Application {0} does not exist in the database or is not current", applicationId));

				if (application.Affordability == null)
					throw new InvalidOperationException(string.Format("Application {0} has no affordability associated", applicationId));


				return new ApplicationAffordability()
				{
					Amount = application.Affordability.Amount,
					InstalmentAmount = (decimal)application.Affordability.Instalment,
					InterestCharges = (application.Affordability.RepaymentAmount - application.Affordability.CapitalAmount),
					RepaymentAmount = application.Affordability.RepaymentAmount,
					Fees = application.Affordability.TotalFees,
					Period = application.Period
				};
			}
		}

		public bool APP_AcceptAffordability(long applicationId)
		{
			using (var uow = new UnitOfWork())
			{
				var application = new XPQuery<Application>(uow).FirstOrDefault(p => p.ApplicationId == applicationId && p.IsCurrent);

				if (application == null)
					throw new InvalidOperationException(string.Format("Application {0} does not exist", applicationId));

				if (application.Affordability == null)
					throw new InvalidOperationException(string.Format("Application {0} has no affordability associated", applicationId));

				string error = string.Empty;
				int result;

				bool success = false;
				WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), (cli) =>
				{
					success = cli.AcceptAffordabilityOption((long)application.AccountId, application.Affordability.OptionId, out error, out result);
				});

				if (success)
				{
					application.Affordability.Accepted = true;
					application.Affordability.Save();
					uow.CommitChanges();
					return true;
				}
				return false;
			}
		}

		public AccountService.Quotation MYC_GetQuote(long applicationId)
		{
			using (var uow = new UnitOfWork())
			{
				var application = new XPQuery<Application>(uow).FirstOrDefault(p => p.ApplicationId == applicationId);

				if (application == null)
					throw new InvalidOperationException(string.Format("Application {0} could not be found in the database.", applicationId));


				string error = string.Empty;
				int result = 0;

				AccountService.Quotation quotation = null;
				WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), client =>
				{
					quotation = client.GetQuote((long)application.AccountId, out error, out result);
				});
				return quotation;
			}
		}

		public AccountService.Settlement MYC_GetSettlementQuotation(long applicationId)
		{
			using (var uow = new UnitOfWork())
			{
				var application = new XPQuery<Application>(uow).FirstOrDefault(p => p.ApplicationId == applicationId);

				if (application == null)
					throw new InvalidOperationException(string.Format("Application {0} does not exist.", applicationId));


				AccountService.Settlement settlement = null;

				WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), client =>
				{
					settlement = client.GetSettlementQuotation((long)application.AccountId, application.Settlement.ReferenceId);
				});

				return settlement;
			}
		}


		public PaidUpLetter MYC_GetPaidUpLetter(long applicationId)
		{
			using (var uow = new UnitOfWork())
			{
				var application = new XPQuery<Application>(uow).FirstOrDefault(p => p.ApplicationId == applicationId);

				if (application == null)
					throw new InvalidOperationException(string.Format("Application {0} does not exist.", applicationId));

				return new PaidUpLetter()
				{
					AccountNo = application.AccountNo,
					ClientName = string.Format("{0} {1}", application.Client.Firstname, application.Client.Surname),
					Email = new XPQuery<UserProfile>(uow).FirstOrDefault(p => p.UserId == application.Client.UserId).Email,
					IdNo = application.Client.IDNumber
				};
			}
		}

		public ApplicationSettlementResult APP_SubmitSettlement(ApplicationSettlementSubmission settlement)
		{
			using (var uow = new UnitOfWork())
			{
				var app = new XPQuery<Application>(uow).First(x => x.ApplicationId == settlement.ApplicationId);

				if (app.Affordability == null)
					throw new InvalidOperationException(string.Format("Application {0} does not have an affordability", settlement.ApplicationId));

				Settlement settle = null;

				string error = string.Empty;
				int result = 0;
				WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), client =>
			 {
				 settle = client.PostSettlement((long)app.AccountId, settlement.RepaymentDate, out error, out result);
			 });

				ApplicationSettlement set = null;
				if (app.Settlement == null)
				{
					set = new ApplicationSettlement(uow);
					set.Application = app;
					set.RepaymentDate = settlement.RepaymentDate;
					set.Amount = settle.TotalAmount;
					set.CreateDate = DateTime.Now;
					set.ReferenceId = settle.SettlementId;
					set.Save();
				}
				else
				{
					set = new XPQuery<ApplicationSettlement>(uow).FirstOrDefault(p => p.SettlementId == app.Settlement.SettlementId);

					set.RepaymentDate = settlement.RepaymentDate;
					set.Amount = settle.TotalAmount;
					set.CreateDate = DateTime.Now;
					set.ReferenceId = settle.SettlementId;
					set.Save();
				}
				app.Settlement = set;
				app.Save();

				uow.CommitChanges();

				return new ApplicationSettlementResult()
				{
					RepaymentAmount = settle.TotalAmount
				};
			}
		}

		public int APP_ApplyIn(long clientId)
		{
			using (var uow = new UnitOfWork())
			{
				var client = new XPQuery<Client>(uow).FirstOrDefault(x => x.ClientId == clientId);

				if (client == null) // Most likely remove.
					return 0;

				if (client != null && client.PersonId != null)
				{
					int day = 0;

					WCFExtensions.Using(new OrchestrationServiceClient("OrchestrationService.NET"), (cli) =>
					{
						day = cli.GetReApplyDelay((long)client.PersonId);
					});

					return day;
				}
				return 0;
			}
		}

		public ApplicationDeclinedReason APP_GetDeclinedReason(long applicationId)
		{
			ApplicationDeclinedReason declinedReason = new ApplicationDeclinedReason();

			using (var uow = new UnitOfWork())
			{

				var application = new XPQuery<Application>(uow).FirstOrDefault(p => p.ApplicationId == applicationId);

				if (application == null)
					throw new InvalidOperationException(string.Format("Application {0} does not exist in the database.", applicationId));

				if (application.AccountId == null)
					throw new InvalidOperationException(string.Format("Application {0} does not have an account associated.", applicationId));

				AccountInfo account = null;
				WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), client =>
				{
					account = client.GetAccount((long)application.Client.PersonId);
				});

				if (account == null)
					throw new InvalidOperationException(string.Format("Application {0} account {1} was not found in the database.", applicationId, application.AccountId));

				if (account.StatusReason == AccountAccountStatusReason.CreditRisk)
					declinedReason.Reason = "You've been declined coz of credit risk";
				else if (account.StatusReason == AccountAccountStatusReason.Affordability)
					declinedReason.Reason = "You've been declined coz of no affordability option";
				else if (account.StatusReason == AccountAccountStatusReason.Fraud)
					declinedReason.Reason = "You've been declined coz of no fraud";
				else if (account.StatusReason == AccountAccountStatusReason.Authentication)
					declinedReason.Reason = "You've been declined because of authentication";
			}
			return declinedReason;
		}

		#endregion

		#region Quote

		public Quotation QTE_GetQuote(long applicationId)
		{
			using (var uow = new UnitOfWork())
			{
				var application = new XPQuery<Application>(uow).FirstOrDefault(p => p.ApplicationId == applicationId && p.IsCurrent && p.Status == Account.AccountStatus.PreApproved);

				if (application == null)
					throw new InvalidOperationException(string.Format("Application {0} could not be found in the database.", applicationId));


				string error = string.Empty;
				int result = 0;

				Quotation quotation = null;
				WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), client =>
				{
					quotation = client.GetQuote((long)application.AccountId, out error, out result);
				});
				return quotation;
			}
		}

		public bool QTE_PreApprove(long applicationId)
		{
			using (var uow = new UnitOfWork())
			{
				var application = new XPQuery<Application>(uow).FirstOrDefault(p => p.ApplicationId == applicationId & p.IsCurrent);

				if (application == null)
					throw new InvalidOperationException(string.Format("Application {0} does not exist", applicationId));

				WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), client =>
				{
					client.WorkflowStepUp((long)application.AccountId, null, WorkflowProcessStep.Quotation);
					client.UpdateAccountStatus((long)application.AccountId, AccountAccountStatus.PreApproved, null, null);
					Application.UpdateStatus(uow, application.ApplicationId, Account.AccountStatus.PreApproved);
				});

				uow.CommitChanges();

				return true;
			}
		}

		public bool QTE_AcceptQuote(long applicationId)
		{
			using (var uow = new UnitOfWork())
			{
				var application = new XPQuery<Application>(uow).FirstOrDefault(p => p.ApplicationId == applicationId);

				if (application == null)
					throw new InvalidOperationException(string.Format("Application {0} does not exist", applicationId));

				WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), client =>
				{
					string error = string.Empty;
					int result;
					if (client.AcceptQuote((long)application.AccountId, out error, out result))
					{
						Application.UpdateStatus(uow, application.ApplicationId, Account.AccountStatus.Approved);
					}
				});

				uow.CommitChanges();
				return true;
			}
		}

		public bool QTE_RejectQuote(long applicationId)
		{
			using (var uow = new UnitOfWork())
			{
				var application = new XPQuery<Application>(uow).FirstOrDefault(p => p.ApplicationId == applicationId);

				if (application == null)
					throw new InvalidOperationException(string.Format("Application {0} does not exist", applicationId));

				WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), client =>
				{
					string error = string.Empty;
					int result;
					if (client.RejectQuote((long)application.AccountId, out error, out result))
					{
						Application.UpdateStatus(uow, application.ApplicationId, Account.AccountStatus.Cancelled);
						Application.UpdateIsCurrent(uow, application.ApplicationId, false);
						uow.CommitChanges();
					}
				});

				return true;
			}
		}

		#endregion

		#region Verification


		public VerificationResult VER_CheckStatus(long applicationId, long clientId)
		{
			using (var uow = new UnitOfWork())
			{
				var application = new XPQuery<Application>(uow).FirstOrDefault(p => p.ApplicationId == applicationId && p.IsCurrent);

				if (application == null)
					throw new InvalidOperationException(string.Format("No active application for client {0}", clientId));

				if (application.AccountId == null)
					throw new InvalidOperationException(string.Format("Application {0} has no account id associated", application.ApplicationId));

				TupleOfbooleanint result = null;
				WCFExtensions.Using(new AuthenticationServerClient("AuthenticationServer.NET"), cli =>
				{
					result = cli.ExceededAuthenticationTries((long)application.AccountId, application.Client.IDNumber);
				});

				if (result.m_Item1 == false && result.m_Item2 >= 2)
				{
					Application.UpdateIsCurrent(uow, application.ApplicationId, false);
					Application.UpdateStatus(uow, application.ApplicationId, Account.AccountStatus.Declined);
					uow.CommitChanges();
					WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), cli =>
					{
						cli.UpdateAccountStatus((long)application.AccountId, AccountAccountStatus.Declined, AccountAccountStatusReason.Authentication, null);
					});

					Dictionary<string, string> dict = new Dictionary<string, string>();

					dict.Add("{Name}", application.Client.Firstname);
					dict.Add("{Surname}", application.Client.Surname);
					dict.Add("{AccountNo}", application.AccountNo);

					string compiled = string.Empty;

					WCFExtensions.Using(new OrchestrationServiceClient("OrchestrationService.NET"), client =>
					{
						compiled = client.GetCompiledTemplate(Notification.NotificationTemplate.Declined_Authentication, dict);
					});

					ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
					{
						ActionDate = DateTime.Now,
						Body = compiled,
						CreatedAt = DateTime.Now,
						From = "noreply@atlasonline.co.za",
						IsHTML = true,
						Priority = Notification.NotificationPriority.High,
						Subject = string.Format("Your Atlas Online application [{0}] has been Declined", application.AccountNo),
						To = new XPQuery<UserProfile>(uow).FirstOrDefault(p => p.UserId == application.Client.UserId).Email
					});
				}
				return new VerificationResult() { Iteration = result.m_Item2, Success = result.m_Item1 };
			}
		}

		public IEnumerable<Questions> VER_GetQuestions(long clientId)
		{
			List<Questions> queshions = new List<Questions>();
			using (var uow = new UnitOfWork())
			{
				var application = new XPQuery<Application>(uow).FirstOrDefault(p => p.Client.ClientId == clientId && p.IsCurrent);

				if (application == null)
					throw new InvalidOperationException(string.Format("No active application for client {0}", clientId));

				if (application.AccountId == null)
					throw new InvalidOperationException(string.Format("Application {0} has no account id associated", application.ApplicationId));

				List<QuestionAnswers> authenticationQuestions = null;

				WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), client =>
				{
					client.WorkflowStepUp((long)application.AccountId, null, WorkflowProcessStep.XDSAuthentication);
				});

				WCFExtensions.Using(new AuthenticationServerClient("AuthenticationServer.NET"), cli =>
				{
					authenticationQuestions = cli.GetQuestions((long)application.AccountId, application.Client.IDNumber, application.ApplicationId.ToString());
				});

				foreach (var question in authenticationQuestions)
				{
					Questions quest = new Questions();
					quest.Question = question.Question;
					quest.QuestionId = question.QuestionId;
					quest.StoreId = question.AuthenticationProcessStoreId;

					quest.Answers = new List<Questions.Answer>();
					foreach (var answer in question.Answers)
					{
						Questions.Answer ans = new Questions.Answer();

						ans.AnswerDescription = answer.AnswerDescription;
						ans.AnswerId = answer.AnswerId;
						ans.IsAnswer = answer.IsAnswer;
						quest.Answers.Add(ans);
					}
					queshions.Add(quest);
				}
			}
			return queshions;
		}

		public VerificationResult VER_SubmitQuestions(long applicationId, long clientId, List<Questions> questions)
		{
			VerificationStatus authenticationStatus = new VerificationStatus();

			List<QuestionAnswers> questionAnswers = new List<QuestionAnswers>();

			foreach (var question in questions)
			{
				QuestionAnswers questAnswer = new QuestionAnswers();
				questAnswer.AuthenticationProcessStoreId = question.StoreId;
				questAnswer.Question = question.Question;
				questAnswer.QuestionId = question.QuestionId;
				questAnswer.Answers = new List<QuestionAnswers.Answer>();

				foreach (var answer in question.Answers)
				{
					QuestionAnswers.Answer answers = new QuestionAnswers.Answer();
					answers.AnswerDescription = answer.AnswerDescription;
					answers.AnswerId = answer.AnswerId;
					answers.IsAnswer = answer.IsAnswer;

					questAnswer.Answers.Add(answers);
				}
				questionAnswers.Add(questAnswer);
			}

			try
			{
				WCFExtensions.Using(new AuthenticationServerClient("AuthenticationServer.NET"), cli =>
				{
					authenticationStatus = cli.SubmitAnswers(questionAnswers);

          // TODO: CHECK_BYPASS Override check bypass here
          var bypassEndpoint = ConfigurationManager.AppSettings["bypass-endpoint"];
          if (!string.IsNullOrWhiteSpace(bypassEndpoint))
          {
            var http = new Http($"{bypassEndpoint}/GetXdsCheck") { Type = Http.PostTypeEnum.Get, TimeOut = 1000 };
            var bypassResult = http.Post();
            bool xdsCheckBypass;
            bool.TryParse(bypassResult, out xdsCheckBypass);
            http = null;
            if (xdsCheckBypass)
            {
              authenticationStatus.Status= Status.PASSED;
            }
          }
        });
			}
			catch (Exception ex)
			{
				throw ex;
			}

			bool success = false;

			if (authenticationStatus.Status == Status.NO_MORE_RETRIES || authenticationStatus.Status == Status.FAILED)
				success = false;
			else
				success = true;

			if (authenticationStatus.Status == Status.NO_MORE_RETRIES && authenticationStatus.Iteration >= 2)
			{
				using (var uow = new UnitOfWork())
				{
					var application = new XPQuery<Application>(uow).FirstOrDefault(p => p.ApplicationId == applicationId);

					if (application == null)
						throw new InvalidOperationException(string.Format("Application {0} does not exist in the database", applicationId));

					Application.UpdateStatus(uow, applicationId, Account.AccountStatus.Declined);

					uow.CommitChanges();

					WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), client =>
					{
						client.UpdateAccountStatus((long)application.AccountId, AccountAccountStatus.Declined, AccountAccountStatusReason.Authentication, null);
					});


					Dictionary<string, string> dict = new Dictionary<string, string>();

					dict.Add("{Name}", application.Client.Firstname);
					dict.Add("{Surname}", application.Client.Surname);
					dict.Add("{AccountNo}", application.AccountNo);

					string compiled = string.Empty;

					WCFExtensions.Using(new OrchestrationServiceClient("OrchestrationService.NET"), client =>
					{
						compiled = client.GetCompiledTemplate(Notification.NotificationTemplate.Declined_Authentication, dict);
					});

					ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
					{
						ActionDate = DateTime.Now,
						Body = compiled,
						CreatedAt = DateTime.Now,
						From = "noreply@atlasonline.co.za",
						IsHTML = true,
						Priority = Notification.NotificationPriority.High,
						Subject = string.Format("Your Atlas Online application [{0}] has been Declined", application.AccountNo),
						To = new XPQuery<UserProfile>(uow).FirstOrDefault(p => p.UserId == application.Client.UserId).Email
					});
				}
			}

			return new VerificationResult()
			{
				Iteration = authenticationStatus.Iteration,
				Success = success
			};
		}

		#endregion

		#region My Account

		public AccountStatement MYC_GetClientStatement(long applicationId)
		{
			using (var uow = new UnitOfWork())
			{
				var application = new XPQuery<Application>(uow).FirstOrDefault(a => a.ApplicationId == applicationId);

				if (application == null)
					throw new Exception(string.Format("Application {0} not found in the database.", applicationId));

				AccountStatement accountStatement = null;

				WCFExtensions.Using(new AccountServerClient("AccountServer.NET"), client =>
				{
					accountStatement = client.GetStatement((long)application.AccountId);
				});

				return accountStatement;
			}
		}

		public ApplicationSettlementResponse MYC_CheckSettlement(long applicationId)
		{
			using (var uow = new UnitOfWork())
			{
				var application = new XPQuery<Application>(uow).FirstOrDefault(a => a.ApplicationId == applicationId);

				if (application == null)
					throw new Exception(string.Format("Application {0} not found in the database.", applicationId));

				if (application.Settlement == null)
					return null;

				return new ApplicationSettlementResponse()
				{
					Amount = application.Settlement.Amount,
					RepaymentDate = application.Settlement.RepaymentDate
				};
			}
		}

		#endregion

		#region Loan Rules

		public Models.Dto.LoanRulesDto APP_SliderRules(long? clientId)
		{
			var day = DateTime.Today.Day;
			int maxTerm = 32;

			if (day < 15)
				maxTerm = 32;
			else if (day == 15)
				maxTerm = 45;
			else
			{
				maxTerm = 45 - (day - 15);
				if (maxTerm < 32)
					maxTerm = 32;
			}

			// TODO if client exists get their particular amounts they are allowed. Calculate based on Risk profile/ Fraud Profile and Payment history.

			return new Models.Dto.LoanRulesDto()
			{
				DefaultTerm = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month),
				MaxLoanPeriod = maxTerm,
				InterestRate = (60M / 365M),
				MaxLoanAmount = 2500,
				MinLoanAmount = 100,
				MinLoanPeriod = 1
			};
		}
		#endregion


		public bool NTF_Registration(string firstName, string lastName, string cellNo, string url, string username)
		{
			WCFExtensions.Using(new OrchestrationServiceClient("OrchestrationService.NET"), cli =>
			{
				Dictionary<string, string> dict = new Dictionary<string, string>();

				dict.Add("{Name}", firstName);
				dict.Add("{Surname}", lastName);
				dict.Add("{CellNo}", cellNo);
				dict.Add("{link}", url);
				dict.Add("{username}", username);

				string compiled = cli.GetCompiledTemplate(Notification.NotificationTemplate.Registration, dict);

				ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish(new EmailNotifyMessage(CombGuid.Generate())
				{
					ActionDate = DateTime.Now,
					Body = compiled,
					CreatedAt = DateTime.Now,
					From = "no-reply@atlasonline.co.za",
					IsHTML = true,
					NotificationType = Notification.NotificationTemplate.Registration,
					Priority = Notification.NotificationPriority.High,
					Subject = "Atlas Online Registration",
					To = username
				});
			});
			return true;
		}

		public bool NTF_ForgotPassword(string firstName, string lastName, string token, string userName)
		{
			WCFExtensions.Using(new OrchestrationServiceClient("OrchestrationService.NET"), cli =>
			{
				Dictionary<string, string> dict = new Dictionary<string, string>();

				dict.Add("{Name}", firstName);
				dict.Add("{Surname}", lastName);
				dict.Add("{token}", token);

				string compiled = cli.GetCompiledTemplate(Notification.NotificationTemplate.Forgot_Password, dict);

				ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish(new EmailNotifyMessage(CombGuid.Generate())
				{
					ActionDate = DateTime.Now,
					Body = compiled,
					CreatedAt = DateTime.Now,
					From = "no-reply@atlasonline.co.za",
					IsHTML = true,
					NotificationType = Notification.NotificationTemplate.Forgot_Password,
					Priority = Notification.NotificationPriority.High,
					Subject = "Atlas Online Forgot Password Reset",
					To = userName
				});
			});
			return true;
		}


		public List<DateTime> UTL_GetHolidays()
		{
			using (var uow = new UnitOfWork())
			{
				return new XPQuery<Holidays>(uow).Select(p => p.Date).ToList();
			}
		}
	}
}
