using System;
using Action = Stream.Framework.Enumerators.Action;

namespace Stream.Framework.DataContracts.Requests
{
  public class AddOrUpdateCaseStreamActionRequest
  {
    public long CaseStreamActionId { get; set; }
    public long CaseStreamId { get; set; }
    public DateTime ActionDate { get; set; }
    public Action.Type ActionType { get; set; }
    public DateTime? DateActioned { get; set; }
    public DateTime? CompleteDate { get; set; }
    public bool? IsSuccess { get; set; }
    public decimal? Amount { get; set; }
  }
}
