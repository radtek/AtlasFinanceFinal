using System;
using Action = Stream.Framework.Enumerators.Action;

namespace Stream.Framework.Structures
{
  public interface ICaseStreamAction
  {
    long CaseStreamActionId { get; set; }
    long CaseStreamId { get; set; }
    DateTime ActionDate { get; set; }
    Action.Type ActionType { get; set; }
    DateTime? DateActioned { get; set; }
    DateTime? CompleteDate { get; set; }
    bool? IsSuccess { get; set; }
    decimal? Amount { get; set; }

    // added for front to know whether to perform a new credit enquiry
    bool PerformNewCreditEnquiry { get; set; }
  }
}
