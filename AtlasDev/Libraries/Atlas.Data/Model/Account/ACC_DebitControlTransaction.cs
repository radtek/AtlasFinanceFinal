using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.Model
{
  public class ACC_DebitControlTransaction : XPLiteObject
  {
    private Int64 _debitControlTransactionId;
    [Key(AutoGenerate = true)]
    public Int64 DebitControlTransactionId
    {
      get
      {
        return _debitControlTransactionId;
      }
      set
      {
        SetPropertyValue("DebitControlTransactionId", ref _debitControlTransactionId, value);
      }
    }

    private ACC_DebitControl _debitControl;
    [Persistent("DebitControlId")]
    public ACC_DebitControl DebitControl
    {
      get
      {
        return _debitControl;
      }
      set
      {
        SetPropertyValue("DebitControl", ref _debitControl, value);
      }
    }

    private DBT_Transaction _debitTransaction;
    [Persistent("DebitTransactionId")]
    public DBT_Transaction DebitTransaction
    {
      get
      {
        return _debitTransaction;
      }
      set
      {
        SetPropertyValue("DebitTransaction", ref _debitTransaction, value);
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

    private DateTime? _datePostedToLedger;
    [Persistent]
    public DateTime? DatePostedToLedger
    {
      get
      {
        return _datePostedToLedger;
      }
      set
      {
        SetPropertyValue("DatePostedToLedger", ref _datePostedToLedger, value);
      }
    }


    #region Constructors

    public ACC_DebitControlTransaction() : base() { }
    public ACC_DebitControlTransaction(Session session) : base(session) { }

    #endregion
  }
}
