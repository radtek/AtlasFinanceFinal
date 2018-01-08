using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Structures
{
  public class VAP : IVAP
  {
    public string LegacyBranchNumber { get; set; }
    public int PayNo { get; set; }
    public decimal VapLinkedLoansValue { get; set; }
    public int VapLinkedLoans { get; set; }
    public int VapDeniedByConWithAuth { get; set; }
    public int VapDeniedByConWithOutAuth { get; set; }
    public int VapExcludedLoans { get; set; }
  }
}