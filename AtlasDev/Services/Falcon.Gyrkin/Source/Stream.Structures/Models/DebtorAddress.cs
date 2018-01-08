using System;
using Atlas.Enumerators;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class DebtorAddress : IDebtorAddress
  {
    public long DebtorAddressId { get; set; }
    public long DebtorId { get; set; }
    public General.AddressType AddressType { get; set; }
    public string Line1 { get; set; }
    public string Line2 { get; set; }
    public string Line3 { get; set; }
    public string Line4 { get; set; }
    public General.Province? Province { get; set; }
    public string PostalCode { get; set; }
    public bool IsActive { get; set; }
    public string CreateUser { get; set; }
    public long CreateUserId { get; set; }
    public DateTime CreateDate { get; set; }
  }
}
