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
  public class STR_AccountNoteDTO
  {
    [DataMember]
    public Int64 AccountNoteId { get; set; }
    [DataMember]
    public STR_AccountDTO Account { get; set; }
    [DataMember]
    public NTE_NoteDTO Note { get; set; }
    [DataMember]
    public STR_CaseDTO Case { get; set; }
    [DataMember]
    public STR_AccountNoteTypeDTO AccountNoteType { get; set; }
    [DataMember]
    public PER_PersonDTO CreateUser { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}