using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Structures
{
  public class Arrears : IArrears
  {
    public string LegacyBranchNumber { get; set; }
    public decimal ArrearsValue { get; set; }
  }
}
