using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_History: XPLiteObject
  {
    private Int64 _historyId;
    [Key(AutoGenerate = true)]
    public Int64 HistoryId
    {
      get
      {
        return _historyId;
      }
      set
      {
        SetPropertyValue("HistoryId", ref _historyId, value);
      }
    }

    private ACC_Account _account;
    [Persistent("AccountId")]
    [Association]
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

    private string _entry;
    public string Entry
    {
      get
      {
        return _entry;
      }
      set
      {
        SetPropertyValue("Entry", ref _entry, value);
      }
    }

    private DateTime _createdDate;
    [Persistent]
    public DateTime CreatedDate
    {
      get
      {
        return _createdDate;
      }
      set
      {
        SetPropertyValue("CreatedDate", ref _createdDate, value);
      }
    }


    #region Constructors

    public ACC_History() : base() { }
    public ACC_History(Session session) : base(session) { }

    #endregion
  }
}
