/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-213 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for TCCTerminal
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

using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using Atlas.Domain.Interface;

namespace Atlas.Domain.Model
{
  public class TCCTerminal : XPCustomObject, IAudit
  {    
    [Key(AutoGenerate = true)]
    public Int64 TerminalId { get; set; }

    private string _SupplierTerminalId;

    [Persistent, Size(50)]
    public string SupplierTerminalId
    {
      get { return _SupplierTerminalId; }
      set { SetPropertyValue("SupplierTerminalId", ref _SupplierTerminalId, value); }
    }

    private string _MerchantId;
    [Persistent, Size(50)]
    public string MerchantId
    {
      get
      { return _MerchantId; }
      set { SetPropertyValue("MerchantId", ref _MerchantId, value); }
    }

    private string _IPAddress;
    [Persistent, Size(20)]
    public string IPAddress
    {
      get { return _IPAddress; }
      set { SetPropertyValue("IPAddress", ref _IPAddress, value); }
    }

    private string _Branch;
    [Persistent, Size(50)]
    [Indexed]
    public string Branch
    {
      get { return _Branch; }
      set { SetPropertyValue("Branch", ref _Branch, value); }
    }

    private string _Description;
    [Persistent, Size(500)]
    public string Description
    {
      get { return _Description; }
      set { SetPropertyValue("Description", ref _Description, value); }
    }

    private string _Location;
    [Persistent, Size(50)]
    public string Location
    {
      get { return _Location; }
      set { SetPropertyValue("Location", ref _Location, value); }
    }

    private string _SerialNum;
    [Persistent, Size(50)]
    public string SerialNum
    {
      get { return _SerialNum; }
      set { SetPropertyValue("SerialNum", ref _SerialNum, value); }
    }

    private int _Status;
    [Persistent]
    public int Status
    {
      get { return _Status; }
      set { SetPropertyValue("Status", ref _Status, value); }
    }

    private DateTime? _LastPolledDT;
    [Persistent]
    public DateTime? LastPolledDT
    {
      get { return _LastPolledDT; }
      set { SetPropertyValue("LastPolledDT", ref _LastPolledDT, value); }
    }

    private string _LastPolledResult;
    [Persistent, Size(500)]
    public string LastPolledResult
    {
      get { return _LastPolledResult; }
      set { SetPropertyValue("LastPolledResult", ref _LastPolledResult, value); }
    }

    private string _LastRequestType;

    [Persistent, Size(50)]
    public string LastRequestType
    {
      get { return _LastRequestType; }
      set { SetPropertyValue("LastRequestType", ref _LastRequestType, value); }
    }

    private DateTime? _LastRequestDT;

    [Persistent]
    public DateTime? LastRequestDT
    {
      get { return _LastRequestDT; }
      set { SetPropertyValue("LastRequestDT", ref _LastRequestDT, value); }
    }

    private string _LastRequestResult;
    [Persistent, Size(500)]
    public string LastRequestResult
    {
      get { return _LastRequestResult; }
      set { SetPropertyValue("LastRequestResult", ref _LastRequestResult, value); }
    }

    private string _HWMake;
    [Persistent, Size(50)]
    public string HWMake
    {
      get { return _HWMake; }
      set { SetPropertyValue("HWMake", ref _HWMake, value); }
    }

    private string _HWModel;

    [Persistent, Size(50)]
    public string HWModel
    {
      get { return _HWModel; }
      set { SetPropertyValue("HWModel", ref _HWModel, value); }
    }

    private Int32 _HWId;

    [Persistent]
    public Int32 HWId
    {
      get { return _HWId; }
      set { SetPropertyValue("HWId", ref _HWId, value); }
    }

    private Int32 _SuccessPingCount;
    [Persistent]
    public Int32 SuccessPingCount
    {
      get { return _SuccessPingCount; }
      set { SetPropertyValue("SuccessPingCount", ref _SuccessPingCount, value); }
    }

    private Int32 _FailedPingCount;
    [Persistent]
    public Int32 FailedPingCount
    {
      get { return _FailedPingCount; }
      set { SetPropertyValue("FailedPingCount", ref _FailedPingCount, value); }
    }

    private Int32 _UnknownPingCount;
    [Persistent]
    public Int32 UnknownPingCount
    {
      get { return _UnknownPingCount; }
      set { SetPropertyValue("UnknownPingCount", ref _UnknownPingCount, value); }
    }

    private PER_Person _CreatedBy;
    [Persistent]
    public PER_Person CreatedBy
    {
      get { return _CreatedBy; }
      set { SetPropertyValue("CreatedBy", ref _CreatedBy, value); }
    }

    private PER_Person _DeletedBy;
    [Persistent]
    public PER_Person DeletedBy
    {
      get { return _DeletedBy; }
      set { SetPropertyValue("DeletedBy", ref _DeletedBy, value); }
    }

    private PER_Person _LastEditedBy;
    [Persistent]
    public PER_Person LastEditedBy
    {
      get { return _LastEditedBy; }
      set { SetPropertyValue("LastEditedBy", ref _LastEditedBy, value); }
    }
    
    private DateTime? _CreatedDT;
    [Persistent]
    public DateTime? CreatedDT
    {
      get { return _CreatedDT; }
      set { SetPropertyValue("CreatedDT", ref _CreatedDT, value); }
    }

    private DateTime? _DeletedDT;
    [Persistent]
    public DateTime? DeletedDT
    {
      get { return _DeletedDT; }
      set { SetPropertyValue("DeletedDT", ref _DeletedDT, value); }
    }

    private DateTime? _LastEditedDT;
    [Persistent]
    public DateTime? LastEditedDT
    {
      get { return _LastEditedDT; }
      set { SetPropertyValue("LastEditedDT", ref _LastEditedDT, value); }
    }


    #region Constructors

    public TCCTerminal() : base() { }
    public TCCTerminal(Session session) : base(session) { }

    #endregion
  }
}
