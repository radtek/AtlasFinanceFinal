using Stream.Framework.Enumerators;

namespace Stream.Framework.Structures
{
  public interface ICategory
  {
    int CategoryId { get; set; }
    Category.Type CategoryType { get; set; }
    string Description { get; set; }
  }
}
