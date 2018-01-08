using System;
using Stream.Framework.Structures;
using Action = Stream.Framework.Enumerators.Action;

namespace Stream.Structures.Models
{
  public sealed class CaseStreamAction : ICaseStreamAction
  {
    public long CaseStreamActionId { get; set; }
    public long CaseStreamId { get; set; }
    public DateTime ActionDate { get; set; }
    public Action.Type ActionType { get; set; }
    public DateTime? DateActioned { get; set; }
    public DateTime? CompleteDate { get; set; }
    public bool? IsSuccess { get; set; }
    public decimal? Amount { get; set; }
    public bool PerformNewCreditEnquiry { get; set; }
  }
}