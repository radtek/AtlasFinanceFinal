using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class PYT_ServiceTypeDTO
  {
    [DataMember]
    public int ServiceTypeId { get; set; }
    [DataMember]
    public Enumerators.Payout.PayoutServiceType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Payout.PayoutServiceType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Payout.PayoutServiceType>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
