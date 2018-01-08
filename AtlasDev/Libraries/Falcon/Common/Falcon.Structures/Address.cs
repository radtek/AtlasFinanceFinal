using System;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures
{
  public sealed class Address:IAddress
  {
    public long AddressId { get; set; }
    public long AddressTypeId { get; set; }
    public string Line1 { get; set; }
    public string Line2 { get; set; }
    public string Line3 { get; set; }
    public string Line4 { get; set; }
    public string PostalCode { get; set; }
    public bool IsActive { get; set; }

    public IAddressType AddressType { get; set; }
    public IProvince Province { get; set; }
    public IPerson CreatedBy { get; set; }
    public DateTime CreatedDT { get; set; }
  }
}
