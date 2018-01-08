using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class ACC_AccountNote : XPLiteObject
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

    private ACC_Account _account;
    [Persistent("AccountId")]
    public ACC_Account Account
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

    #region Constructors

    public ACC_AccountNote() : base() { }
    public ACC_AccountNote(Session session) : base(session) { }

    #endregion
  }
}
