using Atlas.Common.OTP;
using Atlas.Common.Utils;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Domain.Structures;
using Atlas.Enumerators;
using DevExpress.Xpo;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Atlas.Business.BankVerification;
using Atlas.Common.Extensions;
using Atlas.Orchestration.Server.WCF.Interface;
using Atlas.Orchestration.Server.Structures;

namespace Atlas.Orchestration.Server.WCF.Implementation
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class OrchestrationService : IOrchestrationService
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(OrchestrationService));

		#region Decline Day Lengths

		private const int AffordabilityDelay = 30;
		private const int CompanyPolicyDelay = 30;
		private const int CreditRiskDelay = 30;
		private const int FraudDelay = 90;
		private const int AuthenicationDelay = 30;

		#endregion

		#region Policy Constants

		private const int OverageLimit = 65;
		private const int UnderageLimit = 21;

		#endregion

		/// <summary>
		/// Returns person object with related populated fields via identification number.
		/// </summary>
		/// <param name="idNo">Identification Number</param>
		/// <returns>Person object with related populated fields.</returns>
		public PER_PersonDTO GetByIdNo(string idNo)
		{
			using (var uoW = new UnitOfWork())
			{
				Log.Info($"Person_GetByIdNo() - Started (IdNo:{idNo})");
				var person = new XPQuery<PER_Person>(uoW).FirstOrDefault(_ => _.IdNum == idNo);
				Log.Info($"Person_GetByIdNo() - Finished (IdNo:{idNo})");
				if (person == null)
					return null;
				var personDto = AutoMapper.Mapper.Map<PER_Person, PER_PersonDTO>(person);

				personDto.BankDetails =
						AutoMapper.Mapper.Map<List<BankDetailDTO>>(person.GetBankDetails.Select(b => b.BankDetail));
				personDto.AddressDetails =
						AutoMapper.Mapper.Map<List<AddressDTO>>(person.GetAddressDetails.Select(a => a.Address));
				personDto.Contacts = AutoMapper.Mapper.Map<List<ContactDTO>>(person.GetContacts.Select(c => c.Contact));
				if (person.Employer != null)
				{
					personDto.Employer.Contacts =
							AutoMapper.Mapper.Map<List<ContactDTO>>(person.Employer.GetContacts.Select(c => c.Contact));
					personDto.Employer.BankDetails =
							AutoMapper.Mapper.Map<List<BankDetailDTO>>(person.Employer.GetBankDetails.Select(b => b.BankDetail));
					personDto.Employer.Addresses =
							AutoMapper.Mapper.Map<List<AddressDTO>>(person.Employer.GetAddresses.Select(a => a.Address));
				}

				return personDto;
			}
		}

		/// <summary>
		/// Returns person object with related populated fields vi primarykey
		/// </summary>
		/// <param name="pk">Primary key of person object in data store</param>
		/// <returns>Person object with related populated fields.</returns>
		public PER_PersonDTO GetbyPk(long pk)
		{
			using (var uoW = new UnitOfWork())
			{
				Log.Info($"Person_GetbyPk() - Started (Pk:{pk})");
				var person = new XPQuery<PER_Person>(uoW).FirstOrDefault(_ => _.PersonId == pk);
				Log.Info($"Person_GetbyPk() - Finished (Pk:{pk})");
				if (person == null)
					return null;
				var personDto = AutoMapper.Mapper.Map<PER_Person, PER_PersonDTO>(person);

				personDto.BankDetails = AutoMapper.Mapper.Map<List<BankDetailDTO>>(person.GetBankDetails.Select(b => b.BankDetail));
				personDto.AddressDetails = AutoMapper.Mapper.Map<List<AddressDTO>>(person.GetAddressDetails.Select(a => a.Address));
				personDto.Contacts = AutoMapper.Mapper.Map<List<ContactDTO>>(person.GetContacts.Select(c => c.Contact));
				if (person.Employer != null)
				{
					personDto.Employer.Contacts = AutoMapper.Mapper.Map<List<ContactDTO>>(person.Employer.GetContacts.Select(c => c.Contact));
					personDto.Employer.BankDetails = AutoMapper.Mapper.Map<List<BankDetailDTO>>(person.Employer.GetBankDetails.Select(b => b.BankDetail));
					personDto.Employer.Addresses = AutoMapper.Mapper.Map<List<AddressDTO>>(person.Employer.GetAddresses.Select(a => a.Address));
				}

				return personDto;
			}
		}

		/// <summary>
		/// Persists the object to the data store.
		/// </summary>
		/// <returns>Primary key</returns>
		public Tuple<long, long> Save(Person client)
		{
			try
			{
				Log.Info($"Person_Save() - Started - Retrieving details if any (IdNo:{client.IdNo})");

				var saver = new Helper.Save();
				saver.Process(client);

				Log.Info($"Person_Save() - Finished - (IdNo:{client.IdNo})");

				if (saver.PersonId != null)
				{
					if (saver.BankDetailId != null)
					{
						return new Tuple<long, long>(saver.PersonId.Value, saver.BankDetailId.Value);
					}
					Log.Fatal($"Person_Save() - Bank Detail Id is null, Id No {client.IdNo}");
					throw new Exception($"Person_Save() - Bank Detail Id is null, Id No {client.IdNo}");
				}
				Log.Fatal($"Person_Save() - person Id is null, Id No {client.IdNo}");
				throw new Exception($"Person_Save() - person Id is null, Id No {client.IdNo}");
			}
			catch (Exception ex)
			{
				Log.Fatal(ex);
				throw;
			}
		}

		public BankDetailDTO GetBankDetail(long personId, string accountNo, bool isActive)
		{
			using (var uoW = new UnitOfWork())
			{
				var firstOrDefault = new XPQuery<PER_Person>(uoW)
					.FirstOrDefault(p => p.PersonId == personId);

				var perBankDetail =
					firstOrDefault?.GetBankDetails.FirstOrDefault(
						d => d.BankDetail.IsActive = isActive && d.BankDetail.AccountNum == accountNo);

				return perBankDetail == null ? null : AutoMapper.Mapper.Map<BNK_Detail, BankDetailDTO>(perBankDetail.BankDetail);
			}
		}

		public List<BankDetailDTO> GetBankDetails(long? personId)
		{
			using (var uoW = new UnitOfWork())
			{
				List<BNK_Detail> bankDetailCollection = null;
				var person = new XPQuery<PER_Person>(uoW).FirstOrDefault(p => p.PersonId == personId);

				if (person != null)
					bankDetailCollection = person.GetBankDetails.Select(b => b.BankDetail).ToList();

				return AutoMapper.Mapper.Map<List<BNK_Detail>, List<BankDetailDTO>>(bankDetailCollection);
			}
		}

		public AccountVerification GetAccountVerification(string idNo, string accountNo, General.BankName bank, int daysAgo)
		{
			using (var uow = new UnitOfWork())
			{
                //Edited By Prashant
                //var avs = new XPQuery<AVS_Transaction>(uow).Where(t => t.AccountNo == accountNo &&
                //t.Bank.Type == bank &&
                //t.IdNumber == idNo).OrderByDescending(t => t.CreateDate).FirstOrDefault();

                var avs = new XPQuery<AVS_Transaction>(uow).Where(t => t.AccountNo == accountNo &&
                t.Bank.BankId == (int)bank &&
                t.IdNumber == idNo).OrderByDescending(t => t.CreateDate).FirstOrDefault();

                if (avs == null)
					return new AccountVerification { Transaction = Enumerators.Orchestration.AVSTransaction.Do_AVS, TransactionId = null };

				if (avs.Result == null)
					return new AccountVerification { Transaction = Enumerators.Orchestration.AVSTransaction.AVS_Pending, TransactionId = avs.TransactionId };

				var duration = (DateTime.Now - avs.CreateDate).Days;

				if (duration > daysAgo)
					return new AccountVerification { Transaction = Enumerators.Orchestration.AVSTransaction.Duration_Exceeded, TransactionId = null };

				if (duration <= daysAgo)
				{
					if (avs.Result.Type == AVS.Result.Passed || avs.Result.Type == AVS.Result.PassedWithWarnings)
						return new AccountVerification { Transaction = Enumerators.Orchestration.AVSTransaction.AVS_Current, TransactionId = avs.TransactionId };
					else if (avs.Result.Type == AVS.Result.NoResult)
						return new AccountVerification { Transaction = Enumerators.Orchestration.AVSTransaction.Do_AVS, TransactionId = avs.TransactionId };
					else if (avs.Result.Type == AVS.Result.Failed)
						return new AccountVerification { Transaction = Enumerators.Orchestration.AVSTransaction.AVS_Failed, TransactionId = avs.TransactionId };
				}
			}
			return new AccountVerification { Transaction = Enumerators.Orchestration.AVSTransaction.Error, TransactionId = null };
		}

		public AccountVerification GetAccountVerificationById(long transactionId)
		{
			using (var uow = new UnitOfWork())
			{
				var avs = new XPQuery<AVS_Transaction>(uow).FirstOrDefault(t => t.TransactionId == transactionId);

				if (avs == null)
					return new AccountVerification { Transaction = Enumerators.Orchestration.AVSTransaction.Do_AVS, TransactionId = null };

				if (avs.Result == null)
					return new AccountVerification { Transaction = Enumerators.Orchestration.AVSTransaction.AVS_Pending, TransactionId = avs.TransactionId };

				if (avs.Result.Type == AVS.Result.Passed || avs.Result.Type == AVS.Result.PassedWithWarnings)
					return new AccountVerification { Transaction = Enumerators.Orchestration.AVSTransaction.AVS_Current, TransactionId = avs.TransactionId };
				else if (avs.Result.Type == AVS.Result.NoResult)
					return new AccountVerification { Transaction = Enumerators.Orchestration.AVSTransaction.Do_AVS, TransactionId = avs.TransactionId };
				else if (avs.Result.Type == AVS.Result.Failed)
					return new AccountVerification { Transaction = Enumerators.Orchestration.AVSTransaction.AVS_Failed, TransactionId = avs.TransactionId };
			}
			return new AccountVerification { Transaction = Enumerators.Orchestration.AVSTransaction.Error, TransactionId = null };
		}


		public BankDetailDTO GetBankDetailByDetailId(long personId, long bankDetailId)
		{
			using (var uow = new UnitOfWork())
			{
				var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);

				if (person?.GetBankDetails == null) return null;

				var bankDetail = person.GetBankDetails.Select(b => b.BankDetail).FirstOrDefault(p => p.DetailId == bankDetailId);
				return AutoMapper.Mapper.Map<BNK_Detail, BankDetailDTO>(bankDetail);
			}
		}

		public long? SaveBank(long personId, General.BankName bank, General.BankAccountType accountType, string accountName, string accountNo, General.BankPeriod accountPeriod, string branchCode)
		{
			long? bankDetailId = null;

			using (var uow = new UnitOfWork())
			{
				var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);

				if (person == null)
					throw new Exception($"Person {personId} does not exist");

				var bankDetail = person.GetBankDetails.Select(b => b.BankDetail).FirstOrDefault(
					p => p.Bank.Type == bank &&
							 p.AccountType.Type == accountType &&
							 p.AccountNum == accountNo &&
							 p.Period.Type == accountPeriod);

				BNK_Detail bnk = null;

				if (bankDetail == null)
				{
					bnk = new BNK_Detail(uow)
					{
						AccountName = accountName,
						AccountNum = accountNo,
						AccountType = new XPQuery<BNK_AccountType>(uow).FirstOrDefault(p => p.Type == accountType),
						Bank = new XPQuery<BNK_Bank>(uow).FirstOrDefault(b => b.Type == bank),
						Code = branchCode,
						CreatedDT = DateTime.Now,
						IsActive = false,
						Period = new XPQuery<BNK_Period>(uow).FirstOrDefault(p => p.Type == accountPeriod)
					};

					new PER_BankDetails(uow)
					{
						BankDetail = bnk,
						Person = person
					};
				}
				else
				{
					bankDetailId = bankDetail.DetailId;
				}

				person.Save();

				uow.CommitChanges();

				bankDetailId = bnk?.DetailId ?? bankDetailId;
			}
			return bankDetailId;
		}

		public void UpdateBankDetails(long personId, long detailId, bool isActive)
		{
			using (var uow = new UnitOfWork())
			{
				var bankDetail = new XPQuery<BNK_Detail>(uow).FirstOrDefault(p => p.DetailId == detailId);

				if (bankDetail == null)
					throw new Exception("Bank detail was not found in the database");

				bankDetail.IsActive = isActive;
				bankDetail.Save();

				uow.CommitChanges();
			}
		}

		/// <summary>
		/// Generate OTP that lasts 15 mins
		/// </summary>
		public Tuple<int, string> GenerateOTP()
		{
			var security = StringUtils.RandomBase32();
			return new Tuple<int, string>(new TOTP(security, 900).Now(), security);
		}

		/// <summary>
		/// Verify OTP that is generated via GenerateOTP
		/// </summary>
		public bool VerifyOTP(string security, int otp)
		{
			return new TOTP(security, 900).Verify(otp);
		}


		public string GetCompiledTemplate(Notification.NotificationTemplate notificationTemplate, Dictionary<string, string> searchReplace)
		{
			string template;
			using (var uow = new UnitOfWork())
			{
                //Edited By Prashant
                var coreTemplate = new XPQuery<NTF_Template>(uow).OrderBy(p => p.Version).LastOrDefault(p => p.TemplateType.TemplateTypeId == (int)Notification.NotificationTemplate.Core);
                //var coreTemplate = new XPQuery<NTF_Template>(uow).OrderBy(p => p.Version).LastOrDefault(p => p.TemplateType.Type == Notification.NotificationTemplate.Core);

                if (coreTemplate == null)
					throw new Exception("Core building template is missing");

				var templateContent = new XPQuery<NTF_Template>(uow).OrderBy(p => p.Version).LastOrDefault(p => p.TemplateType.TemplateTypeId == (int)notificationTemplate);

				if (templateContent == null)
					throw new Exception($"Template {notificationTemplate.ToStringEnum()} does not exist in the datastore");

				var body = templateContent.Template;

				foreach (var value in searchReplace)
				{
					body = body.Replace(value.Key, value.Value);
				}

				template = coreTemplate.Template;

				template = template.Replace("{CORE}", body);

			}
			return template;
		}

		/// <summary>
		/// Create basic person entry for linkage to AVS
		/// </summary>
		public long CreatePerson(string firstName, string lastName, string idNo)
		{
			using (var uow = new UnitOfWork())
			{
				var person = new PER_Person(uow)
				{
					IdNum = idNo,
					Firstname = firstName,
					Lastname = lastName,
					CreatedDT = DateTime.Now
				};
				person.Save();
				uow.CommitChanges();
				return person.PersonId;
			}
		}

		public bool PerformCDV(long bankId, long accountTypeId, string bankAccountNo, string branchCode)
		{
			return new AccountCDV().PerformCDV(bankId, accountTypeId, bankAccountNo, branchCode);
		}

		public List<Account.Policy> CompanyPolicies(long accountId)
		{
			using (var uow = new UnitOfWork())
			{
				var policies = new List<Account.Policy>();

				var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(p => p.AccountId == accountId);

				if (account == null)
				{
					Log.Fatal($"Account [{accountId}] was not found in the database.");
					throw new Exception($"Account [{accountId}] was not found in the database.");
				}

				#region Age Policy

				if (GetPersonAge(account.Person.DateOfBirth) >= OverageLimit)
				{
					policies.Add(Account.Policy.AgeMax65Years);

					new ACC_AccountPolicy(uow)
					{
						Account = account,
						Policy = new XPQuery<ACC_Policy>(uow).FirstOrDefault(p => p.PolicyId == Account.Policy.AgeMax65Years.ToInt() && p.Enabled),
						CreateDate = DateTime.Now
					};
				}
				else if (GetPersonAge(account.Person.DateOfBirth) <= UnderageLimit)
				{
					policies.Add(Account.Policy.AgeMinimum21OfYears);

					new ACC_AccountPolicy(uow)
					{
						Account = account,
						Policy = new XPQuery<ACC_Policy>(uow).FirstOrDefault(p => p.PolicyId == Account.Policy.AgeMinimum21OfYears.ToInt() && p.Enabled),
						CreateDate = DateTime.Now
					};
				}

				#endregion

				uow.CommitChanges();

				return policies;
			}
		}

		public List<ACC_PolicyDTO> AccountPolicies(long personId)
		{
			using (var uow = new UnitOfWork())
			{
				var policyCollection = new List<ACC_PolicyDTO>();
				// Check accounts        
				Log.Info($"Checking policies for person [{personId}]..");
                //Edited By Prashant
                //var accountCollection = new XPQuery<ACC_Account>(uow).Where(a => a.Person.PersonId == personId && a.Status.Type != Account.AccountStatus.Technical_Error).ToList();
                var accountCollection = new XPQuery<ACC_Account>(uow).Where(a => a.Person.PersonId == personId && a.Status.StatusId != (int)Account.AccountStatus.Technical_Error).ToList();

                Log.Info($"Found [{accountCollection.Count}] account(s) for person [{personId}]");

				if (accountCollection.Count > 1)
				{
					var lastAccount = accountCollection.OrderBy(o => o.CreateDate).FirstOrDefault();

					if (lastAccount != null)
					{
						Log.Info($"Last account for person [{personId}] has a status of [{lastAccount.Status.Type.ToStringEnum()}]");

						switch (lastAccount.Status.Type)
						{
							case Account.AccountStatus.Inactive:
								policyCollection.Add(new ACC_PolicyDTO { Description = Account.Policy.Account_Status_Inactive.ToStringEnum() });
								break;
							case Account.AccountStatus.Pending:
								policyCollection.Add(new ACC_PolicyDTO { Description = Account.Policy.Account_Status_Pending.ToStringEnum() });
								break;
							case Account.AccountStatus.Cancelled:
								return null;
							case Account.AccountStatus.Declined:
								// Determine decline reason
								int? total;

								if (lastAccount.StatusReason.Type == Account.AccountStatusReason.Fraud)
								{
									total = FraudDelay;
									if (lastAccount.StatusSubReason.Type == Account.AccountStatusSubReason.PersonalSuspect ||
											lastAccount.StatusSubReason.Type == Account.AccountStatusSubReason.PersonalConfirmed)
									{
										if (GetDay((int)total, lastAccount.StatusChangeDate) >= 0)
											policyCollection.Add(new ACC_PolicyDTO { Description = Account.Policy.Fraud_Alert_Declined_Previous_Account.ToStringEnum() });
									}
								}
								else if (lastAccount.StatusReason.Type == Account.AccountStatusReason.CreditRisk)
								{
									total = CreditRiskDelay;
									if (GetDay((int)total, lastAccount.StatusChangeDate) >= 0)
										policyCollection.Add(new ACC_PolicyDTO { Description = Account.Policy.Client_Credit_Risk.ToStringEnum() });
								}
								else if (lastAccount.StatusReason.Type == Account.AccountStatusReason.CompanyPolicy)
								{
									total = CompanyPolicyDelay;
									if (GetDay((int)total, lastAccount.StatusChangeDate) >= 0)
										policyCollection.Add(new ACC_PolicyDTO { Description = Account.Policy.Company_Policy.ToStringEnum() });
								}
								else if (lastAccount.StatusReason.Type == Account.AccountStatusReason.Affordability)
								{
									total = AffordabilityDelay;
									if (GetDay((int)total, lastAccount.StatusChangeDate) >= 0)
										policyCollection.Add(new ACC_PolicyDTO { Description = Account.Policy.Company_Policy.ToStringEnum() });
								}
								else if (lastAccount.StatusReason.Type == Account.AccountStatusReason.Authentication)
								{
									total = AuthenicationDelay;
									if (GetDay((int)total, lastAccount.StatusChangeDate) >= 0)
										policyCollection.Add(new ACC_PolicyDTO { Description = Account.Policy.Fraud_Alert_Declined_Previous_Account.ToStringEnum() });
								}
								break;
							case Account.AccountStatus.Review:
								policyCollection.Add(new ACC_PolicyDTO { Description = Account.Policy.Account_Status_Review.ToStringEnum() });
								break;
							case Account.AccountStatus.PreApproved:
								policyCollection.Add(new ACC_PolicyDTO { Description = Account.Policy.Account_Status_PreApproved.ToStringEnum() });
								break;
							case Account.AccountStatus.Approved:
								policyCollection.Add(new ACC_PolicyDTO { Description = Account.Policy.Account_Status_Approved.ToStringEnum() });
								break;
							case Account.AccountStatus.Open:
								policyCollection.Add(new ACC_PolicyDTO { Description = Account.Policy.Account_Status_Current.ToStringEnum() });
								break;
							case Account.AccountStatus.Legal:
								policyCollection.Add(new ACC_PolicyDTO { Description = Account.Policy.Handed_Over.ToStringEnum() });
								break;
							case Account.AccountStatus.WrittenOff:
								policyCollection.Add(new ACC_PolicyDTO { Description = Account.Policy.Bad_Debt.ToStringEnum() });
								break;
							default:
								return null;

						}
					}
				}
				return policyCollection;
			}
		}

		public int GetReApplyDelay(long personId)
		{
			using (var uow = new UnitOfWork())
			{
				var policyCollection = new List<ACC_PolicyDTO>();
				// Check accounts        
				Log.Info($"Checking policies for person [{personId}]..");

                //Edited by Prashant
               // var accountCollection = new XPQuery<ACC_Account>(uow).Where(a => a.Person.PersonId == personId && a.Status.Type != Account.AccountStatus.Technical_Error).ToList();
                var accountCollection = new XPQuery<ACC_Account>(uow).Where(a => a.Person.PersonId == personId && a.Status.StatusId != (int)Account.AccountStatus.Technical_Error).ToList();

                Log.Info($"Found [{accountCollection.Count}] account(s) for person [{personId}]");

				if (accountCollection.Count >= 1)
				{
					var lastAccount = accountCollection.OrderBy(o => o.CreateDate).FirstOrDefault();

					if (lastAccount != null)
					{
						Log.Info($"Last account for person [{personId}] has a status of [{lastAccount.Status.Type.ToStringEnum()}]");

						switch (lastAccount.Status.Type)
						{
							case Account.AccountStatus.Inactive:
								return 1;
							case Account.AccountStatus.Pending:
								return 1;
							case Account.AccountStatus.Cancelled:
								return 0;
							case Account.AccountStatus.Declined:
								// Determine decline reason
								int? total;

								if (lastAccount.StatusReason.Type == Account.AccountStatusReason.Fraud)
								{
									total = FraudDelay; // Hard coded for now.
									if (lastAccount.StatusSubReason.Type == Account.AccountStatusSubReason.PersonalSuspect ||
											lastAccount.StatusSubReason.Type == Account.AccountStatusSubReason.PersonalConfirmed)
									{
										return GetDay((int)total, lastAccount.StatusChangeDate);
									}
								}
								else if (lastAccount.StatusReason.Type == Account.AccountStatusReason.CreditRisk)
								{
									total = CreditRiskDelay;
									return GetDay((int)total, lastAccount.StatusChangeDate);
								}
								else if (lastAccount.StatusReason.Type == Account.AccountStatusReason.CompanyPolicy)
								{
									total = CompanyPolicyDelay;
									return GetDay((int)total, lastAccount.StatusChangeDate);
								}
								else if (lastAccount.StatusReason.Type == Account.AccountStatusReason.Affordability)
								{
									total = AffordabilityDelay;
									return GetDay((int)total, lastAccount.StatusChangeDate);
								}
								else if (lastAccount.StatusReason.Type == Account.AccountStatusReason.Authentication)
								{
									total = AuthenicationDelay;
									if (GetDay((int)total, lastAccount.StatusChangeDate) >= 0)
										policyCollection.Add(new ACC_PolicyDTO { Description = Account.Policy.Fraud_Alert_Declined_Previous_Account.ToStringEnum() });
								}
								break;
							case Account.AccountStatus.Review:
								return 1;
							case Account.AccountStatus.PreApproved:
								return 1;
							case Account.AccountStatus.Approved:
								return 1;
							case Account.AccountStatus.Open:
								return 1;
							case Account.AccountStatus.Legal:
								return 1;
							case Account.AccountStatus.WrittenOff:
								return 1;
							default:
								return 0;

						}
					}
				}
				if (policyCollection.Count > 0)
					return 1;
				return 0;
			}
		}

		private int GetPersonAge(DateTime birthDate)
		{
			var today = DateTime.Now;
			var age = today.Year - birthDate.Year;
			if (birthDate > today.AddYears(-age)) age--;

			return age;
		}

		private int GetDay(int total, DateTime statusChangeDate)
		{
			var ts = (DateTime.Now - statusChangeDate);
			return (total - Convert.ToInt32(Math.Floor(ts.TotalDays))) < 0 ? 0 : (total - Convert.ToInt32(Math.Floor(ts.TotalDays)));
		}

	}
}