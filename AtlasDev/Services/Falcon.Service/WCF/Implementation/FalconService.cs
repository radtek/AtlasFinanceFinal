using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using Magnum;
using Serilog;
using AutoMapper;
using DevExpress.Xpo;

using Falcon.Common.Structures;
using Falcon.Common.Structures.Avs;
using Falcon.Common.Structures.Branch;
using Falcon.Common.Structures.Report.Ass;
using Falcon.Common.Structures.Report.General;
using Falcon.Service.Business;
using Falcon.Service.Business.Reporting;
using Falcon.Service.Core;
using Falcon.Service.CreditService;
using Falcon.Service.Helpers;
using Falcon.Service.OrchestrationService;
using Falcon.Service.WCF.Interface;
using Account = Atlas.Enumerators.Account;
using GeneralLedger = Atlas.Enumerators.GeneralLedger;
using Host = Falcon.Common.Structures.Host;
using Log = Serilog.Log;
using Notification = Falcon.Common.Structures.Notification;
using PER_PersonDTO = Atlas.Domain.DTO.PER_PersonDTO;
using PublicHoliday = Falcon.Common.Structures.PublicHoliday;
using Region = Falcon.Common.Structures.Region;
using Atlas.Common.Extensions;
using Atlas.Domain.Model;
using Atlas.Enumerators;
//using Atlas.LoanEngine.Account;
using Atlas.RabbitMQ.Messages.Notification;
using Atlas.RabbitMQ.Messages.Online;


namespace Falcon.Service.WCF.Implementation
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public sealed class FalconService : IFalconService
  {
    private static readonly ILogger _log = Log.Logger.ForContext<FalconService>();

    #region Database Association Operations

    /// <summary>
    /// Locate the person on the core database.
    /// </summary>
    public Person Operations_LocatebyId(string idNo)
    {
      // Validate Id No before wasting resources.
      using (var uow = new UnitOfWork())
      {
        var locatedPerson = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.IdNum == idNo.Trim());

        if (locatedPerson == null)
          return null;

        var userInWebRoles = new XPQuery<WEB_UserRole>(uow).Where(p => p.Person.PersonId == locatedPerson.PersonId).ToList();

        var pD = new Person() { IdNum = idNo, PersonId = locatedPerson.PersonId };
        //pD.BranchId = locatedPerson.Branch != null ? locatedPerson.Branch.BranchId : (long?)null;

        if (userInWebRoles.Count > 0)
          pD.Roles = new List<WebRole>();

        foreach (var role in userInWebRoles)
        {
          pD.Roles.Add(new WebRole { WebRoleId = role.WebRole.WebRoleId, WebRoleDescription = role.WebRole.Role });
        }
        return pD;
      }
    }

    /// <summary>
    /// Locate the person on the core database.
    /// </summary>
    public Person Operations_LocateByPersonId(long personId)
    {
      // Validate Id No before wasting resources.
      using (var uow = new UnitOfWork())
      {
        var locatedPerson = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);

        if (locatedPerson == null)
          return null;

        var userInWebRoles = new XPQuery<WEB_UserRole>(uow).Where(p => p.Person.PersonId == locatedPerson.PersonId).ToList();

        var pD = new Person() { IdNum = locatedPerson.IdNum, PersonId = locatedPerson.PersonId };
        //pD.BranchId = locatedPerson.Branch != null ? locatedPerson.Branch.BranchId : (long?)null;

        if (userInWebRoles.Count > 0)
          pD.Roles = new List<WebRole>();

        foreach (var role in userInWebRoles)
        {
          pD.Roles.Add(new WebRole { WebRoleId = role.WebRole.WebRoleId, WebRoleDescription = role.WebRole.Role });
        }
        return pD;
      }
    }

    /// <summary>
    /// Returns roles associated to a person
    /// </summary>
    /// <param name="personId">Roles to determine for person</param>
    /// <param name="cache">cache roles in redis to prevent db bottlenecks</param>
    public List<Role> Operations_GetPermissions(long personId, bool cache)
    {

      return null;
    }

    /// <summary>
    /// Returns roles stored in core database.
    /// </summary>
    public List<WebRole> Operations_GetWebRoles()
    {
      var webRoleCollection = new List<WebRole>();

      using (var uow = new UnitOfWork())
      {
        var webRoles = new XPQuery<WEB_WebRole>(uow).ToList();

        webRoleCollection.AddRange(webRoles.Select(web => new WebRole
        {
          WebRoleDescription = web.Role,
          WebRoleId = web.WebRoleId
        }));
      }

      return webRoleCollection;
    }

    /// <summary>
    /// Returns associated branches for user.
    /// </summary>
    /// <param name="personId"></param>
    /// <param name="cache"></param>
    /// <returns></returns>
    public List<long> GetAssociatedBranches(long personId, bool cache)
    {
      using (var uow = new UnitOfWork())
      {
        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
        if (person == null)
        {
          return new List<long>();
        }
        //var branches = new XPQuery<BRN_Branch>(uow).Where(p => p.Persons.OfType<PER_Person>().Count(c => c.PersonId == personId) > 0);

        return person.GetBranches.Select(b => b.Branch.BranchId).ToList();
      }
    }

    /// <summary>
    /// Returns a list of employee's in the system.
    /// </summary>
    /// <returns></returns>
    public List<Person> Operations_GetPersonList(List<long> idCollection)
    {
      var collection = new List<Person>();
      using (var uow = new UnitOfWork())
      {
        var personCollection =
          new XPQuery<PER_Person>(uow).Where(p => p.PersonType.TypeId == (int)General.PersonType.Employee && idCollection.Contains(p.PersonId)).Select(c => new
          {
            c.Firstname,
            c.IdNum,
            c.Lastname,
            c.PersonId
          }).ToList();

        collection.AddRange(personCollection.Select(per => new Person
        {
          Firstname = per.Firstname,
          IdNum = per.IdNum,
          Lastname = per.Lastname,
          PersonId = per.PersonId
        }));
      }
      return collection;
    }

    #endregion

    #region AVS

    public Tuple<List<AvsStatistics>, List<AvsTransactions>> AVS_GetTransactions(long? branchId, DateTime? startDate, DateTime? endDate, long? transactionId, string idNumber, long? bankId)
    {
      return new AvsUtility().GetTransactions(branchId, startDate, endDate, transactionId, idNumber, bankId);
    }

    public void AVS_ResendTransactions(List<long> transactionIds)
    {
      new AvsUtility().ResendAvsTransactions(transactionIds);
    }

    public void AVS_CancelTransactions(List<long> transactionIds)
    {
      new AvsUtility().CancelAvsTransactions(transactionIds);
    }

    public List<Bank> AVS_GetBanks()
    {
      return new AvsUtility().GetAVSBanks();
    }

    public Dictionary<AvsService, List<AvsServiceBank>> AVS_GetServiceSchedules()
    {
      return new AvsUtility().GetServiceSchedules();
    }

    public void AVS_UpdateServicesSchedules(Dictionary<AvsService, List<AvsServiceBank>> serviceSchedules)
    {
      new AvsUtility().UpdateServiceSchedule(serviceSchedules);
    }

    #endregion

    #region Ping

    /// <summary>
    ///  Determine states of various services required, 
    ///  report them to FE to display partial operational behaviours.
    /// </summary>
    /// <returns>Object with various service states.</returns>
    public string Ping()
    {
      return "Pong";
    }


    #endregion

    #region Payout

    public Tuple<PayoutStatistics, List<PayoutTransaction>> Payout_GetTransactions(long? branchId, DateTime? startRangeActionDate, DateTime? endRangeActionDate, long? payoutId, string idNumber, int? bankId)
    {
      return PayoutUtility.GetPayoutTransaction(branchId, startRangeActionDate, endRangeActionDate, payoutId, idNumber, bankId);
    }

    public List<Bank> Payout_GetBanks()
    {
      return PayoutUtility.GetBanks();
    }

    public List<DashboardAlert> Payout_GetAlerts()
    {
      return PayoutUtility.GetAlerts();
    }

    public void Payout_PlaceOnHold(long payoutId)
    {
      PayoutUtility.PlaceOnHold(payoutId);
    }

    public void Payout_RemoveFromHold(long payoutId)
    {
      PayoutUtility.RemoveFromHold(payoutId);
    }

    #endregion

    #region Notification

    /// <summary>
    /// Deletes a notification from the redis DB
    /// </summary>
    public void Notification_MarkAsRead(long userId, string notificationId)
    {
      NotificationUtility.MarkNotificationAsRead(userId, notificationId);
    }

    /// <summary>
    /// GetDetail of a notification
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="notificationId"></param>
    /// <returns></returns>
    public Notification Notification_Get(long userId, string notificationId)
    {
      return NotificationUtility.GetNotification(userId, notificationId);
    }

    /// <summary>
    /// Get Notifications for a branch and/or user
    /// </summary>
    public List<Notification> Notification_Gets(long? branchId, long? userId)
    {
      return NotificationUtility.GetNotifications(branchId, userId);
    }

    #endregion

    #region Workflow

    public List<ProcessAccount> Workflow_Get(General.Host host, long? branchId, string accountNo, DateTime? startRange, DateTime? endRange)
    {
      return WorkflowUtility.GetWorkflow(host, branchId, accountNo, startRange, endRange);
    }

    public List<ProcessStepAccount> Workflow_GetProcessSteps(long processJobId)
    {
      return WorkflowUtility.GetProcessSteps(processJobId);
    }

    public void Workflow_RedirectAccountToProcessStep(long processStepJobAccountId, long userId)
    {
      WorkflowUtility.RedirectAccount(processStepJobAccountId, userId);
    }

    #endregion

    #region General

    public List<Host> Host_GetAccessible(long personId)
    {
      var accessibleHosts = new List<Host>();
      using (var uow = new UnitOfWork())
      {
        var hosts = new XPQuery<Atlas.Domain.Model.Host>(uow).ToList();
        accessibleHosts.AddRange(hosts.Select(host => new Host
        {
          HostId = host.HostId,
          HostName = host.Description,
          Type = host.Type
        }));
      }

      return accessibleHosts;
    }

    public List<PublicHoliday> PublicHolidays_Get(DateTime fromDate)
    {
      var publicHolidays = new List<PublicHoliday>();

      using (var uow = new UnitOfWork())
      {
        var holidays = new XPQuery<Atlas.Domain.Model.PublicHoliday>(uow).Where(d => d.Date >= fromDate).ToList();
        holidays.ForEach(h => publicHolidays.Add(new PublicHoliday { Name = h.Name, Date = h.Date }));
      }
      return publicHolidays;
    }

    #endregion

    #region Orchestration

    /// <summary>
    /// Generate OTP verification code
    /// </summary>
    /// <param name="cellNo"></param>
    /// <returns>OTP Code</returns>
    public TupleOfintstring OTP_Send(string cellNo)
    {
      TupleOfintstring result = null;

      new OrchestrationServiceClient("OrchestrationService.NET").Using(cli =>
      {
        result = cli.GenerateOTP();
      });

      MqBus.Bus().Publish(new SMSNotifyMessage(CombGuid.Generate())
      {
        CreatedAt = DateTime.Now,
        To = cellNo,
        Body = string.Format("Falcon OTP: {0}", result.m_Item1),
        ActionDate = DateTime.Now,
        Priority = Atlas.Enumerators.Notification.NotificationPriority.High
      });


      return result;

    }
    public bool OTP_Verify(string security, int otp)
    {
      bool valid = false;
      new OrchestrationServiceClient("OrchestrationService.NET").Using(cli =>
      {
        valid = cli.VerifyOTP(security, otp);
      });

      return valid;
    }

    public Tuple<bool, string> OTP_VerifyToHash(string hash, int otp)
    {
      var valid = false;

      var keyValue = RedisConnection.Current.GetDatabase().StringGet(hash);
      string resetHash;

      if (!keyValue.IsNullOrEmpty)
      {
        if (!keyValue.ToString().Contains(":"))
          return null;

        var splitKey = keyValue.ToString().Split(':');
        var storedSecurity = splitKey[1];
        var username = splitKey[2];

        new OrchestrationServiceClient("OrchestrationService.NET").Using(cli =>
        {
          valid = cli.VerifyOTP(storedSecurity, otp);
        });

        if (!valid)
          return new Tuple<bool, string>(false, string.Empty);

        resetHash = Hashing.GetSHA256(string.Format("{0}{1}{2}", hash, otp, (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds));
        RedisConnection.Current.GetDatabase().StringSet(resetHash, username, new TimeSpan(0, 10, 0));
      }
      else
        return null;

      return new Tuple<bool, string>(true, resetHash);
    }

    public string OTP_StoreToHash(string otpSecurity, string cellNo)
    {
      var hash = Hashing.GetSHA256(string.Format("{0}{1}{2}", otpSecurity, cellNo, (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds));
      RedisConnection.Current.GetDatabase().StringSet(hash, otpSecurity, new TimeSpan(0, 5, 30));

      return hash;
    }

    #endregion

    #region Account

    public List<Common.Structures.Account> Account_Search(string searchString)
    {
      return AccountUtility.Search(searchString);
    }

    public AccountDetail Account_GetDetail(long accountId)
    {
      return AccountUtility.GetAccountDetail(accountId, false);
    }

    public void Account_UpdateStatus(long accountId, long userId, Account.AccountStatus newStatus, Account.AccountStatusReason? reason, Account.AccountStatusSubReason? subReason)
    {
      AccountUtility.UpdateAccountStatus(accountId, userId, newStatus, reason, subReason);
    }

    public void Account_AddNote(long accountId, long userPersonId, string note, long? parentNoteId)
    {
      // Bye bye xsockets
      //new Controllers.AccountDetail().SendNoteUpdate(accountId, AccountUtility.AddNote(accountId, userPersonId, note, parentNoteId));
    }

    public void Account_EditNote(long noteId, long userPersonId, string note)
    {
      AccountUtility.EditNote(noteId, userPersonId, note);
    }

    public void Account_DeleteNote(long noteId, long userPersonId)
    {
      AccountUtility.DeleteNote(noteId, userPersonId);
    }

    public void Account_AdjustLoan(long accountId, decimal loanAmount, int period)
    {
      var account = AccountUtility.AdjustAccount(accountId, loanAmount, period);
      if (account.Host.Type == General.Host.Atlas_Online)
      {
        MqBus.Bus().Publish(new UpdateApplicationInformation(CombGuid.Generate())
        {
          CreatedAt = DateTime.Now,
          AccountId = accountId,
          Type = UpdateApplicationInformation.MessageType.PaymentDateAndAffordabilityRequest,
          RepaymentDate = DateTime.Today.AddDays(period)
        });
      }
    }

    public List<AffordabilityCategory> Account_Affordability_GetCategories(General.Host host)
    {
      return AccountUtility.GetAffordabilityCategories(host);
    }

    public void Account_Affordability_AcceptOption(long accountId, int affordabilityOptionId)
    {
      AccountUtility.AcceptAffordabilityOption(accountId, affordabilityOptionId);
    }

    public AccountAffordabilityItem Account_Affordability_AddItem(long accountId, int affordabilityCategoryId, decimal amount, long personId)
    {
      return AccountUtility.AddAffordabilityItem(accountId, affordabilityCategoryId, amount, personId);
    }

    public AccountAffordabilityItem Account_Affordability_DeleteItem(long accountId, long affordabilityId, long personId)
    {
      return AccountUtility.DeleteAffordabilityItem(accountId, affordabilityId, personId);
    }

    public void Account_QuotationAccept(long accountId, long quotationId)
    {
      var account = AccountUtility.AcceptQuotation(accountId, quotationId);
      if (account.Host.Type == General.Host.Atlas_Online)
      {
        MqBus.Bus().Publish(new UpdateApplicationInformation(CombGuid.Generate())
        {
          CreatedAt = DateTime.Now,
          AccountId = accountId,
          Type = UpdateApplicationInformation.MessageType.AffordabilityRejectionDeclineLoan
        });
      }
    }

    public void Account_QuotationReject(long accountId, long quotationId)
    {
      var account = AccountUtility.RejectQuotation(accountId, quotationId);
      if (account.Host.Type == General.Host.Atlas_Online)
      {
        MqBus.Bus().Publish(new UpdateApplicationInformation(CombGuid.Generate())
        {
          CreatedAt = DateTime.Now,
          AccountId = accountId,
          Type = UpdateApplicationInformation.MessageType.AffordabilityRejectionDeclineLoan
        });
      }
    }

    #endregion

    #region General Ledger

    public List<LedgerTransactionType> Ledger_GetPastPaymentTypes()
    {
      var ledgerTypes = new List<LedgerTransactionType>();
      using (var uow = new UnitOfWork())
      {
        var transactionTypes = new XPQuery<LGR_TransactionType>(uow).Where(a => a.TransactionTypeGroup.TType == GeneralLedger.TransactionTypeGroup.Payment);
        foreach (var transactionType in transactionTypes)
        {
          ledgerTypes.Add(new LedgerTransactionType
          {
            Description = transactionType.Description,
            SortKey = transactionType.SortKey,
            TransactionTypeGroup = transactionType.TransactionTypeGroup.Description,
            TransactionTypeGroupId = transactionType.TransactionTypeGroup.TransactionTypeGroupId,
            TransactionTypeId = transactionType.TransactionTypeId
          });
        }
      }

      return ledgerTypes;
    }

    public void Ledger_AddPastPayment(long accountId, long personId, GeneralLedger.TransactionType transactionType, DateTime transactionDate, decimal amount)
    {
      using (var uow = new UnitOfWork())
      {
        var createUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
        if (createUser == null)
          throw new Exception("User does not exist in DB");

        throw new MissingMethodException();
        //var gl = new Atlas.LoanEngine.Account.GeneralLedger(accountId);
        //if (transactionType == GeneralLedger.TransactionType.EFTPayment)
        //  gl.PostPastPayment(transactionType, transactionDate, Mapper.Map<PER_Person, PER_PersonDTO>(createUser), amount);
        //else
        //  gl.AddTransaction(transactionType, transactionDate, Mapper.Map<PER_Person, PER_PersonDTO>(createUser), amount);
      }
    }

    #endregion

    #region Person

    public AccountContact Person_AddContact(long personId, General.ContactType contactType, string value)
    {
      return PersonUtility.AddContact(personId, contactType, value);
    }

    public AccountContact Person_DisableContact(long personId, long contactId)
    {
      return PersonUtility.DisableContact(personId, contactId);
    }

    public AccountAddress Person_AddAddress(long personId, long userPersonId, General.AddressType addressType, General.Province province,
      string line1, string line2, string line3, string line4, string postalCode)
    {
      return PersonUtility.AddAddress(personId, userPersonId, addressType, province, line1, line2, line3, line4, postalCode);
    }

    public AccountAddress Person_DisableAddress(long personId, long addressId)
    {
      return PersonUtility.DisableAddress(personId, addressId);
    }

    public Relation Person_NewRelation(long personId, long userPersonId, string firstname, string lastname, string cellNo, General.RelationType relationType)
    {
      return PersonUtility.CreateNewRelation(personId, userPersonId, firstname, lastname, cellNo, relationType);
    }

    public Relation Person_UpdateRelation(long personId, long relationPersonId, string firstname, string lastname, string cellNo, General.RelationType relationType)
    {
      return PersonUtility.UpdateRelation(personId, relationPersonId, firstname, lastname, cellNo, relationType);
    }

    public bool Person_LocateByIdentityNo(string idNo)
    {
      return PersonUtility.LocateByIdentityNo(idNo) != null;
    }


    public PER_PersonDTO Person_GetByIdentityNo(string idNo)
    {
      return PersonUtility.LocateByIdentityNo(idNo);
    }

    public PER_PersonDTO Person_GetByPrimaryKey(long pk)
    {
      return null;
    }

    public long? Person_VerifyIsValid(string idNo, string cellNo)
    {
      return PersonUtility.VerifyIsValid(idNo, cellNo);
    }

    public string Person_ValidatePasswordReset(string hash)
    {
      var result = RedisConnection.Current.GetDatabase().StringGet(hash);

      if (result.IsNull)
        return string.Empty;

      return result;
    }

    #endregion

    #region Fraud

    /// <summary>
    /// Override the result of a fraud score record
    /// </summary>
    public bool Fraud_OverrideResult(long fraudScoreId, long overridePersonId, string reason)
    {
      using (var uow = new UnitOfWork())
      {
        var fraud = new XPQuery<FPM_FraudScore>(uow).FirstOrDefault(p => p.FraudScoreId == fraudScoreId);

        if (fraud == null)
          throw new Exception(string.Format("Unable to locate fraud score [{0}]", fraudScoreId));

        var overridePerson = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == overridePersonId);

        if (overridePerson == null)
        {

        }
        // throw new Exception(string.Format("Unable to locate override user [{0}]", overridePersonId));

        fraud.OverrideDate = DateTime.Now;
        fraud.Passed = true;
        fraud.OverrideUser = overridePerson;
        fraud.OverrideReason = reason;

        uow.CommitChanges();
      }

      return true;
    }

    #endregion

    #region Authentication

    /// <summary>
    /// Used to reset an attempt on authenication, this may be required in case of someone having internet issues, or service provider issues.
    /// </summary>
    public bool Authentication_ResetAttempts(long authenticationId, long overridePersonId, string reason)
    {
      using (var uow = new UnitOfWork())
      {
        var overridePerson = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == overridePersonId);

        if (overridePerson == null)
          throw new Exception(string.Format("Unable to locate override user [{0}]", overridePersonId));

        var authentication = new XPQuery<FPM_Authentication>(uow).FirstOrDefault(p => p.AuthenticationId == authenticationId && p.Enabled);

        if (authentication != null)
        {
          authentication.Enabled = false;
          authentication.OverrideDate = DateTime.Now;
          authentication.OverridePerson = overridePerson;
          authentication.OverrideReason = reason;

          if (authentication.Account != null)
          {
            // Only change status of account if its a atlasonline and its been declined because of authentication, if they are overriding this it should be reset (technical error perhaps)
            if (authentication.Account.Host.Type == General.Host.Atlas_Online && authentication.Account.Status.Type == Account.AccountStatus.Declined
              && authentication.Account.StatusReason.Type == Account.AccountStatusReason.Authentication)
            {
              throw new MissingMethodException();
              //var accountEngine = new Default(authentication.Account.AccountId);
              //accountEngine.SetAccountStatus(authentication.Account, Account.AccountStatus.Inactive, null, null, uow);
            }
          }
        }
        uow.CommitChanges();
      }
      return true;
    }

    /// <summary>
    /// Used to override the result of the authentication attempts
    /// </summary>
    public bool Authentication_OverrideResult(long authenticationId, long overridePersonId, string reason)
    {
      using (var uow = new UnitOfWork())
      {
        var overridePerson = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == overridePersonId);

        if (overridePerson == null)
          throw new Exception(string.Format("Unable to locate override user [{0}]", overridePersonId));

        var authentication = new XPQuery<FPM_Authentication>(uow).FirstOrDefault(p => p.AuthenticationId == authenticationId && p.Enabled);

        if (authentication != null)
        {
          authentication.Authenticated = true;
          authentication.Completed = true;
          authentication.OverrideDate = DateTime.Now;
          authentication.OverridePerson = overridePerson;
          authentication.OverrideReason = reason;

          if (authentication.Account != null)
          {
            // Only change status of account if its a atlasonline and its been declined because of authentication, if they are overriding this it should be reset (technical error perhaps)
            if (authentication.Account.Host.Type == General.Host.Atlas_Online && authentication.Account.Status.Type == Account.AccountStatus.Declined
              && authentication.Account.StatusReason.Type == Account.AccountStatusReason.Authentication)
            {
              throw new MissingMethodException();
              //var accountEngine = new Default(authentication.Account.AccountId);
              //accountEngine.SetAccountStatus(authentication.Account, Account.AccountStatus.Inactive, null, null, uow);
            }
          }
        }

        uow.CommitChanges();
      }
      return true;
    }

    #endregion

    #region Credit

    /// <summary>
    /// Resubmits a credit enquiry
    /// </summary>
    /// <param name="enquiryId"></param>
    public string Credit_ReSubmit(long enquiryId)
    {
      throw new Exception("Not Implemented");
    }

    public string Credit_FetchCreditReport(long enquiryId)
    {
      string report = string.Empty;
      new CreditServerClient("CreditServer.NET").Using(cli =>
      {
        report = cli.GetReport(enquiryId);
      });

      return report;
    }





    #endregion

    #region PDF


    #endregion

    #region ASS Reporting

    public List<Region> AssReporting_GetPersonRegions(long personId)
    {
      return AssReporting.GetPersonRegions(personId);
    }

    public List<Branch> AssReporting_GetRegionBranches(long regionId)
    {
      return AssReporting.GetRegionBranches(regionId);
    }

    public List<RegionBranch> AssReporting_GetPersonRegionBranches(long personId)
    {
      return AssReporting.GetPersonRegionBranches(personId);
    }

    public MainSummary AssReporting_GetMainSummary(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      return new AssReporting().GetMainSummary(branchIds, startDate, endDate);
    }

    public List<Cheque> AssReporting_GetCheque(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      return new AssReporting().GetCheque(branchIds, startDate, endDate, _log);
    }

    public List<InterestPercentiles> AssReporting_GetInterestPercentiles(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      return new AssReporting().GetInterestPercentiles(branchIds, startDate, endDate);
    }

    public List<InsurancePercentiles> AssReporting_GetInsurancePercentiles(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      return new AssReporting().GetInsurancePercentiles(branchIds, startDate, endDate);
    }

    public List<Insurance> AssReporting_GetInsuranceFees(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      return new AssReporting().GetInsurance(branchIds, startDate, endDate);
    }

    public List<Interest> AssReporting_GetInterestFees(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      return new AssReporting().GetInterest(branchIds, startDate, endDate);
    }

    public List<LoanMix> AssReporting_GetLoanMix(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      return new AssReporting().GetLoanMix(branchIds, startDate, endDate);
    }

    public List<AverageNewCientLoan> AssReporting_GetAverageNewClientLoan(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      return new AssReporting().GetAverageNewClientLoanSize(branchIds, startDate, endDate);
    }

    public List<AverageLoan> AssReporting_GetAverageLoan(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      return new AssReporting().GetAverageLoanSize(branchIds, startDate, endDate);
    }

    public byte[] AssReporting_ExportCIReport(List<long> branchIds, DateTime startDate, DateTime endDate, bool exportPossibleHandover)
    {
      _log.Debug("CI Report Hit");
      var a = new CiExtract().ExportCiReport(branchIds, startDate, endDate, _log, exportPossibleHandover);
      _log.Debug("CI Report end");

      return a;
    }

    #endregion

    #region Targets

    public void Targets_AddNewPossibleHandover()
    {

    }

    #endregion
    
    #region Ass integration

    public long AssInt_AddUserOverride(DateTime startDate, DateTime endDate, string userOperatorCode, string branchNum, 
      string regionalOperatorId, byte newLevel, string reason)
    {
      _log.Information("[FalconService][AssIntegration_AddUserOverride]- {startDate}, {endDate}, {userOperatorCode}, " +
        "{branchNum}, {regionalOperatorId}, {newLevel}, {reason}", startDate, endDate, userOperatorCode,
        branchNum, regionalOperatorId, newLevel, reason);

      Int64 recId = -1;
      new AssIntService.AssThirdPartyClient("AssInt.NET").Using(client =>
        {
          recId = client.AddUserOverride(new AssIntService.AddUserOverrideArgs
          {
            StartDate = startDate,
            EndDate = endDate,
            UserOperatorCode = userOperatorCode,
            BranchNum = branchNum,
            RegionalOperatorId = regionalOperatorId,
            NewLevel = newLevel,
            Reason = reason
          });

        });

      _log.Information("[FalconService][AssIntegration_AddUserOverride]- Finished {recId}", recId);

      return recId;
    }

    #endregion
  }
}