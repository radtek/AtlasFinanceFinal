using System;

namespace Falcon.Common.Interfaces.Structures
{
  public interface IAddress
  {
     Int64 AddressId { get; set; }
     IAddressType AddressType { get; set; }
     string Line1 { get; set; }
     string Line2 { get; set; }
     string Line3 { get; set; }
     string Line4 { get; set; }
     IProvince Province { get; set; }
     string PostalCode { get; set; }
     bool IsActive { get; set; }
     IPerson CreatedBy { get; set; }
     DateTime CreatedDT { get; set; }
  }
}
