using Atlas.Enumerators;

namespace Stream.Framework.DataContracts.Requests
{
  public class AddOrUpdateDebtorAddressRequest
  {
    public long DebtorAddressId { get; set; }
    public long DebtorId { get; set; }
    public General.AddressType AddressType { get; set; }
    public string Line1 { get; set; }
    public string Line2 { get; set; }
    public string Line3 { get; set; }
    public string Line4 { get; set; }
    public General.Province Province { get; set; }
    public string PostalCode { get; set; }
    public bool IsActive { get; set; }
    public long CreateUserId { get; set; }
  }
}
