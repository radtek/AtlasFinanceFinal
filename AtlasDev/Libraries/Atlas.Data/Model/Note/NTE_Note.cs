using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class NTE_Note : XPLiteObject
  {
    private Int64 _noteId;
    [Key(AutoGenerate = true)]
    public Int64 NoteId
    {
      get
      {
        return _noteId;
      }
      set
      {
        SetPropertyValue("NoteId", ref _noteId, value);
      }
    }

    private NTE_Note _parentNote;
    [Persistent("ParentNoteId")]
    public NTE_Note ParentNote
    {
      get
      {
        return _parentNote;
      }
      set
      {
        SetPropertyValue("ParentNote", ref _parentNote, value);
      }
    }

    private string _note;
    [Persistent, Size(int.MaxValue)]
    public string Note
    {
      get
      {
        return _note;
      }
      set
      {
        SetPropertyValue("Note", ref _note, value);
      }
    }

    private DateTime _createDate;
    [Persistent]
    public DateTime CreateDate
    {
      get
      {
        return _createDate;
      }
      set
      {
        SetPropertyValue("CreateDate", ref _createDate, value);
      }
    }

    private PER_Person _createUser;
    [Persistent("CreateUserId")]
    public PER_Person CreateUser
    {
      get
      {
        return _createUser;
      }
      set
      {
        SetPropertyValue("CreateUser", ref _createUser, value);
      }
    }

    private DateTime? _lastEditDate;
    [Persistent]
    public DateTime? LastEditDate
    {
      get
      {
        return _lastEditDate;
      }
      set
      {
        SetPropertyValue("LastEditDate", ref _lastEditDate, value);
      }
    }

    private DateTime? _deleteDate;
    [Persistent]
    public DateTime? DeleteDate
    {
      get
      {
        return _deleteDate;
      }
      set
      {
        SetPropertyValue("DeleteDate", ref _deleteDate, value);
      }
    }

    private PER_Person _deleteUser;
    [Persistent("DeleteUserId")]
    public PER_Person DeleteUser
    {
      get
      {
        return _deleteUser;
      }
      set
      {
        SetPropertyValue("DeleteUser", ref _deleteUser, value);
      }
    }

    #region Constructors

    public NTE_Note() : base() { }
    public NTE_Note(Session session) : base(session) { }

    #endregion
  }
}
