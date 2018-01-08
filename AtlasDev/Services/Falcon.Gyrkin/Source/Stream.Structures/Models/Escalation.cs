using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public sealed class Escalation : IEscalation
  {
    public int EscalationId { get; set; }
    public Framework.Enumerators.Stream.EscalationType EscalationType { get; set; }
    public string Description { get; set; }
  }
}
