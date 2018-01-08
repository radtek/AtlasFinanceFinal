using Atlas.Common.Extensions;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public sealed class StreamPriority : IStreamPriority
  {
    public int PriorityId { get; set; }
    public Framework.Enumerators.Stream.PriorityType PriorityType
    {
      get
      {
        return Description.FromStringToEnum<Framework.Enumerators.Stream.PriorityType>();
      }
      set
      {
        value = Description.FromStringToEnum<Framework.Enumerators.Stream.PriorityType>();
      }
    }

    public string Description { get; set; }
    public int Value { get; set; }
  }
}