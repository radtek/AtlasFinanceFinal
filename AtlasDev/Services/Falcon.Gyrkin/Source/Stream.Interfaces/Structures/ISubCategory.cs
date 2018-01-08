using System;

namespace Stream.Framework.Structures
{
  public interface ISubCategory
  {
    int SubCategoryId { get; set; }
    ICategory Category { get; set; }
    Enumerators.Stream.SubCategory SubCategoryType { get; set; }
    string Description { get; set; }
    DateTime? DisableDate { get; set; }
  }
}
