using Stream.Framework.Enumerators;

namespace Stream.Framework.Structures
{
  public interface IActionType
  {
    int ActionTypeId { get; set; }
    Action.Type Type { get; set; }
    string Description { get; set; }
  }
}
