using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class Frequency : IFrequency
  {
    public int FrequencyId { get; set; }
    public Framework.Enumerators.Stream.FrequencyType Type { get; set; }
    public string Description { get; set; }
  }
}
