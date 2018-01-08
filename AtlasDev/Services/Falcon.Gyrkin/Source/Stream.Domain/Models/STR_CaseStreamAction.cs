using System;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  public class STR_CaseStreamAction : XPLiteObject
  {
    private Int64 _caseStreamActionId;
    [Key(AutoGenerate = true)]
    public Int64 CaseStreamActionId
    {
      get
      {
        return _caseStreamActionId;
      }
      set
      {
        SetPropertyValue("CaseStreamActionId", ref _caseStreamActionId, value);
      }
    }

    private STR_CaseStream _caseStream;
    [Persistent("CaseStreamId")]
    [Indexed]
    public STR_CaseStream CaseStream
    {
      get
      {
        return _caseStream;
      }
      set
      {
        SetPropertyValue("CaseStream", ref _caseStream, value);
      }
    }

    private DateTime _actionDate;
    [Persistent]
    [Indexed]
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

    private STR_ActionType _actionType;
    [Persistent("ActionTypeId")]
    [Indexed]
    public STR_ActionType ActionType
    {
      get
      {
        return _actionType;
      }
      set
      {
        SetPropertyValue("ActionType", ref _actionType, value);
      }
    }

    private DateTime? _dateActioned;
    [Persistent]
    [Indexed]
    public DateTime? DateActioned
    {
      get
      {
        return _dateActioned;
      }
      set
      {
        SetPropertyValue("DateActioned", ref _dateActioned, value);
      }
    }

    private DateTime? _completeDate;
    [Persistent]
    [Indexed]
    public DateTime? CompleteDate
    {
      get
      {
        return _completeDate;
      }
      set
      {
        SetPropertyValue("CompleteDate", ref _completeDate, value);
      }
    }

    private bool? _isSuccess;
    [Persistent]
    public bool? IsSuccess
    {
      get
      {
        return _isSuccess;
      }
      set
      {
        SetPropertyValue("IsSuccess", ref _isSuccess, value);
      }
    }

    private decimal? _amount;
    [Persistent]
    public decimal? Amount
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


    #region Constructors

    public STR_CaseStreamAction()
    { }
    public STR_CaseStreamAction(Session session) : base(session) { }

    #endregion

  }
}
