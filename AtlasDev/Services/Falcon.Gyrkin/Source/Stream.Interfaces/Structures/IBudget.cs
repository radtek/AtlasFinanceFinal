using System;

namespace Stream.Framework.Structures
{
  public interface IBudget
  {
    int BudgetId { get; set; }
    Enumerators.Stream.BudgetType BudgetType { get; set; }
    string Description { get; set; }
    DateTime RangeStart { get; set; }
    DateTime RangeEnd { get; set; }
    long MaxValue { get; set; }
    long CurrentValue { get; set; }
    bool IsActive { get; set; }
    DateTime CreateDate { get; set; }
  }
}
