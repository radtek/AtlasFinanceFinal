using System;
using System.Runtime.Serialization;
using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class ProvinceDTO
  {
    [DataMember]
    public Int64 ProvinceId { get; set; }
    [DataMember]
    public Enumerators.General.Province Type
    {
      get { return ShortCode.FromStringToEnum<Enumerators.General.Province>(); }
      set { value = ShortCode.FromStringToEnum<Enumerators.General.Province>(); }
    }
    [DataMember]
    public string ShortCode { get; set; }
    [DataMember]
    public string Description { get; set; }
  }
}
