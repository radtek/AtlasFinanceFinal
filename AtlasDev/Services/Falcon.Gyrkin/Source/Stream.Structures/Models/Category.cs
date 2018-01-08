using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class Category : ICategory
  {
    public int CategoryId { get; set; }
    public Framework.Enumerators.Category.Type CategoryType { get; set; }
    public string Description { get; set; }
  }
}