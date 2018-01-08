namespace Atlas.Ass.Framework.Structures
{
  public interface IRolledAccounts
  {
    string LegacyBranchNumber { get; set; }
    int PayNo { get; set; }
    decimal RollbackValue { get; set; }
  }
}
