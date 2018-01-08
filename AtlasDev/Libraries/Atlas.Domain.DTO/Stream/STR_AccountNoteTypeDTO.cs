using Atlas.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class STR_AccountNoteTypeDTO
  {
    [DataMember]
    public int AccountNoteTypeId { get; set; }
    [DataMember]
    public Enumerators.Stream.AccountNoteType AccountNoteType
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.AccountNoteType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.AccountNoteType>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}