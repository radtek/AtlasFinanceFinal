using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Structures
{
  public class DebtorsBook : IDebtorsBook
  {
    public string LegacyBranchNumber { get; set; }
    public decimal DebtorsBookValue { get; set; }
  }
}
