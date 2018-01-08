namespace Atlas.Ass.Framework.Structures
{
  public interface IVAP
  {
    string LegacyBranchNumber { get; set; }
    int PayNo { get; set; }
    decimal VapLinkedLoansValue { get; set; }
    int VapLinkedLoans { get; set; }
    int VapDeniedByConWithAuth { get; set; }
    int VapDeniedByConWithOutAuth { get; set; }
    int VapExcludedLoans { get; set; }
  }
}