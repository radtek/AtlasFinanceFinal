using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures
{
  public class AddressType:IAddressType
  {
    public long AddressTypeId { get; set; }
    public string Description { get; set; }
    public General.AddressType Type { get; set; }
  }
}
