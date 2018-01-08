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
  public class STR_CommentGroupDTO
  {
    [DataMember]
    public int CommentGroupId { get; set; }
    [DataMember]
    public STR_GroupDTO Group { get; set; }
    public Enumerators.Stream.CommentGroupType CommentGroupType
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.CommentGroupType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.CommentGroupType>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}