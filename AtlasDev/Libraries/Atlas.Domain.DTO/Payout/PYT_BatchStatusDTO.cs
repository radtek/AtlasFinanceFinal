using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class PYT_BatchStatusDTO
  {
    [DataMember]
    public int BatchStatusId { get; set; }
    [DataMember]
    public Enumerators.Payout.PayoutBatchStatus Status
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Payout.PayoutBatchStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Payout.PayoutBatchStatus>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
