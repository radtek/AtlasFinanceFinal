using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class DBT_Transaction : XPLiteObject
  {
    private Int64 _transactionId;
    [Key(AutoGenerate = true)]
    public Int64 TransactionId
    {
      get
      {
        return _transactionId;
      }
      set
      {
        SetPropertyValue("TransactionId", ref _transactionId, value);
      }
    }

    private DBT_Control _control;
    [Persistent("ControlId")]
    [Indexed]
    [Association]
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

    private DBT_DebitType _debitType;
    [Persistent("DebitTypeId")]
    [Indexed]
    public DBT_DebitType DebitType
    {
      get
      {
        return _debitType;
      }
      set
      {
        SetPropertyValue("DebitType", ref _debitType, value);
      }
    }

    private DBT_Batch _batch;
    [Persistent("BatchId")]
    [Indexed]
    public DBT_Batch Batch
    {
      get
      {
        return _batch;
      }
      set
      {
        SetPropertyValue("Batch", ref _batch, value);
      }
    }

    private DBT_Status _status;
    [Persistent("StatusId")]
    [Indexed]
    public DBT_Status Status
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

    private DateTime? _lastStatusDate;
    [Persistent]
    public DateTime? LastStatusDate
    {
      get
      {
        return _lastStatusDate;
      }
      set
      {
        SetPropertyValue("LastStatusDate", ref _lastStatusDate, value);
      }
    }

    private decimal _amount;
    [Persistent]
    public decimal Amount
    {
      get
      {
        return _amount;
      }
      set
      {
        SetPropertyValue("Amount", ref _amount, value);
      }
    }

    private DateTime _instalmentDate;
    [Persistent]
    public DateTime InstalmentDate
    {
      get
      {
        return _instalmentDate;
      }
      set
      {
        SetPropertyValue("InstalmentDate", ref _instalmentDate, value);
      }
    }

    private DateTime _actionDate;
    [Persistent]
    public DateTime ActionDate
    {
      get
      {
        return _actionDate;
      }
      set
      {
        SetPropertyValue("ActionDate", ref _actionDate, value);
      }
    }

    private int _repetition;
    [Persistent]
    public int Repetition
    {
      get
      {
        return _repetition;
      }
      set
      {
        SetPropertyValue("Repetition", ref _repetition, value);
      }
    }

    private DBT_ResponseCode _responseCode;
    [Persistent("ResponseCodeId")]
    [Indexed]
    public DBT_ResponseCode ResponseCode
    {
      get
      {
        return _responseCode;
      }
      set
      {
        SetPropertyValue("ResponseCode", ref _responseCode, value);
      }
    }

    private DateTime? _responseDate;
    [Persistent]
    public DateTime? ResponseDate
    {
      get
      {
        return _responseDate;
      }
      set
      {
        SetPropertyValue("ResponseDate", ref _responseDate, value);
      }
    }

    private DateTime? _cancelDate;
    [Persistent]
    public DateTime? CancelDate
    {
      get
      {
        return _cancelDate;
      }
      set
      {
        SetPropertyValue("CancelDate", ref _cancelDate, value);
      }
    }

    private PER_Person _cancelUser;
    [Persistent("CancelUserId")]
    [Indexed]
    public PER_Person CancelUser
    {
      get
      {
        return _cancelUser;
      }
      set
      {
        SetPropertyValue("CancelUser", ref _cancelUser, value);
      }
    }

    private DateTime? _overrideDate;
    [Persistent]
    public DateTime? OverrideDate
    {
      get
      {
        return _overrideDate;
      }
      set
      {
        SetPropertyValue("OverrideDate", ref _overrideDate, value);
      }
    }

    private PER_Person _overrideUser;
    [Persistent("OverrideUserId")]
    [Indexed]
    public PER_Person OverrideUser
    {
      get
      {
        return _overrideUser;
      }
      set
      {
        SetPropertyValue("OverrideUser", ref _overrideUser, value);
      }
    }

    private decimal? _overrideAmount;
    [Persistent("OverrideAmount")]
    public decimal? OverrideAmount
    {
      get
      {
        return _overrideAmount;
      }
      set
      {
        SetPropertyValue("OverrideAmount", ref _overrideAmount, value);
      }
    }

    private DateTime? _overrideActionDate;
    [Persistent("OverrideActionDate")]
    public DateTime? OverrideActionDate
    {
      get
      {
        return _overrideActionDate;
      }
      set
      {
        SetPropertyValue("OverrideActionDate", ref _overrideActionDate, value);
      }
    }

    private int? _overrideTrackingDays;
    [Persistent]
    public int? OverrideTrackingDays
    {
      get
      {
        return _overrideTrackingDays;
      }
      set
      {
        SetPropertyValue("OverrideTrackingDays", ref _overrideTrackingDays, value);
      }
    }

    private DateTime? _createDate;
    [Persistent("CreateDate")]
    public DateTime? CreateDate
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

    public DBT_Transaction() : base() { }
    public DBT_Transaction(Session session) : base(session) { }
  }
}