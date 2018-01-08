using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Structures
{
  public class LoansFlagged : ILoansFlagged
  {
    public string LegacyBranchNumber { get; set; }
    public decimal OverdueValue { get; set; }
    public int NoOfLoans { get; set; }
  }
}