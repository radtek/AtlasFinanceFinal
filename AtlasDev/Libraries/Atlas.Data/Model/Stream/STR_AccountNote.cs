using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class STR_AccountNote : XPLiteObject
  {
    private Int64 _accountNoteId;
    [Key(AutoGenerate = true)]
    public Int64 AccountNoteId
    {
      get
      {
        return _accountNoteId;
      }
      set
      {
        SetPropertyValue("AccountNoteId", ref _accountNoteId, value);
      }
    }

    private STR_Account _account;
    [Persistent("AccountId")]
    [Indexed]
    public STR_Account Account
    {
      get
      {
        return _account;
      }
      set
      {
        SetPropertyValue("Account", ref _account, value);
      }
    }

    private NTE_Note _note;
    [Persistent("NoteId")]
    [Indexed]
    public NTE_Note Note
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

    public STR_AccountNote() : base() { }
    public STR_AccountNote(Session session) : base(session) { }

    #endregion

  }
}