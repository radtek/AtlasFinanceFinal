namespace Stream.Framework.Structures
{
  public interface IEscalation
  {
    int EscalationId { get; set; }
    Enumerators.Stream.EscalationType EscalationType { get; set; }
    string Description { get; set; }
  }
}
