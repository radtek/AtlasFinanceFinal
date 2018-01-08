using System;
using System.Collections.Generic;
using System.ServiceModel;

using Atlas.Enumerators;
using Falcon.Common.Structures;
using Falcon.Common.Structures.Avs;
using Falcon.Common.Structures.Branch;
using Falcon.Common.Structures.Report.Ass;
using Falcon.Common.Structures.Report.General;
using Falcon.Service.OrchestrationService;
using Account = Atlas.Enumerators.Account;
using Notification = Falcon.Common.Structures.Notification;
using PER_PersonDTO = Atlas.Domain.DTO.PER_PersonDTO;

namespace Falcon.Service.WCF.Interface
{
  [ServiceContract]
  public interface IFalconService
  { 
    #region Database Association Operations

    [OperationContract]
    Person Operations_LocatebyId(string idNo);

    [OperationContract]
    Person Operations_LocateByPersonId(long personId);

    [OperationContract]
    List<Role> Operations_GetPermissions(long personId, bool cache);

    [OperationContract]
    List<WebRole> Operations_GetWebRoles();

    [OperationContract]
    List<Person> Operations_GetPersonList(List<long> idCollection);

    #endregion

    #region AVS

    [OperationContract]
    Tuple<List<AvsStatistics>, List<AvsTransactions>> AVS_GetTransactions(long? branchId, DateTime? startDate, DateTime? endDate, long? transactionId, string idNumber, long? bankId);

    [OperationContract]
    void AVS_ResendTransactions(List<long> transactionIds);

    [OperationContract]
    void AVS_CancelTransactions(List<long> transactionIds);

    [OperationContract]
    List<Bank> AVS_GetBanks();

    [OperationContract]
    Dictionary<AvsService, List<AvsServiceBank>> AVS_GetServiceSchedules();

    [OperationContract]
    void AVS_UpdateServicesSchedules(Dictionary<AvsService, List<AvsServiceBank>> serviceSchedules);

    #endregion

    #region Ping

    [OperationContract]
    string Ping();

    #endregion

    #region Payout

    [OperationContract]
    Tuple<PayoutStatistics, List<PayoutTransaction>> Payout_GetTransactions(long? branchId, DateTime? startRangeActionDate, DateTime? endRangeActionDate, long? payoutId, string idNumber, int? bankId);

    [OperationContract]
    List<Bank> Payout_GetBanks();

    [OperationContract]
    List<DashboardAlert> Payout_GetAlerts();

    [OperationContract]
    void Payout_PlaceOnHold(long payoutId);

    [OperationContract]
    void Payout_RemoveFromHold(long payoutId);
    #endregion

    #region Notification

    [OperationContract]
    Notification Notification_Get(long userId, string notificationId);

    [OperationContract]
    void Notification_MarkAsRead(long userId, string notificationId);

    [OperationContract]
    List<Notification> Notification_Gets(long? branchId, long? userId);

    #endregion

    #region Workflow

    [OperationContract]
    List<ProcessAccount> Workflow_Get(General.Host host, long? branchId, string accountNo, DateTime? startRange, DateTime? endRange);

    [OperationContract]
    List<ProcessStepAccount> Workflow_GetProcessSteps(long processJobId);

    [OperationContract]
    void Workflow_RedirectAccountToProcessStep(long processStepJobAccountId, long userId);

    #endregion

    #region General

    [OperationContract]
    List<Host> Host_GetAccessible(long personId);

    [OperationContract]
    List<PublicHoliday> PublicHolidays_Get(DateTime fromDate);

    #endregion

    #region OTP

    [OperationContract]
    TupleOfintstring OTP_Send(string cellNo);

    [OperationContract]
    bool OTP_Verify(string security, int otp);

    [OperationContract]
    Tuple<bool, string> OTP_VerifyToHash(string hash, int otp);

    [OperationContract]
    string OTP_StoreToHash(string otpSecurity, string cellNo);

    #endregion

    #region Account

    [OperationContract]
    List<Common.Structures.Account> Account_Search(string searchString);

    [OperationContract]
    AccountDetail Account_GetDetail(long accountId);

    [OperationContract]
    void Account_UpdateStatus(long accountId, long userId, Account.AccountStatus newStatus, Account.AccountStatusReason? reason, Account.AccountStatusSubReason? subReason);

    [OperationContract]
    void Account_AddNote(long accountId, long userPersonId, string note, long? parentNoteId);

    [OperationContract]
    void Account_EditNote(long noteId, long userPersonId, string note);

    [OperationContract]
    void Account_DeleteNote(long noteId, long userPersonId);

    [OperationContract]
    void Account_AdjustLoan(long accountId, decimal loanAmount, int period);

    [OperationContract]
    List<AffordabilityCategory> Account_Affordability_GetCategories(General.Host host);

    [OperationContract]
    void Account_Affordability_AcceptOption(long accountId, int affordabilityOptionId);

    [OperationContract]
    AccountAffordabilityItem Account_Affordability_AddItem(long accountId, int affordabilityCategoryId, decimal amount, long personId);

    [OperationContract]
    AccountAffordabilityItem Account_Affordability_DeleteItem(long accountId, long affordabilityId, long personId);

    [OperationContract]
    void Account_QuotationAccept(long accountId, long quotationId);

    [OperationContract]
    void Account_QuotationReject(long accountId, long quotationId);

    #endregion

    #region General Ledger

    [OperationContract]
    List<LedgerTransactionType> Ledger_GetPastPaymentTypes();

    [OperationContract]
    void Ledger_AddPastPayment(long accountId, long personId, GeneralLedger.TransactionType transactionType, DateTime transactionDate, decimal amount);

    #endregion

    #region Person

    [OperationContract]
    AccountContact Person_AddContact(long personId, General.ContactType contactType, string value);

    [OperationContract]
    AccountContact Person_DisableContact(long personId, long contactId);

    [OperationContract]
    AccountAddress Person_AddAddress(long personId, long userPersonId, General.AddressType addressType, General.Province province,
      string line1, string line2, string line3, string line4, string postalCode);

    [OperationContract]
    AccountAddress Person_DisableAddress(long personId, long addressId);

    [OperationContract]
    Relation Person_NewRelation(long personId, long userPersonId, string firstname, string lastname, string cellNo, General.RelationType relationType);

    [OperationContract]
    Relation Person_UpdateRelation(long personId, long relationPersonId, string firstname, string lastname, string cellNo, General.RelationType relationType);

    [OperationContract]
    bool Person_LocateByIdentityNo(string idNo);

    [OperationContract]
    PER_PersonDTO Person_GetByIdentityNo(string idNo);

    [OperationContract]
    PER_PersonDTO Person_GetByPrimaryKey(long pk);

    [OperationContract]
    long? Person_VerifyIsValid(string idNo, string cellNo);

    [OperationContract]
    string Person_ValidatePasswordReset(string hash);

    #endregion

    #region Fraud

    [OperationContract]
    bool Fraud_OverrideResult(long fraudScoreId, long overridePersonId, string reason);

    #endregion

    #region Authentication

    [OperationContract]
    bool Authentication_ResetAttempts(long authenticationId, long overridePersonId, string reason);

    [OperationContract]
    bool Authentication_OverrideResult(long authenticationId, long overridePersonId, string reason);

    #endregion

    #region Credit

    [OperationContract]
    string Credit_ReSubmit(long enquiryId);

    [OperationContract]
    string Credit_FetchCreditReport(long enquiryId);

    #endregion

    #region PDF


    #endregion

    #region ASS Reporting

    [OperationContract]
    List<Region> AssReporting_GetPersonRegions(long personId);

    [OperationContract]
    List<Branch> AssReporting_GetRegionBranches(long regionId);

    [OperationContract]
    List<RegionBranch> AssReporting_GetPersonRegionBranches(long personId);

    [OperationContract]
    MainSummary AssReporting_GetMainSummary(List<long> branchIds, DateTime startDate, DateTime endDate);

    [OperationContract]
    List<Cheque> AssReporting_GetCheque(List<long> branchIds, DateTime startDate, DateTime endDate);

    [OperationContract]
    List<InsurancePercentiles> AssReporting_GetInsurancePercentiles(List<long> branchIds, DateTime startDate, DateTime endDate);

    [OperationContract]
    List<InterestPercentiles> AssReporting_GetInterestPercentiles(List<long> branchIds, DateTime startDate, DateTime endDate);

    [OperationContract]
    List<Insurance> AssReporting_GetInsuranceFees(List<long> branchIds, DateTime startDate, DateTime endDate);

    [OperationContract]
    List<Interest> AssReporting_GetInterestFees(List<long> branchIds, DateTime startDate, DateTime endDate);

    [OperationContract]
    List<LoanMix> AssReporting_GetLoanMix(List<long> branchIds, DateTime startDate, DateTime endDate);

    [OperationContract]
    List<AverageNewCientLoan> AssReporting_GetAverageNewClientLoan(List<long> branchIds, DateTime startDate, DateTime endDate);

    [OperationContract]
    List<AverageLoan> AssReporting_GetAverageLoan(List<long> branchIds, DateTime startDate, DateTime endDate);

    [OperationContract]
    byte[] AssReporting_ExportCIReport(List<long> branchIds, DateTime startDate, DateTime endDate, bool exportPossibleHandover);

    #endregion

    #region Targets

    void Targets_AddNewPossibleHandover();

    #endregion

    #region Ass integration

    [OperationContract]
    long AssInt_AddUserOverride(DateTime startDate, DateTime endDate, string userOperatorCode, string branchNum,
      string regionalOperatorId, byte newLevel, string reason);

    #endregion

  }
}