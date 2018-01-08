using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.Model
{
  public class ACC_DebitControl : XPLiteObject
  {
    private Int64 _debitControlId;
    [Key(AutoGenerate = true)]
    public Int64 DebitControlId
    {
      get
      {
        return _debitControlId;
      }
      set
      {
        SetPropertyValue("DebitControlId", ref _debitControlId, value);
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

    private DBT_Control _control;
    [Persistent("ControlId")]
    public DBT_Control Control
    {
      get
      {
        return _control;
      }
      set
      {
        SetPropertyValue("Control", ref _control, value);
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

    public ACC_DebitControl() : base() { }
    public ACC_DebitControl(Session session) : base(session) { }

    #endregion
  }
}
