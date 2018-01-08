using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class LGR_TransactionTypeDTO
  {
    [DataMember]
    public int TransactionTypeId { get; set; }
    [DataMember]
    public Enumerators.GeneralLedger.TransactionType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.GeneralLedger.TransactionType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.GeneralLedger.TransactionType>();
      }
    }
    [DataMember]
    public LGR_TransactionTypeGroupDTO TransactionTypeGroup { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public int SortKey { get; set; }
  }
}
