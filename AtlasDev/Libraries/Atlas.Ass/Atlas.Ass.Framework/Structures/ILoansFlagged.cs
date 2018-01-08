namespace Atlas.Ass.Framework.Structures
{
  public interface ILoansFlagged
  {
    string LegacyBranchNumber { get; set; }
    decimal OverdueValue { get; set; }
    int NoOfLoans { get; set; }
  }
}