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
  public class STR_CommentDTO
  {
    [DataMember]
    public int CommentId { get; set; }
    [DataMember]
    public STR_CommentGroupDTO CommentGroup { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public DateTime? DisableDate { get; set; }
  }
}