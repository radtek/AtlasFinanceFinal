using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class PYT_ValidationDTO
  {
    [DataMember]
    public int ValidationId { get; set; }
    [DataMember]
    public Enumerators.Payout.Validation Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Payout.Validation>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Payout.Validation>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
