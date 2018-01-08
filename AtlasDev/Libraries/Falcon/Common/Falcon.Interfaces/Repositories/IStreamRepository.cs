using System;
using System.Collections.Generic;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;
using Falcon.Common.Interfaces.Structures.Stream;

namespace Falcon.Common.Interfaces.Repositories
{
  public interface IStreamRepository
  {
    Tuple<List<INote>, string> GetNoteHistory(long caseId);
    List<IAccountStreamAction> GetAccounts(long[] caseIds);
    void AddActuator(Stream.ActuatorType actuatorType, long? branchId, long? regionId, DateTime rangeStart, DateTime rangeEnd,
       bool isActive = true, bool disableOverlappingActuators = false);
    void EscalateUnworkedCases();
    byte[] GetFinalLetterOfDemandPdf(long caseStreamId);
    IEnumerable<Stream.CaseStatus> GetCaseStatuses();
    List<IStream> GetStreamTypes(Stream.GroupType groupType);
    List<IComment> GetCommentsByStreamGroup(Stream.GroupType groupType);
    List<IAccountStreamAction> GetWorkItems(Stream.GroupType[] groupTypes, Stream.CaseStatus?[] caseStatuses = null, long? branchId = null, long? allocatedPersonId = null, Guid? allocatedUserId = null, TimeSpan? bufferTime = null, DateTime? completeDate = null, long caseId = 0, string idNumber = null, string accountReference = null, DateTime? actionDateStartRange = null, DateTime? actionDateEndRange = null, int limitResults = 100, params Stream.StreamType[] streamTypes);
    List<ICaseStreamAction> GetEscalatedWorkItemsOnly(Guid allocatedUserId, long branchId, int limitResults = 100);
    Tuple<IWorkItem, List<IStreamAccountNote>, string> GetNextWorkItem(Stream.GroupType groupType, Guid userId, Stream.StreamType streamType);
    Tuple<IWorkItem, List<IStreamAccountNote>, string> GetWorkItem(long caseStreamActionId, Guid userId);
    Dictionary<Guid, List<Tuple<IStream, int>>> GetWorkItemsSummary(Stream.GroupType groupType, List<Guid> userIds, params Stream.StreamType[] streamTypes);
    void AddAccountNote(long accountId, Guid userId, string note, Stream.AccountNoteType accountNoteType = Stream.AccountNoteType.Normal, long? caseId = null);
    bool AddNoteComment(long accountNoteId, Guid userId, string note);
    List<INote> GetNoteComments(long accountNoteId);
    void AddCaseStreamActionWithPriority(long caseStreamId, Guid userId, DateTime actionDate, Stream.ActionType actionType, bool allowMultipleActionTypes = true, bool completePendingActions = true, Stream.PriorityType? prirotyType = null);
    void AddCaseStreamAction(long caseStreamId, DateTime actionDate, Stream.ActionType actionType, bool allowMultipleActionTypes = true, bool completePendingActions = true);
    Dictionary<long, string> GetAccountReferenceByStatus(Stream.GroupType[] groupType, params Stream.CaseStatus[] caseStatuses);
    //List<IStreamAccount> GetAccountsByStatus(Atlas.Enumerators.Stream.GroupType[] groupType, params Atlas.Enumerators.Stream.CaseStatus[] caseStatuses);
    List<long> GetClientReferencesByAccountStatus(Stream.GroupType[] groupType, long? branchId, params Stream.CaseStatus[] caseStatuses);
    Dictionary<long, DateTime> GetClientReferencesAndLoanDateByAccountStatus(Stream.GroupType[] groupType, long branchId, params Stream.CaseStatus[] caseStatuses);
    List<IGetCaseStreamAction> GetCaseStreamActions(Stream.GroupType groupType, DateTime? completeDate, List<DateTime> actionDates, Stream.ActionType[] actionTypes, Stream.CaseStatus[] caseStatuses, params Stream.StreamType[] streamType);
    void IncreaseCaseStreamPriority(long caseStreamId, Guid userId, Stream.PriorityType? priorityType = null);
    void MoveCaseToStream(long caseStreamId, long personId, Stream.StreamType newStream, int completedCommentId, string completeNote, bool cancelPendingActions, bool makeDefaultAction, bool returnIfPendingActionsExist = true, Stream.CaseStatus? newCaseStatus = null);
    void ChangeCaseStreamAllocatedUser(long caseStreamId, Guid userId, long newUserId);
    //List<IStreamAccountNote> GetNotes(long accountId, long? caseId = null);
    void MoveCaseToFollowUpStream(long caseStreamId, Guid userId, int commentId, string completeNote, DateTime actionDate);
    void MoveCaseToPtpStream(long caseStreamId, Guid userId, int commentId, string completeNote, decimal amount, DateTime[] actionDates);
    void MoveCaseToPtcStream(long caseStreamId, Guid userId, int commentId, string completeNote, DateTime actionDate);
    void MarkEscalationAsComplete(long caseStreamId, Guid userId, int commentId);

    /// <summary>
    /// Moves case from PTC to PTC Broken
    /// </summary>
    /// <param name="caseStreamId"></param>
    /// <param name="userId"></param>
    /// <param name="commentId"></param>
    /// <param name="completeNote"></param>
    void MovePtcCaseToPtcBrokenStream(long caseStreamId, Guid userId, int commentId, string completeNote);
    void MovePtcCaseToComplete(long caseStreamId, Guid userId, int commentId);
    void PtcClientNotInterested(long caseStreamId, Guid userId, string completeNote);
    void NoActionOnCaseStream(long caseStreamId, long caseStreamActionId, Guid userId, int commentId, DateTime? actionDate = null);
    bool ForceCheckPtpPayment(long caseStreamId);
    bool EscalateCaseStream(long caseStreamId, Guid userId, int commentId, params Stream.EscalationType[] escalationTypes);
    void AddContact(long debtorId, Guid userId, General.ContactType contactType, string value);
    void ProcessPtpResults(Dictionary<long, Tuple<long, bool?>> ptpResults);
    void BreakOutstandingPtCs(Stream.GroupType group = Stream.GroupType.Sales);
    IDebtor GetDebtorById(long debtorId);
    void MarkActionReminderAsComplete(long caseStreamActionId, Guid userId, int commentId);

    // group specific methods
    void CloseAccountsWithoutArrears();

    void IncBudget(Stream.Budget budgetType, DateTime? enquiryDate = null);
    void DeActivateBudget(int budgetId);
    IBudget CreateBudget(Stream.Budget budgetType, DateTime rangeStart, DateTime rangeEnd, long maxValue, bool isActive = true, bool deActivateCurrent = false, long currentValue = 0);
    bool DoesBudgetAllow(Stream.Budget budgetType, DateTime? enquiryDate = null, long enquiryValue = 0);
    void RemoveDeceasedClients(List<long> debtorReferences);
  }
}