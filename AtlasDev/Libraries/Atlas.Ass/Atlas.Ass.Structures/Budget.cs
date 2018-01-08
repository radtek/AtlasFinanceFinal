using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Structures
{
  public class Budget : IBudget
  {
    public string LegacyBranchNumber { get; set; }
    public decimal HandoverBudget { get; set; }
    public decimal ArrearTarget { get; set; }
    public string TargetMonth { get; set; }
    public string TargetYear { get; set; }
  }
}
