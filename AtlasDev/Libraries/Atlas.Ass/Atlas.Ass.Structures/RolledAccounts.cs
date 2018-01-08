using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Structures
{
  public class RolledAccounts : IRolledAccounts
  {
    public string LegacyBranchNumber { get; set; }
    public int PayNo { get; set; }
    public decimal RollbackValue { get; set; }
  }
}
