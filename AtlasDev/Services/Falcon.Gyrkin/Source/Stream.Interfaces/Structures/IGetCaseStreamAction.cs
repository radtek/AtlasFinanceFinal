using System;
using System.Collections.Generic;

namespace Stream.Framework.Structures
{
  public interface IGetCaseStreamAction
  {
    ICollection<long> AccountReference2 { get; set; }
    long CaseStreamActionId { get; set; }
    long CaseStreamId { get; set; }
    decimal? Amount { get; set; }
    DateTime ActionDate { get; set; }
    Enumerators.Action.Type ActionType { get; set; }
    DateTime? CompleteDate { get; set; }
    Enumerators.Stream.StreamType StreamType { get; set; }
  }
}

