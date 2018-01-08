using Atlas.Common.ExceptionBase;
using Atlas.Common.Utils;
using Atlas.Domain.Model;
using Atlas.ThirdParty.XDSConnect;
using DevExpress.Xpo;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Atlas.Common.Extensions;
using Atlas.Enumerators;

namespace Atlas.ThirdParty.Xds
{
	public enum Status
	{
		Passed = 0,
		Failed = 1,
		NoMoreRetries = 2
	}

	public struct VerificationStatus
	{
		public Status Status { get; set; }
		public int Iteration { get; set; }
	}

	public sealed class AuthenticationQuestionImpl : IDisposable
	{
		private string _loginToken = string.Empty;
		private XDSConnectWSSoapClient _xdsConnect;
		bool _disposed;
		private const int DeclineAmnt = 2;
		private readonly Connect.Destination _destination;
		private static readonly ILog Log = LogManager.GetLogger(typeof(AuthenticationQuestionImpl));


		public AuthenticationQuestionImpl()
		{
			_destination = Connect.Destination.TEST;
			Login();
		}

		internal void Login()
		{
			_xdsConnect = new XDSConnectWSSoapClient("XDSConnectWSSoap", _destination.ToStringEnum());

			string userName;
			string password;

			using (var uow = new UnitOfWork())
			{
				var xdsAuthenticationSrvice =
					new XPQuery<BUR_Service>(uow).FirstOrDefault(s => s.Enabled && s.ServiceType == Risk.ServiceType.XDS_Authentication);

				if (xdsAuthenticationSrvice == null)
					throw new Exception("No service profile found for XDS authentication");

				userName = xdsAuthenticationSrvice.Username;
				password = xdsAuthenticationSrvice.Password;
			}
			//"NCRCP3994", "yrj6hg"
			//"atlas2", by3i6esu
			_loginToken = _xdsConnect.Login(userName, password); //.Login("", "by3i6esu");
		}

		public VerificationStatus SubmitAnswers(List<QuestionAnswers> questionAnswers)
		{
			var incomplete = new HashSet<long>();

			if (questionAnswers.Count == 0)
				throw new Exception("No question / answers found in collection.");

			AuthenticationProcess authenticationProcess;
			long? authenticationId = null;

			using (var uoW = new UnitOfWork())
			{
				Log.Info(
					$"Looking up authentication process store {questionAnswers[0].AuthenticationProcessStoreId} entry...");

				var authStore =
					new XPQuery<FPM_AuthenticationProcessStore>(uoW).FirstOrDefault(
						o => o.AuthenticationProcessStoreId == questionAnswers[0].AuthenticationProcessStoreId);

				if (authStore == null)
					throw new Exception(
						"Question auth store record is missing, this is required in order to determine question validity");

				Log.Info("Decompressing process document / Decrypting process document...");
				var questions = Cryptography.Decrypt(Compression.Decompress(authStore.ProcessDocument), "2384uy29834h29134",
					"23842y893dh19g3478123g4871234d123");

				authenticationProcess = Xml.DeSerialize<AuthenticationProcess>(questions) as AuthenticationProcess;

				Log.Info("Building authentication document challenge response...");
				foreach (var ques in questionAnswers)
				{
					if (authenticationId == null)
					{
						var fpmAuthenticationProcessStore =
							new XPQuery<FPM_AuthenticationProcessStore>(uoW).FirstOrDefault(
								o => o.AuthenticationProcessStoreId == ques.AuthenticationProcessStoreId);
						if (
							fpmAuthenticationProcessStore != null)
							authenticationId = fpmAuthenticationProcessStore.Authentication.AuthenticationId;
					}

					if (authenticationProcess != null)
					{
						var question =
							authenticationProcess.CurrentObjectState.AuthenticationDocument.Questions.FirstOrDefault(
								o => o.ProductAuthenticationQuestionID == ques.QuestionId);

						dynamic yesSelectedQuestion = ques.Answers.Where(o => o.IsAnswer).ToList();

						if (yesSelectedQuestion.Count > 1)
							throw new Exception("More than one answer has been selected, only one answer is allowed");

						yesSelectedQuestion = yesSelectedQuestion[0];

						var questionAnswer = question?.Answers.FirstOrDefault(o => o.AnswerID == yesSelectedQuestion.AnswerId);
						if (questionAnswer != null)
							questionAnswer.IsEnteredAnswerYN = yesSelectedQuestion.IsAnswer;
					}
				}
			}

			if (!_xdsConnect.IsTicketValid(_loginToken))
				Login();

			Log.Info("Submitting authentication challenge document to authentication provider...");
			var authProcessed = _xdsConnect.ConnectFraudProcess(_loginToken, AuthenticationProcessAction.Authenticate,
				authenticationProcess, "");
			var authenticationDocument = authProcessed.CurrentObjectState.AuthenticationDocument;

			var authenticated = false;

			Log.Info("Checking for response...");
			if (authenticationDocument.AuthenticatedPerc >= authenticationDocument.RequiredAuthenticatedPerc)
				authenticated = true;

			using (var uoW = new UnitOfWork())
			{
				Log.Info("Updating authentication record with challenge response...");
				var authentication =
					new XPQuery<FPM_Authentication>(uoW).FirstOrDefault(o => o.AuthenticationId == (long)authenticationId);
				if (authentication != null)
				{
					authentication.Authenticated = authenticated;
					authentication.Completed = true;
					authentication.AuthenticatedPercentage = authenticationDocument.AuthenticatedPerc;
					authentication.RequiredPercentage = authenticationDocument.RequiredAuthenticatedPerc;

					uoW.CommitChanges();

					var authenticationRequestsPerPerson =
						new XPQuery<FPM_Authentication>(uoW).Where(o => o.Person.PersonId == authentication.Person.PersonId)
							.OrderByDescending(p => p.CreateDate)
							.ToList();

					if (authentication.Person.GetBankDetails == null || authentication.Person.GetBankDetails.Count == 0)
						throw new Exception($"No bank details exist for person {authentication.Person.PersonId}");

					var bankDetail =
						authentication.Person.GetBankDetails.Select(b => b.BankDetail).OrderBy(c => c.CreatedDT).FirstOrDefault();

					if (bankDetail == null)
						throw new Exception($"No active bank exists for person {authentication.Person.PersonId}");

					var contact = authentication.Person.GetContacts.FirstOrDefault(o
						=> o.Contact.IsActive && o.Contact.ContactType.ContactTypeId == General.ContactType.CellNo.ToInt());

					if (contact == null)
						throw new Exception($"No cell no exists for person {authentication.Person.PersonId}");



					if (authenticationRequestsPerPerson.Count >= 2)
					{
						foreach (var item in authenticationRequestsPerPerson)
						{
							if (item.Completed && item.Authenticated && item.Enabled && item.BankDetail.DetailId == bankDetail.DetailId &&
									item.Contact.ContactId == contact.Contact.ContactId && incomplete.Count <= 1)
							{
								Log.Info("Person passed authentication");
								return new VerificationStatus() { Iteration = incomplete.Count, Status = Status.Passed };
							}
							if (!item.Authenticated)
							{
								incomplete.Add(item.AuthenticationId);
							}
						}
						if (incomplete.Count >= DeclineAmnt)
						{
							Log.Info($"Person {authentication.Person.PersonId} failed authentication");
							return new VerificationStatus() { Iteration = incomplete.Count, Status = Status.NoMoreRetries };
						}
					}
					else if (authenticationRequestsPerPerson.Count < 2)
					{
						if (
							authenticationRequestsPerPerson.Any(
								item =>
									item.Completed && item.Authenticated && item.Enabled && item.BankDetail.DetailId == bankDetail.DetailId &&
									item.Contact.ContactId == contact.Contact.ContactId && incomplete.Count <= 1))
						{
							Log.Info("Person passed authentication");
							return new VerificationStatus() { Iteration = incomplete.Count, Status = Status.Passed };
						}
					}
				}
			}
			return new VerificationStatus() { Iteration = incomplete.Count + 1, Status = Status.Failed };
		}

		public List<QuestionAnswers> GetQuestions(long accountId, string idNo, string refNo)
		{
			// Get the fraud profile of the subscriber (Atlas)
			var connectProfile = _xdsConnect.ConnectFraudGetProfile(_loginToken);

			var profileSerializer = new XmlSerializer(typeof(Profile));
			var encoding = new UTF8Encoding();

			var byteArray = encoding.GetBytes(connectProfile);

			using (var streamProfile = new MemoryStream(byteArray))
			{
				XmlReader reader = new XmlTextReader(streamProfile);
				var profile = (Profile)profileSerializer.Deserialize(reader);

				var branchCode = string.Empty;
				var purposeId = 0;

				if (profile.Items.Length > 0)
				{
					try
					{
						branchCode = ((ProfileBranches)profile.Items[0]).BranchCode;
					}
					catch (Exception exception)
					{
						Log.Warn($"Cannot deserialize xds response [{connectProfile}] - {exception.Message}");
					}
				}
				if (profile.Items.Length > 1)
				{
					try
					{
						purposeId = Convert.ToInt32(((ProfilePurposes)profile.Items[2]).PurposeID);
					}
					catch (Exception exception)
					{
						Log.Warn($"Cannot deserialize xds response [{connectProfile}] - {exception.Message}");
					}

				}

				// Get the consumer match details (Client)
				var idMatch = _xdsConnect.ConnectFraudConsumerMatch(_loginToken, AuthenticationSearchType.ConsumerIdentity,
					branchCode, purposeId, idNo, "", "", "", "", "", refNo, "");

				var consumerSerializer = new XmlSerializer(typeof(ListOfConsumers));
				encoding = new UTF8Encoding();
				byteArray = encoding.GetBytes(idMatch);
				List<QuestionAnswers> questionsAnswers;
				using (var streamConsumer = new MemoryStream(byteArray))
				{
					reader = new XmlTextReader(streamConsumer);
					var customerList = (ListOfConsumers)consumerSerializer.Deserialize(reader);
					// Get the questions for the consumer fraud match
					var authenticationProcess = _xdsConnect.ConnectFraudGetQuestions(_loginToken,
						(int)customerList.ConsumerDetails.EnquiryID, (int)customerList.ConsumerDetails.EnquiryResultID);

					// Get the document

					var questionDocument = authenticationProcess.CurrentObjectState.AuthenticationDocument.Questions;

					if (questionDocument == null)
						throw new Exception($"Question document for accountId {accountId}, IDNo: {idNo}, RefNo: {refNo}");

					long? authStoreId;

					using (var uoW = new UnitOfWork())
					{
						var account = new XPQuery<ACC_Account>(uoW).FirstOrDefault(o => o.AccountId == accountId);

						if (account == null)
							throw new RecordNotFoundException($"Account {accountId} record was not found in the database");

						var person = account.Person;

						if (person.GetBankDetails == null || person.GetBankDetails.Count == 0)
							throw new Exception($"No bank details exist for person {person.PersonId}");

						var bankDetail = person.GetBankDetails.Select(b => b.BankDetail).OrderBy(p => p.CreatedDT).FirstOrDefault();
						if (bankDetail == null)
							throw new Exception($"No active bank exists for person {person.PersonId}");


						var authentication = new FPM_Authentication(uoW)
						{
							Account = account,
							Authenticated = false,
							BankDetail = bankDetail
						};
						var perContact =
							person.GetContacts.FirstOrDefault(o => o.Contact.ContactType.ContactTypeId == General.ContactType.CellNo.ToInt());
						if (perContact != null)
						{
							authentication.Contact = perContact.Contact;
						}
						authentication.Completed = false;
						authentication.Person = account.Person;
						authentication.QuestionCount = questionDocument.Length.ToString();
						authentication.CreateDate = DateTime.Now;
						authentication.Enabled = true;
						authentication.Reference = authenticationProcess.CurrentObjectState.AuthenticationDocument.ReferenceNo;
						authentication.CreatedBy =
							new XPQuery<PER_Person>(uoW).FirstOrDefault(p => p.PersonId == General.Person.System.ToInt());

						uoW.CommitChanges();

						var authStore = new FPM_AuthenticationProcessStore(uoW) { Authentication = authentication };

						// Serialize object to string
						var questions = Xml.Serialize(authenticationProcess);
						// Encrypt text
						var encryptedQuestions = Cryptography.Encrypt(questions, "2384uy29834h29134", "23842y893dh19g3478123g4871234d123");
						authStore.ProcessDocument = Compression.Compress(encryptedQuestions);

						uoW.CommitChanges();

						authStoreId = authStore.AuthenticationProcessStoreId;
					}

					questionsAnswers = new List<QuestionAnswers>();

					foreach (var question in questionDocument)
					{
						var questionAnswerItem = new QuestionAnswers();

						if (questionAnswerItem.Answers == null)
							questionAnswerItem.Answers = new List<QuestionAnswers.Answer>();

						questionAnswerItem.QuestionId = question.ProductAuthenticationQuestionID;
						questionAnswerItem.Question = question.Question;
						questionAnswerItem.AuthenticationProcessStoreId = (long)authStoreId;

						foreach (var answer in question.Answers)
						{
							var answerItem = new QuestionAnswers.Answer
							{
								AnswerId = answer.AnswerID,
								AnswerDescription = answer.Answer
							};

							questionAnswerItem.Answers.Add(answerItem);
						}
						questionsAnswers.Add(questionAnswerItem);
					}
				}

				return questionsAnswers;
			}
		}

		#region IDisposeable

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~AuthenticationQuestionImpl()
		{
			Dispose(false);
		}

		void Dispose(bool disposing)
		{
			if (_disposed)
				return;
			if (disposing)
			{
				_xdsConnect = null;
				_loginToken = string.Empty;
			}
			_disposed = true;
		}

		#endregion

	}
}