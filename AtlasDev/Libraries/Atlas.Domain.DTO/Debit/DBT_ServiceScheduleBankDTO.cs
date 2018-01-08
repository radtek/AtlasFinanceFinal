using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class DBT_ServiceScheduleBankDTO
  {
    [DataMember]
    public int ServiceScheduleBankId { get;set;}
    [DataMember]
    public DBT_ServiceScheduleDTO ServiceSchedule { get;set;}
    [DataMember]
    public BankDTO Bank { get;set;}
    [DataMember]
    public bool Enabled { get; set; }
  }
}