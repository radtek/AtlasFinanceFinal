using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_PeriodFrequencyDTO
  {
    [DataMember]
    public Int32 PeriodFrequencyId { get; set; }
    [DataMember]
    public HostDTO Host { get; set; }
    [DataMember]
    public Enumerators.Account.PeriodFrequency Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.PeriodFrequency>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.PeriodFrequency>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
