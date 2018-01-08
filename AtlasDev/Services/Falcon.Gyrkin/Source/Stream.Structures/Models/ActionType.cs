using Stream.Framework.Structures;
using Stream.Framework.Enumerators;

namespace Stream.Structures.Models
{
  public class ActionType : IActionType
  {
    public int ActionTypeId { get; set; }

    public Action.Type Type { get; set; }

    public string Description { get; set; }

  }
}
