using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Structures
{
  public class ClientLoanInfo : IClientLoanInfo
  {
    public string LegacyBranchNumber { get; set; }
    public int PayNo { get; set; }
    public int Quantity { get; set; }
    public decimal Cheque { get; set; }
    public int NewClientQuantity { get; set; }
    public decimal NewClientAmount { get; set; }
    public int ExistingClientCount { get; set; }
    public int RevivedClientCount { get; set; }
    public decimal RevivedClientAmount { get; set; }
  }
}
