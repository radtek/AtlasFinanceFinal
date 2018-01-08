using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class ACC_AccountStatus : XPLiteObject
  {
    private Int64 _accountStatusId;
    [Key(AutoGenerate = true)]
    public Int64 AccountStatusId
    {
      get
      {
        return _accountStatusId;
      }
      set
      {
        SetPropertyValue("AccountStatusId", ref _accountStatusId, value);
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

    private ACC_Status _status;
    [Persistent("StatusId")]
    public ACC_Status Status
    {
      get
      {
        return _status;
      }
      set
      {
        SetPropertyValue("Status", ref _status, value);
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

    public ACC_AccountStatus() : base() { }
    public ACC_AccountStatus(Session session) : base(session) { }

    #endregion
  }
}
