using System;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class Budget : IBudget
  {
    public int BudgetId { get; set; }
    public Framework.Enumerators.Stream.BudgetType BudgetType { get; set; }
    public string Description { get; set; }
    public DateTime RangeStart { get; set; }
    public DateTime RangeEnd { get; set; }
    public long MaxValue { get; set; }
    public long CurrentValue { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreateDate { get; set; }
  }
}