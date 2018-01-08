namespace Atlas.Ass.Framework.Structures
{
  public interface IBudget
  {
    string LegacyBranchNumber { get; set; }
    decimal HandoverBudget { get; set; }
    decimal ArrearTarget { get; set; }
    string TargetMonth { get; set; }
    string TargetYear { get; set; }
  }
}
