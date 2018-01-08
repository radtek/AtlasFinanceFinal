using System;
using Atlas.Enumerators;

namespace Falcon.Common.Interfaces.Structures
{
  public interface IProvince
  {
    Int64 ProvinceId { get; set; }
    General.Province Type { get; set; }
    string ShortCode { get; set; }
    string Description { get; set; }
  }
}
