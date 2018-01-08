
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for LogNuCardEvent
 * 
 * 
 *  Author:
 *  ------------------
 *     Lee Venkatsamy
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-11-07 - Initial
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using Atlas.Domain.Security;

namespace Atlas.Domain.Model
{
  public class LogProductEvent : XPLiteObject
  {
    private Int64 _logProductEventId;
    [Key(AutoGenerate = true)]
    public Int64 LogProductEventId
    {
      get
      {
        return _logProductEventId;
      }
      set
      {
        SetPropertyValue("LogProductEventId", ref _logProductEventId, value);
      }
    }

    private DateTime _eventDT;
    [Persistent]
    [Indexed]
    public DateTime EventDT
    {
      get
      {
        return _eventDT;
      }
      set
      {
        SetPropertyValue("EventDT", ref _eventDT, value);
      }
    }

    private TransactionSource _sourceSystem;
    [Persistent("SourceSystem")]
    public TransactionSource SourceSystem
    {
      get
      {
        return _sourceSystem;
      }
      set
      {
        SetPropertyValue("SourceSystem", ref _sourceSystem, value);
      }
    }

    private DateTime _requestStartDT;
    [Persistent]
    public DateTime RequestStartDT
    {
      get
      {
        return _requestStartDT;
      }
      set
      {
        SetPropertyValue("RequestStartDT", ref _requestStartDT, value);
      }
    }

    private DateTime _requestEndDT;
    [Persistent]
    public DateTime RequestEndDT
    {
      get
      {
        return _requestEndDT;
      }
      set
      {
        SetPropertyValue("RequestEndDT", ref _requestEndDT, value);
      }
    }

    private Enumerators.General.LogProductRequestType _productRequestType;
    [Persistent("ProductRequestType")]
    public Enumerators.General.LogProductRequestType ProductRequestType
    {
      get
      {
        return _productRequestType;
      }
      set
      {
        SetPropertyValue("ProductRequestType", ref _productRequestType, value);
      }
    }

    private COR_Machine _requestedBy;
    [Persistent("RequestedBy")]
    public COR_Machine RequestedBy
    {
      get
      {
        return _requestedBy;
      }
      set
      {
        SetPropertyValue("RequestedBy", ref _requestedBy, value);
      }
    }

    private string _requestParams;
    [Persistent("RequestParams"), Size(500)]
    public string RequestParams
    {
      get
      {
        return _requestParams;
      }
      set
      {
        SetPropertyValue("RequestParams", ref _requestParams, value);
      }
    }

    private Enumerators.General.LogProductRequestResult _requestResult;
    [Persistent("RequestResult")]
    public Enumerators.General.LogProductRequestResult RequestResult
    {
      get
      {
        return _requestResult;
      }
      set
      {
        SetPropertyValue("RequestResult", ref _requestResult, value);
      }
    }

    private string _ResultText;
    [Persistent, Size(5000)]
    public string ResultText
    {
      get
      {
        return _ResultText;
      }
      set
      {
        SetPropertyValue("ResultText", ref _ResultText, value);
      }
    }

    #region Constructors

    public LogProductEvent() : base() { }
    public LogProductEvent(Session session) : base(session) { }

    #endregion
  }
}
