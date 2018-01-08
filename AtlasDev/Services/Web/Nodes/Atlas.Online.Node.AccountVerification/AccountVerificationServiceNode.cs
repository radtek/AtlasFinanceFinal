using Atlas.Online.Node.Core;
using DevExpress.Xpo;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using Atlas.Common.Extensions;
using Atlas.Online.Data.Models.Definitions;
using System.Linq;
using Atlas.Enumerators;
using Atlas.Common.Utils;
using System.Configuration;
using System.Threading;
using Atlas.Domain.DTO;
using Atlas.Online.Node.AccountVerificationNode.EasyNetQ;
using Atlas.Online.Node.AccountVerificationNode.OrchestrationService;
using Atlas.RabbitMQ.Messages.Online;
using BankDetail = Atlas.Online.Data.Models.Definitions.BankDetail;

namespace Atlas.Online.Node.AccountVerificationNode
{
	public sealed class AccountVerificationServiceNode :
		AbstractService<AccountVerificationServiceNode, AccountVerificationMessage, SinkMessage>
	{
		private readonly IKernel _kernel;
		private readonly int _delay;
		private readonly int _avsDelay = 16;
		private Timer _checkAvsResponseTimer; // Used for checking the avs response for bank details

		public AccountVerificationServiceNode(ILogger ilogger, IKernel kernel)
			: base(ilogger)
		{
			_kernel = kernel;
			_delay = Convert.ToInt32(ConfigurationManager.AppSettings["AVS.Transaction.Delay"]);
		}

		public override void Start()
		{
			base.Start();

			var atlasOnlineServiceBus = _kernel.Get<AtlasOnlineServiceBus>();

			// Setup subscription handler for specific message type
			atlasOnlineServiceBus.GetServiceBus()
				.Subscribe<AccountVerificationMessage>("queue_AccountVerificationMessage", Handle);

			ServiceLocator.SetServiceLocator(_kernel);
			var exportPeriodString = ConfigurationManager.AppSettings["check-avs-reponse-timer"];
			int exportPeriod;
			if (!int.TryParse(exportPeriodString, out exportPeriod))
			{
				_logger.Warn("Cannot find configuration for avs response timer check (check-avs-reponse-timer)");
				exportPeriod = 60;
			}

			// start check avs response timer
			_checkAvsResponseTimer = new Timer(ChecAvsResponseTimerExecute, null, 0, exportPeriod*1000);
			// convert seconds to milliseconds
		}

		private void ChecAvsResponseTimerExecute(object state)
		{
			new OrchestrationServiceClient("OrchestrationService.NET").Using(service =>
			{
				using (var uow = new UnitOfWork())
				{
					// get bank all unverified bank details, to check the avs result
					var bankDetails =
						new XPQuery<BankDetail>(uow).Where(b => !b.IsVerified && b.TransactionId.HasValue && b.ReferenceId.HasValue && b.IsEnabled).ToList();
					foreach (var bankDetail in bankDetails)
					{
						if (bankDetail.Clients.Count > 1)
						{
							_logger.Fatal($"Bank Detail with Id {bankDetail.BankDetailId} linked to more than 1 client");
						}
						else
						{
							var client = bankDetail.Clients.FirstOrDefault();
							if (client == null)
							{
								bankDetail.IsVerified = true;
								bankDetail.IsEnabled = false;
								_logger.Warn($"Bank Detail with Id {bankDetail.BankDetailId} is not linked to anyone. Record disabled");
								bankDetail.Save();
								uow.CommitChanges();
							}
							else
							{
								CheckAvsResult(bankDetail, service, client, bankDetail.AccountNo, bankDetail.Bank.Type);

								uow.CommitChanges();
							}
						}
					}
				}
			});
		}

		// Handles subscribed message
		public override void Handle(AccountVerificationMessage message)
		{
			using (_kernel.BeginBlock())
			{
				new OrchestrationServiceClient("OrchestrationService.NET").Using(service =>
				{
					using (var uow = new UnitOfWork())
					{
						// Retrieve client from web DB
						var client = new XPQuery<Client>(uow).FirstOrDefault(c => c.ClientId == message.ClientId);
						if (client == null)
						{
							_logger.Fatal($"Handle() - Client [{message.ClientId}] does not exist");
							return;
						}
						_logger.Info($"Found client [{client.ClientId}]...");
						var application =
							new XPQuery<Application>(uow).FirstOrDefault(
								a => a.AccountId == message.AccountId && a.Client.ClientId == client.ClientId);

						if (application == null)
						{
							_logger.Fatal($":: Application Missing - Client [{client.ClientId}] AccountId [{message.AccountId}]");
							return;
						}

						if (application.BankDetail == null)
						{
							_logger.Fatal(
								$":: Client [{client.ClientId}] Missing bank details for Account [{application.ApplicationId}]");
							return;
						}

						if (application.BankDetail.IsActive &&
						    new DateDifference(application.BankDetail.CreateDate, DateTime.Now).Days >= _avsDelay)
						{
							_logger.Info($":: Client [{client.ClientId}] - Active bank details less than [{_avsDelay}] days old");
							return;
						}

						// Retrieve banks details from core DB
						var coreBankDetails = service.GetBankDetails(client.PersonId);

						// Get the details from core db based on details from web db
						var coreBankDetail =
							coreBankDetails.FirstOrDefault(
								o =>
									o.AccountNum == application.BankDetail.AccountNo &&
									o.Bank.Description == application.BankDetail.Bank.Description);

						if (coreBankDetail == null)
						{
							_logger.Info($":: Client [{client.ClientId}] No details on core...");
							_logger.Info($":: Client [{client.ClientId}] Bank details saved to core...");
							// Create core DB if there is none from the WEb DB
							if (service.SaveBank((long) client.PersonId, application.BankDetail.Bank.Type,
								application.BankDetail.AccountType.Type, application.BankDetail.AccountName,
								application.BankDetail.AccountNo, application.BankDetail.Period.Type, application.BankDetail.Bank.Code) !=
							    null)
							{
								_logger.Info($":: Performing AVS for client [{client.ClientId}]...");
								DoAvs(message, application.BankDetail, client, service, null);
							}
						}
						else
						{
							_logger.Info($":: Client [{client.ClientId}] has bank details on core...");
							// Already has a result, get the verification result from AVS
							var verificationResult = service.GetAccountVerification(client.IDNumber, coreBankDetail.AccountNum,
								coreBankDetail.Bank.Type, _delay);
							if (verificationResult.Transaction == Orchestration.AVSTransaction.Duration_Exceeded
							    || verificationResult.Transaction == Orchestration.AVSTransaction.Do_AVS
							    || verificationResult.Transaction == Orchestration.AVSTransaction.AVS_Failed
							    || verificationResult.Transaction == Orchestration.AVSTransaction.Error)
							{
								_logger.Info($":: Performing AVS for client [{client.ClientId}]...");
								DoAvs(message, application.BankDetail, client, service, coreBankDetail);
							}
							else if (verificationResult.Transaction == Orchestration.AVSTransaction.AVS_Current)
							{
								_logger.Info($":: AVS for client [{client.ClientId}] passed.");

								var bankDetail =
									new XPQuery<BankDetail>(uow).FirstOrDefault(p => p.BankDetailId == application.BankDetail.BankDetailId);

								if (bankDetail != null)
									UpdateBank(bankDetail, client.PersonId ?? 0, true, true, verificationResult.TransactionId, service);
							}
						}
						uow.CommitChanges();
					}
				});
			}
		}

		public void DoAvs(BankVerification.EasyNetQ.AddAVSRequest avs)
		{
			var avsServiceBus = ServiceLocator.Get<AVSServiceBus>().GetServiceBus();
			var avsResponseawait =
				avsServiceBus.RequestAsync<BankVerification.EasyNetQ.AddAVSRequest, BankVerification.EasyNetQ.AVSResponse>(avs)
					.GetAwaiter();
			var avsResponse = avsResponseawait.GetResult();
			if (avsResponse.Error)
			{
				throw new Exception("There was an unknown error in the AVS Engine.");
			}
		}

		public override void Publish(SinkMessage message)
		{
			throw new NotImplementedException();
		}

		public override long AddRouteHistory(dynamic message)
		{
			return base.AddRouteHistory((AccountVerificationMessage) message);
		}

		private Domain.Structures.AccountVerification GetAccountVerification(OrchestrationServiceClient service, Client client,
			string bankAccountNum, General.BankName bank)
		{
			return service.GetAccountVerification(client.IDNumber, bankAccountNum, bank, _delay);
		}

		private void DoAvs(dynamic message, BankDetail bankDetail, Client client,
			OrchestrationServiceClient service, BankDetailDTO coreBankDetail)
		{
			DoAvs(new BankVerification.EasyNetQ.AddAVSRequest
			{
				PersonId = client.PersonId,
				Initials = string.IsNullOrEmpty(client.Firstname) ? string.Empty : client.Firstname.Substring(0, 1),
				LastName = client.Surname,
				IdNumber = client.IDNumber,
				AccountNo = bankDetail.AccountNo,
				BranchCode = bankDetail.Bank.Code,
				Bank = bankDetail.Bank.Type,
				Host = General.Host.Atlas_Online,
				BankAccountPeriod = bankDetail.Period.Type,
				AccountId = message.AccountId,
				FireAndForget = false
			});

			// wait 20 seconds for avs response
			Thread.Sleep(new TimeSpan(0, 0, 20));

			var result = CheckAvsResult(bankDetail, service, client, coreBankDetail.AccountNum, coreBankDetail.Bank.Type);

			_logger.Info(
				$":: AVS Account [{message.AccountId}] , Status [{Enum.GetName(result.GetType(), result)}]");
		}

		private Orchestration.AVSTransaction CheckAvsResult(BankDetail bankDetail,
			OrchestrationServiceClient service, Client client,
			string bankAccountNum, General.BankName bank)
		{
			var result = GetAccountVerification(service, client, bankAccountNum, bank);

      // TODO: CHECK_BYPASS Override check bypass here
		  var bypassEndpoint = ConfigurationManager.AppSettings["bypass-endpoint"];
		  if (!string.IsNullOrWhiteSpace(bypassEndpoint))
		  {
		    var http = new Http($"{bypassEndpoint}/GetAvsCheck") {Type = Http.PostTypeEnum.Get, TimeOut = 1000};
		    var bypassResult = http.Post();
		    bool avsCheckBypass;
		    bool.TryParse(bypassResult, out avsCheckBypass);
		    http = null;
		    if (avsCheckBypass)
		    {
		      result.Transaction = Orchestration.AVSTransaction.AVS_Current;
		    }
		  }

		  if (result.Transaction == Orchestration.AVSTransaction.AVS_Failed)
				UpdateBank(bankDetail, client.PersonId ?? 0, false, true, result.TransactionId, service);
			else if (result.Transaction == Orchestration.AVSTransaction.AVS_Current)
				UpdateBank(bankDetail, client.PersonId ?? 0, true, true, result.TransactionId, service);
			else
				UpdateBank(bankDetail, client.PersonId ?? 0, false, false, result.TransactionId, service);
			return result.Transaction;
		}

		private void UpdateBank(BankDetail bankDetail, long personId, bool isActive, bool isVerified, long? transactionId, OrchestrationServiceClient service)
		{
			bankDetail.IsActive = isActive;
			bankDetail.IsVerified = isVerified;
			bankDetail.TransactionId = transactionId;
			bankDetail.Save();

			var retry = 0;
			var successfulySentToOrchestrationService = false;
			do
			{
				try
				{
					if (bankDetail.ReferenceId != null)
						service.UpdateBankDetailsAsync(personId, bankDetail.ReferenceId.Value, isActive).GetAwaiter().GetResult();

					successfulySentToOrchestrationService = true;
				}
				catch (Exception exception)
				{
					_logger.Error(
						$"Error calling OrchestrationService to update bank details with id {bankDetail.BankDetailId} to active: {exception.Message}");
					retry++;
					// wait 2 seconds, service might be busy
					Thread.Sleep(2000);
				}
			} while (retry < 3 && !successfulySentToOrchestrationService);
		}
	}
}
