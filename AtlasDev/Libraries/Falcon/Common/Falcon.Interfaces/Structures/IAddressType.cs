using System;
using Atlas.Enumerators;

namespace Falcon.Common.Interfaces.Structures
{
  public interface IAddressType
  {
    Int64 AddressTypeId { get; set; }
    General.AddressType Type { get; set; }
    string Description { get; set; }
  }
}
