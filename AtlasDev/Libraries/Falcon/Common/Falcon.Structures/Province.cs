using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures
{
  public class Province:IProvince
  {
    public long ProvinceId { get; set; }
    public string Description { get; set; }
    public General.Province Type { get; set; }
    public string ShortCode { get; set; }
  }
}
