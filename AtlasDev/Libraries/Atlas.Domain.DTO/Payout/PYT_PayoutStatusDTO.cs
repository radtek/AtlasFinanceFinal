using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class PYT_PayoutStatusDTO
  {
    [DataMember]
    public int PayoutStatusId { get; set; }
    [DataMember]
    public Enumerators.Payout.PayoutStatus Status
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Payout.PayoutStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Payout.PayoutStatus>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}