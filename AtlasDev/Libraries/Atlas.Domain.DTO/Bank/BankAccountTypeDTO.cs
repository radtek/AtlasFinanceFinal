using System;
using System.Runtime.Serialization;
using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class BankAccountTypeDTO
  {
    [DataMember]
    public Int64 AccountTypeId { get; set; }
    [DataMember]
    public Enumerators.General.BankAccountType Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.BankAccountType>(); }
      set { value = Description.FromStringToEnum<Enumerators.General.BankAccountType>(); }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
