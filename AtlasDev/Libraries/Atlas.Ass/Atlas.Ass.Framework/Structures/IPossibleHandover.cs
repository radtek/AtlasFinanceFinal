
namespace Atlas.Ass.Framework.Structures
{
  public interface IPossibleHandover
  {
    string LegacyBranchNumber { get; set; }
    decimal PossibleHandOvers { get; set; }
    decimal NextPossibleHandOvers { get; set; }
  }
}