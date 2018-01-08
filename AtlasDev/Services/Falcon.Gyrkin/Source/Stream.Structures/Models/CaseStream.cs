using System;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public sealed class CaseStream : ICaseStream
  {
    public long CaseStreamId { get; set; }
    public long CaseId { get; set; }
    public Framework.Enumerators.Stream.EscalationType EscalationType { get; set; }
    public Framework.Enumerators.Stream.StreamType StreamType { get; set; }
    public Framework.Enumerators.Stream.PriorityType PriorityType { get; set; }
    public DateTime LastPriorityDate { get; set; }
    public long CompletedUserId { get; set; }
    public string CompletedUser { get; set; }
    public string CompleteComment { get; set; }
    public DateTime? CompleteDate { get; set; }
    public string CompleteNote { get; set; }
    public long CreateUserId { get; set; }
    public string CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
  }
}