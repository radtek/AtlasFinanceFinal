using System;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class SubCategory : ISubCategory
  {
    public int SubCategoryId { get; set; }
    public ICategory Category { get; set; }
    public Framework.Enumerators.Stream.SubCategory SubCategoryType { get; set; }
    public string Description { get; set; }
    public DateTime? DisableDate { get; set; }
  }
}