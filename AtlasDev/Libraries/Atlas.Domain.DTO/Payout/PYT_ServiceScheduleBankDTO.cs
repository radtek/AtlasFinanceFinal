using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class PYT_ServiceScheduleBankDTO
  {
    [DataMember]
    public int ServiceScheduleBankId { get; set; }
    [DataMember]
    public PYT_ServiceScheduleDTO ServiceSchedule { get; set; }
    [DataMember]
    public BankDTO Bank { get; set; }
  }
}
