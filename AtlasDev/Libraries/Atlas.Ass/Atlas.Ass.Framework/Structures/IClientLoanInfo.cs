namespace Atlas.Ass.Framework.Structures
{
  public interface IClientLoanInfo
  {
    string LegacyBranchNumber { get; set; }
    int PayNo { get; set; }
    int Quantity { get; set; }
    decimal Cheque { get; set; }
    int NewClientQuantity { get; set; }
    decimal NewClientAmount { get; set; }
    int ExistingClientCount { get; set; }
    int RevivedClientCount { get; set; }
    decimal RevivedClientAmount { get; set; }
  }
}
