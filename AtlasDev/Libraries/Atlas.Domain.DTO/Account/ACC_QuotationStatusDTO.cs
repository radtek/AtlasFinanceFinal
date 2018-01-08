using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class ACC_QuotationStatusDTO
  {
    [DataMember]
    public long QuotationStatusId{get;set;}
    [DataMember]
    public Enumerators.Account.QuotationStatus Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.QuotationStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.QuotationStatus>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
