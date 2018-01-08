using System;
using System.Collections.Generic;
using Stream.Framework.Structures;
using Action = Stream.Framework.Enumerators.Action;

namespace Stream.Structures.Models
{
  public sealed class EscalatedCaseStream : IEscalatedCaseStream
  {
    public long CaseStreamActionId { get; set; }
    public long CaseId { get; set; }
    public string AccountReference { get; set; }
    public long CaseStreamId { get; set; }
    public string DebtorFullName { get; set; }
    public string DebtorIdNumber { get; set; }
    public string Priority { get; set; }
    public string CaseStatus { get; set; }
    public DateTime ActionDate { get; set; }
    public Action.Type ActionType { get; set; }
    public DateTime? DateActioned { get; set; }
    public DateTime? CompleteDate { get; set; }
    public bool? IsSuccess { get; set; }
    public decimal? Amount { get; set; }
    public long AllocatedUserId { get; set; }
    public string AllocatedUserFullName { get; set; }
    public bool Danger { get; set; }
    public bool Warning { get; set; }
    public List<ICaseStreamAllocation> CaseStreamAllocations { get; set; }
  }
}