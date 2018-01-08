using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference =  true)]
  public class NTE_NoteDTO
  {
    [DataMember]
    public Int64 NoteId { get; set; }
    [DataMember]
    public NTE_NoteDTO ParentNote { get; set; }
    [DataMember]
    public string Note { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
    [DataMember]
    public PER_PersonDTO CreateUser { get; set; }
    [DataMember]
    public DateTime? LastEditDate { get; set; }
    [DataMember]
    public DateTime? DeleteDate { get; set; }
    [DataMember]
    public PER_PersonDTO DeleteUser { get; set; }
  }
}