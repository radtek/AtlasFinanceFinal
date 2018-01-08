using System;
using System.Collections.Generic;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class GetCaseStreamAction : IGetCaseStreamAction
  {
    public ICollection<long> AccountReference2 { get; set; }
    public long CaseStreamActionId { get; set; }
    public long CaseStreamId { get; set; }
    public decimal? Amount { get; set; }
    public DateTime ActionDate { get; set; }
    public Framework.Enumerators.Action.Type ActionType { get; set; }
    public DateTime? CompleteDate { get; set; }
    public Framework.Enumerators.Stream.StreamType StreamType { get; set; }
  }
}
