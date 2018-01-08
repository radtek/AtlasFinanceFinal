using System;
using System.Runtime.Serialization;
using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class AVS_BankAccountPeriodDTO
  { 
    [DataMember]
    public Int32 BankAccountPeriodId { get; set; }
    [DataMember]
    public Enumerators.General.BankPeriod Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.General.BankPeriod>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.General.BankPeriod>();
      }
    }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public int Period { get; set; }
  }
}
