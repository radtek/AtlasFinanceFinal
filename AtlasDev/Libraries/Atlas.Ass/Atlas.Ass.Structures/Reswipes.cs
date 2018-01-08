using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Structures
{
  public class Reswipes : IReswipes
  {
    public string LegacyBranchNumber { get; set; }
    public int PayNo { get; set; }
    public int BankChange { get; set; }
    public int LoanTermChange { get; set; }
    public int InstalmentChange { get; set; }
  }
}
