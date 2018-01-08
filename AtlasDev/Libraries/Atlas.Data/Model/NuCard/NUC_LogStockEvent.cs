/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-213 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Logs stock events- stock movement, etc.
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Domain.Security;
using DevExpress.Xpo;
using Atlas.Domain.Interface;

#endregion


namespace Atlas.Domain.Model
{
  public class NUC_LogStockEvent : XPLiteObject
  {
    private Int64 _LogStockEventId;
    [Key(AutoGenerate = true)]
    public Int64 LogStockEventId
    {
      get { return _LogStockEventId; }
      set { SetPropertyValue("LogStockEventId", ref _LogStockEventId, value); }
    }

    private DateTime _EventDT;
    [Persistent, Indexed]
    public DateTime EventDT
    {
      get { return _EventDT; }
      set { SetPropertyValue("EventDT", ref _EventDT, value); }
    }

    private COR_AppUsage _Application;
    [Persistent("AppUsage")]
    public COR_AppUsage Application
    {
      get { return _Application; }
      set { SetPropertyValue("AppUsage", ref _Application, value); }
    }

    private Enumerators.NuCard.StockRequestType _NuCardStockRequestType;
    [Persistent("NuCardRequestType")]
    public Enumerators.NuCard.StockRequestType NuCardStockRequestType
    {
      get { return _NuCardStockRequestType; }
      set { SetPropertyValue("NuCardStockRequestType", ref _NuCardStockRequestType, value); }
    }

    private Enumerators.NuCard.StockRequestResult _RequestResult;
    [Persistent("RequestResult")]
    public Enumerators.NuCard.StockRequestResult RequestResult
    {
      get { return _RequestResult; }
      set { SetPropertyValue("RequestResult", ref _RequestResult, value); }
    }

    private string _ResultText;
    [Persistent("ResultText"), Size(int.MaxValue)]
    public string ResultText
    {
      get { return _ResultText; }
      set { SetPropertyValue("ResultText", ref _ResultText, value); }
    }
    
    private NUC_NuCard _Card;
    [Persistent("NuCard"), Indexed]
    public NUC_NuCard Card
    {
      get { return _Card; }
      set { SetPropertyValue("NuCard", ref _Card, value); }
    }

    private string _UnknownCardNum;
    [Persistent("UnknownCardNum"), Size(20), Indexed]
    public string UnknownCard
    {
      get { return _UnknownCardNum; }
      set { SetPropertyValue("UnknownCardNum", ref _UnknownCardNum, value); }
    }
        
    private string _SourceRequestParameters;
    [Persistent, Size(int.MaxValue)]
    public string SourceRequestParameters
    {
      get { return _SourceRequestParameters;}
      set { SetPropertyValue("SourceRequestParameters", ref _SourceRequestParameters, value); }
    }
             
    private DateTime? _CreatedDT;
    [Persistent]
    public DateTime? CreatedDT
    {
      get { return _CreatedDT; }
      set { SetPropertyValue("CreatedDT", ref _CreatedDT, value); }
    }


    #region Constructors

    public NUC_LogStockEvent() : base() { }
    public NUC_LogStockEvent(Session session) : base(session) { }

    #endregion
  }
}