using System;

namespace Stream.Framework.Structures
{
  public interface IDebtorAddress
  {
    long DebtorAddressId { get; set; }
    long DebtorId { get; set; }
    Atlas.Enumerators.General.AddressType AddressType { get; set; }
    string Line1 { get; set; }
    string Line2 { get; set; }
    string Line3 { get; set; }
    string Line4 { get; set; }
    Atlas.Enumerators.General.Province? Province { get; set; }
    string PostalCode { get; set; }
    bool IsActive { get; set; }
    string CreateUser { get; set; }
    long CreateUserId { get; set; }
    DateTime CreateDate { get; set; }
  }
}
