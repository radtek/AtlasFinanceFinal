/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-213 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Log for card administration (Tatuka API)- load funds/remove funds/stop/etc.
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
  public class NUC_LogAdminEvent : XPLiteObject
  {
    private Int64 _LogAdminEventId;
    [Key(AutoGenerate = true)]
    public Int64 LogAdminEventId
    {
      get { return _LogAdminEventId; }
      set { SetPropertyValue("LogAdminEventId", ref _LogAdminEventId, value); }
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

    private Enumerators.NuCard.AdminRequestType _NuCardRequestType;

    [Persistent("NuCardRequestType")]
    public Enumerators.NuCard.AdminRequestType NuCardRequestType
    {
      get { return _NuCardRequestType; }
      set { SetPropertyValue("NuCardRequestType", ref _NuCardRequestType, value); }
    }

    private Enumerators.NuCard.AdminRequestResult _RequestResult;
    [Persistent("RequestResult")]
    public Enumerators.NuCard.AdminRequestResult RequestResult
    {
      get { return _RequestResult; }
      set { SetPropertyValue("RequestResult", ref _RequestResult, value); }
    }

    private Decimal? _Amount;
    [Persistent]
    public Decimal? Amount
    {
      get { return _Amount; }
      set { SetPropertyValue("Amount", ref _Amount, value); }
    }

    private NUC_NuCard _MainCard;
    /// <summary>
    /// The main/destination card in the transaction
    /// </summary>
    [Persistent("SourceCard"), Indexed]
    public NUC_NuCard MainCard
    {
      get { return _MainCard; }
      set { SetPropertyValue("MainCard", ref _MainCard, value); }
    }

    private NUC_NuCard _SecondaryCard;
    /// <summary>
    /// Secondary/source card of transaction
    /// </summary>
    [Persistent("SecondaryCard"), Indexed]
    public NUC_NuCard SecondaryCard
    {
      get { return _SecondaryCard; }
      set { SetPropertyValue("SecondaryCard", ref _SecondaryCard, value); }
    }
    
    private string _AdditionalInfo;
    /// <summary>
    /// JSON encoded additional information
    /// </summary>
    [Persistent, Size(int.MaxValue)]    
    public string AdditionalInfo
    {
      get { return _AdditionalInfo; }
      set { SetPropertyValue("AdditionalInfo", ref _AdditionalInfo, value); }
    }
   

    private string _ClientTransactionID;
    /// <summary>
    /// Atlas Client transaction ID
    /// </summary>    
    [Persistent, Size(255)]
    public string ClientTransactionID
    {
      get { return _ClientTransactionID; }
      set { SetPropertyValue("ClientTransactionID", ref _ClientTransactionID, value); }
    }

    private string _ServerTransactionID;
    /// <summary>
    /// Tatuka transaction ID
    /// </summary>
    [Persistent, Size(255)]
    public string ServerTransactionID
    {
      get { return _ServerTransactionID; }
      set { SetPropertyValue("ServerTransactionID", ref _ServerTransactionID, value); }
    }

    private string _ResultText;
    [Persistent, Size(int.MaxValue)]
    public string ResultText
    {
      get { return _ResultText; }
      set { SetPropertyValue("ResultText", ref _ResultText, value); }
    }

    private DateTime? _CreatedDT;
    [Persistent]
    public DateTime? CreatedDT
    {
      get { return _CreatedDT; }
      set { SetPropertyValue("CreatedDT", ref _CreatedDT, value); }
    }

    private string _XMLSent;
    [Persistent, Size(int.MaxValue)]
    public string XMLSent
    {
      get { return _XMLSent; }
      set { SetPropertyValue("XMLSent", ref _XMLSent, value); }
    }

    private string _XMLReceived;
    [Persistent, Size(int.MaxValue)]
    public string XMLReceived
    {
      get { return _XMLReceived; }
      set { SetPropertyValue("XMLReceived", ref _XMLReceived, value); }
    }
    
    #region Constructors

    public NUC_LogAdminEvent() : base() { }
    public NUC_LogAdminEvent(Session session) : base(session) { }

    #endregion
  }
}