using System;
using Atlas.Domain.Model;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  public class STR_Note : XPLiteObject
  {
    private long _noteId;
    [Key(AutoGenerate = true)]
    public long NoteId
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

    private STR_Case _case;
    [Persistent("CaseId")]
    [Indexed]
    public STR_Case Case
    {
      get
      {
        return _case;
      }
      set
      {
        SetPropertyValue("Case", ref _case, value);
      }
    }

    private STR_AccountNoteType _accountNoteType;
    [Persistent("AccountNoteTypeId")]
    public STR_AccountNoteType AccountNoteType
    {
      get
      {
        return _accountNoteType;
      }
      set
      {
        SetPropertyValue("AccountNoteType", ref _accountNoteType, value);
      }
    }

    private PER_Person _createUser;
    [Persistent("CreateUserId")]
    [Indexed]
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


    #region Constructors

    public STR_Note()
    { }
    public STR_Note(Session session) : base(session) { }

    #endregion

  }
}