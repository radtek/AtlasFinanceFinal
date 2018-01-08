using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class AVS_ServiceScheduleBankDTO
  {
    [DataMember]
    public Int32 ServiceScheduleBankId { get; set; }
    [DataMember]
    public AVS_ServiceScheduleDTO ServiceSchedule { get; set; }
    [DataMember]
    public BankDTO Bank { get; set; }
  }
}
