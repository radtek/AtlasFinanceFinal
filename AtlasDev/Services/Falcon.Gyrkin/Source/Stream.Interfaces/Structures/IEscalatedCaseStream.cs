using System;
using System.Collections.Generic;
using Action = Stream.Framework.Enumerators.Action;

namespace Stream.Framework.Structures
{
  public interface IEscalatedCaseStream
  {
    long CaseStreamActionId { get; set; }
    long CaseId { get; set; }
    string AccountReference { get; set; }
    long CaseStreamId { get; set; }
    string DebtorFullName { get; set; }
    string DebtorIdNumber { get; set; }
    string Priority { get; set; }
    string CaseStatus { get; set; }
    DateTime ActionDate { get; set; }
    Action.Type ActionType { get; set; }
    DateTime? DateActioned { get; set; }
    DateTime? CompleteDate { get; set; }
    bool? IsSuccess { get; set; }
    decimal? Amount { get; set; }
    long AllocatedUserId { get; set; }
    string AllocatedUserFullName { get; set; }
    bool Danger { get; set; }
    bool Warning { get; set; }
    List<ICaseStreamAllocation> CaseStreamAllocations { get; set; }
  }
}
