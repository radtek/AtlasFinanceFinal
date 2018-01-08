namespace Atlas.Ass.Framework.Structures
{
  public interface IReswipes
  {
    string LegacyBranchNumber { get; set; }
    int PayNo { get; set; }
    int BankChange { get; set; }
    int LoanTermChange { get; set; }
    int InstalmentChange { get; set; }
  }
}
