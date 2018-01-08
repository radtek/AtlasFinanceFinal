using System;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures
{
  public class Notes : INote
  {
    public Int64 NoteId { get; set; }
    public INote ParentNote { get; set; }
    public string Note { get; set; }
    public IPerson CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? LastEditDate { get; set; }
    public DateTime? DeleteDate { get; set; }
    public IPerson DeleteUser { get; set; }
  }
}