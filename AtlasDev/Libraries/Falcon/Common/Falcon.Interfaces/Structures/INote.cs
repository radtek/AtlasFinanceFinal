using System;

namespace Falcon.Common.Interfaces.Structures
{
  public interface INote
  {
    Int64 NoteId { get; set; }
    INote ParentNote { get; set; }
    string Note { get; set; }
    DateTime CreateDate { get; set; }
    IPerson CreateUser { get; set; }
    DateTime? LastEditDate { get; set; }
    DateTime? DeleteDate { get; set; }
    IPerson DeleteUser { get; set; }
  }
}