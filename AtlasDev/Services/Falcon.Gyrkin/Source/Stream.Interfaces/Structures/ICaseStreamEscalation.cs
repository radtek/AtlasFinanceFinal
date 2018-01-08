using System;

namespace Stream.Framework.Structures
{
  public interface ICaseStreamEscalation
  {
    long CaseStreamEscalationId { get; set; }
    long CaseStreamId { get; set; }
    Enumerators.Stream.EscalationType EscalationType { get; set; }
    DateTime CreateDate { get; set; }
  }
}
