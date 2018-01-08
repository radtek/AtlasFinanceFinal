using System;
using System.Collections.Generic;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;
using Stream.Framework.DataContracts.Requests;
using Stream.Framework.Enumerators;
using Stream.Framework.Structures;

namespace Stream.Framework.Repository
{
  public interface IStreamRepository
  {
    IDebtor AddOrUpdateDebtor(AddOrUpdateDebtorRequest request);

    IStreamAccount AddOrUpdateAccount(AddOrUpdateAccountRequest request);

    IStreamAccount GetAccount(long accountId);

    ICollection<IStreamAccount> GetAccountsByCaseId(long caseId, bool? upToDate = null);

    ICase GetCase(long caseId);

    ICollection<ICase> GetCasesByDebtorId(long debtorId, Enumerators.Stream.GroupType? groupType = null, params CaseStatus.Type[] caseStatuses);

    ICase GetCaseByAccountId(long accountId);

    ICollection<ICase> GetCasesByStatus(Enumerators.Stream.GroupType groupType, params CaseStatus.Type[] caseType);

    ICase AddOrUpdateCase(AddOrUpdateCaseRequest request);

    ICaseStream GetOpenCaseStreamForCase(long caseId);
    ICaseStream AddOrUpdateCaseStream(AddOrUpdateCaseStreamRequest request);

    ICaseStreamAllocation AddOrUpdateCaseStreamAllocation(AddOrUpdateCaseStreamAllocationRequest request);

    ICaseStreamEscalation AddOrUpdateCaseStreamEscalation(AddOrUpdateCaseStreamEscalationRequest request);

    ICaseStreamAction AddOrUpdateCaseStreamAction(AddOrUpdateCaseStreamActionRequest request);

    IDebtorContact AddOrUpdateDebtorContact(AddOrUpdateDebtorContactRequest request);

    IDebtorAddress AddOrUpdateDebtorAddress(AddOrUpdateDebtorAddressRequest request);

    ICaseStreamAction GetNextAction(long caseStreamId, Enumerators.Action.Type actionType);

    ITransaction AddOrUpdateAccountTransaction(AddOrUpdateAccountTransactionRequest request);

    ICollection<ITransaction> GetAccountTransactions(long accountId);
    ICollection<ITransaction> GetCaseTransactions(long caseId);
    ICollection<ITransaction> GetCaseAccountTransactions(long accountId, long caseId); 

    void UpdateAccountWithLastImportReference(long accountId, string lastImportReference);

    void CompletePtp(long caseStreamActionId, bool success, long personId);

    void AddCaseStreamActionWithPriority(long caseStreamId, long personId, DateTime actionDate,
      Enumerators.Action.Type actionType, bool allowMultipleActionTypes = true,
      bool completePendingActions = true, Enumerators.Stream.PriorityType? prirotyType = null);

    ICollection<long> GetAccountsReference2ByCaseStreamId(long caseStreamId);
    Dictionary<string, long> GetCurrentAccountReferences(Enumerators.Stream.GroupType groupType);
    ICollection<IAccountLastTransactionImportReference> GetCurrentAccountReferencesAndLastImportReference(Enumerators.Stream.GroupType groupType);

    Tuple<List<INote>, string> GetNoteHistory(long caseId);
    List<IAccountStreamAction> GetAccountStreamActions(params long[] caseIds);

    void AddActuator(Enumerators.Stream.ActuatorType actuatorType, long? branchId, long? regionId,
      DateTime rangeStart, DateTime rangeEnd,
      bool isActive = true, bool disableOverlappingActuators = false);

    void EscalateUnworkedCases();
    byte[] GetFinalLetterOfDemandPdf(long caseStreamId);
    IEnumerable<CaseStatus.Type> GetCaseStatuses();
    List<IStream> GetStreamTypes(Enumerators.Stream.GroupType groupType);
    List<IComment> GetCommentsByStreamGroup(Enumerators.Stream.GroupType groupType);

    List<IAccountStreamAction> GetWorkItems(Enumerators.Stream.GroupType[] groupTypes,
      CaseStatus.Type?[] caseStatuses = null, long? branchId = null, long? allocatedPersonId = null,
      string allocatedUserId = null, TimeSpan? bufferTime = null, DateTime? completeDate = null, long caseId = 0,
      string idNumber = null, string accountReference = null, DateTime? actionDateStartRange = null,
      DateTime? actionDateEndRange = null, int limitResults = 100,
      DateTime? createDateStartRange = null, DateTime? createDateEndRange = null,
      params Enumerators.Stream.StreamType[] streamTypes);

    List<IEscalatedCaseStream> GetEscalatedWorkItemsOnly(string allocatedUserId, long branchId, int limitResults = 100);

    Tuple<IWorkItem, List<INote>, string> GetNextWorkItem(Enumerators.Stream.GroupType groupType,
      string userId, Enumerators.Stream.StreamType streamType);

    Tuple<IWorkItem, List<INote>, string> GetWorkItem(long caseStreamActionId, string userId);

    Dictionary<string, List<Tuple<IStream, int>>> GetWorkItemsSummary(Enumerators.Stream.GroupType groupType,
      List<string> userIds, params Enumerators.Stream.StreamType[] streamTypes);

    INote AddAccountNote(string userId, string note, long caseId,
      Enumerators.Stream.AccountNoteType accountNoteType = Enumerators.Stream.AccountNoteType.Normal);

    void AddCaseStreamActionWithPriority(long caseStreamId, string userId, DateTime actionDate,
      Enumerators.Action.Type actionType, bool allowMultipleActionTypes = true,
      bool completePendingActions = true, Enumerators.Stream.PriorityType? prirotyType = null);

    void AddCaseStreamAction(long caseStreamId, DateTime actionDate, Enumerators.Action.Type actionType,
      bool allowMultipleActionTypes = true, bool completePendingActions = true);

    Dictionary<long, string> GetAccountReferenceByCaseStatus(Enumerators.Stream.GroupType[] groupType,
      params CaseStatus.Type[] caseStatuses);

    List<long> GetClientReferencesByAccountStatus(Enumerators.Stream.GroupType[] groupType, long? branchId,
      params CaseStatus.Type[] caseStatuses);

    Dictionary<long, DateTime> GetClientReferencesAndLoanDateByAccountStatus(
      Enumerators.Stream.GroupType[] groupType, long branchId, params CaseStatus.Type[] caseStatuses);

    List<IGetCaseStreamAction> GetCaseStreamActions(Enumerators.Stream.GroupType groupType, DateTime? completeDate,
      List<DateTime> actionDates, Enumerators.Action.Type[] actionTypes,
      CaseStatus.Type[] caseStatuses, params Enumerators.Stream.StreamType[] streamType);

    IGetCaseStreamAction GetCaseStreamAction(long caseStreamId, DateTime? completeDate,
      DateTime actionDate, Enumerators.Action.Type actionType, Enumerators.Stream.StreamType streamType);

    void IncreaseCaseStreamPriority(long caseStreamId, string userId,
      Enumerators.Stream.PriorityType? priorityType = null);

    void MoveCaseToStream(long caseStreamId, long personId, Enumerators.Stream.StreamType newStream,
      int completedCommentId, string completeNote, bool makeDefaultAction, CaseStatus.Type? newCaseStatusType = null);

    void ChangeCaseStreamAllocatedUser(long caseStreamId, string userId, long currentUserId, long newUserId);
    //List<IStreamAccountNote> GetNotes(long accountId, long? caseId = null);
    void MoveCaseToFollowUpStream(long caseStreamId, string userId, int commentId, string completeNote,
      DateTime actionDate);

    void MoveCaseToPtpStream(long caseStreamId, string userId, int commentId, string completeNote, decimal amount,
      DateTime[] actionDates);

    void MoveCaseToPtcStream(long caseStreamId, string userId, int commentId, string completeNote, DateTime actionDate);
    void MarkEscalationAsComplete(long caseStreamId, string userId, int commentId);
    void MarkEscalationAsComplete(long caseStreamId, string allocatedUserId); 

    /// <summary>
    /// Moves case from PTC to PTC Broken
    /// </summary>
    /// <param name="caseStreamId"></param>
    /// <param name="userId"></param>
    /// <param name="commentId"></param>
    /// <param name="completeNote"></param>
    void MovePtcCaseToPtcBrokenStream(long caseStreamId, string userId, int commentId, string completeNote);

    void MovePtcCaseToComplete(long caseStreamId, string userId, int commentId);
    void PtcClientNotInterested(long caseStreamId, string userId, string completeNote);

    void NoActionOnCaseStream(long caseStreamId, long caseStreamActionId, string userId, int commentId,
      DateTime? actionDate = null);

    bool EscalateCaseStream(long caseStreamId, string userId, int commentId,
      params Enumerators.Stream.EscalationType[] escalationTypes);

    void AddContact(long debtorId, string userId, General.ContactType contactType, string value);
    void ProcessPtpResults(Dictionary<long, Tuple<long, bool?>> ptpResults);
    void BreakOutstandingPtCs(Enumerators.Stream.GroupType group = Enumerators.Stream.GroupType.Sales);
    IDebtor GetDebtorByIdNumber(string idNumber);
    IDebtor GetDebtorById(long debtorId);
    IDebtor GetDebtorByReference(long reference);
    void MarkActionReminderAsComplete(long caseStreamActionId, string userId, int commentId);

    // group specific methods
    void CloseCasesWithoutArrears(General.Host host, Enumerators.Stream.GroupType groupType);

    void CheckAndCloseCaseWithoutArrears(long caseId);

    void ForceCloseCase(long caseStreamId, int commentId);
    void CloseCase(long caseId, int? commentId = null);

    void IncBudget(Enumerators.Stream.BudgetType budgetTypeType, DateTime? enquiryDate = null);
    void DeActivateBudget(int budgetId);

    IBudget CreateBudget(Enumerators.Stream.BudgetType budgetTypeType, DateTime rangeStart, DateTime rangeEnd,
      long maxValue,
      bool isActive = true, bool deActivateCurrent = false, long currentValue = 0);

    bool DoesBudgetAllow(Enumerators.Stream.BudgetType budgetTypeType, DateTime? enquiryDate = null,
      long enquiryValue = 0);

    void RemoveDeceasedClients(List<long> debtorReferences);
  }
}