using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class DBT_HostServiceDTO
  {
    public int HostServiceId { get; set; }
    public HostDTO Host { get; set; }
    public DBT_ServiceDTO Service { get; set; }
    public bool Enabled { get; set; }
    public DateTime? DisabledDate { get; set; }
  }
}