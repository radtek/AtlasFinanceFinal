using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Structures
{
  public class QryCollections : IQryCollections
  {
    public string LegacyBranchNumber { get; set; }
    public decimal Receivable{ get; set; }
    public decimal Received{ get; set; }
  }
}