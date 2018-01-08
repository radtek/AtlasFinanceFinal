using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_AccountNoteDTO
  {
    [DataMember]
    public Int64 AccountNoteId { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public NTE_NoteDTO Note { get; set; }
  }
}
