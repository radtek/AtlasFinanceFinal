using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class ACC_ScoreResultDTO
  {
    [DataMember]
    public Int64 ScoreResultId { get; set; }
    [DataMember]
    public Enumerators.Account.ScoreResult Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.ScoreResult>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.ScoreResult>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
