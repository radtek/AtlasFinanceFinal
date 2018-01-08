using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class LGR_FeeRangeTypeDTO
  {
    [DataMember]
    public Int64 FeeRangeTypeId { get; set; }
    [DataMember]
    public Enumerators.GeneralLedger.FeeRangeType Type
    {
      get { return Description.FromStringToEnum<Enumerators.GeneralLedger.FeeRangeType>(); }
      set { value = Description.FromStringToEnum<Enumerators.GeneralLedger.FeeRangeType>(); }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
