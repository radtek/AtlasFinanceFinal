
using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Structures
{
  public class PossibleHandover : IPossibleHandover
  {
    public string LegacyBranchNumber { get; set; }
    public decimal PossibleHandOvers { get; set; }
    public decimal NextPossibleHandOvers { get; set; }
  }
}