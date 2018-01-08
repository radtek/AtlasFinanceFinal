using System;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public sealed class CaseStreamEscalation : ICaseStreamEscalation
  {
    public long CaseStreamEscalationId { get; set; }
    public long CaseStreamId { get; set; }
    public Framework.Enumerators.Stream.EscalationType EscalationType { get; set; }
    public DateTime CreateDate { get; set; }
  }
}
