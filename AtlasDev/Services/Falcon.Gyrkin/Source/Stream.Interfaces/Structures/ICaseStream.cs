using System;

namespace Stream.Framework.Structures
{
  public interface ICaseStream
  {
    long CaseStreamId { get; set; }
    long CaseId { get; set; }
    Enumerators.Stream.EscalationType EscalationType { get; set; }
    Enumerators.Stream.StreamType StreamType { get; set; }
    Enumerators.Stream.PriorityType PriorityType { get; set; }
    DateTime LastPriorityDate { get; set; }
    long CompletedUserId { get; set; }
    string CompletedUser { get; set; }
    string CompleteComment { get; set; }
    DateTime? CompleteDate { get; set; }
    string CompleteNote { get; set; }
    long CreateUserId { get; set; }
    string CreateUser { get; set; }
    DateTime CreateDate { get; set; }
    //List<ICaseStreamAllocation> CaseStreamAllocations { get; set; }
  }
}