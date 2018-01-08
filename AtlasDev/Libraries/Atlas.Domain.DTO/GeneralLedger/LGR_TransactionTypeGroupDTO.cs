using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class LGR_TransactionTypeGroupDTO
  {
    [DataMember]
    public int TransactionTypeGroupId { get; set; }
    [DataMember]
    public Enumerators.GeneralLedger.TransactionTypeGroup TType
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.GeneralLedger.TransactionTypeGroup>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.GeneralLedger.TransactionTypeGroup>();
      }
    }
    [DataMember]
    public LGR_TypeDTO Type { get; set; }
    [DataMember]
    public string Description { get; set; }
  }
}
