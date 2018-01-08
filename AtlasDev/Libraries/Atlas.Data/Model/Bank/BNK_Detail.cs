
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for BankDetails
 * 
 * 
 *  Author:
 *  ------------------
 *     Fabian Franco-Roldan
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
  public class BNK_Detail : XPCustomObject
  {

    private Int64 _DetailId;
    [Key(AutoGenerate = true)]
    public Int64 DetailId
    {
      get
      {
        return _DetailId;
      }
      set
      {
        SetPropertyValue("DetailId", ref _DetailId, value);
      }
    }

    private BNK_Bank _Bank;
    [Persistent("BankId")]
    public BNK_Bank Bank
    {
      get
      {
        return _Bank;
      }
      set
      {
        SetPropertyValue("Bank", ref _Bank, value);
      }
    }

    private BNK_AccountType _AccountType;
    [Persistent("AccountTypeId")]
    public BNK_AccountType AccountType
    {
      get
      {
        return _AccountType;
      }
      set
      {
        SetPropertyValue("AccountType", ref _AccountType, value);
      }
    }
     
    private BNK_Period _period;
    [Persistent("PeriodId")]
    public BNK_Period Period
    {
      get
      {
        return _period;
      }
      set
      {
        SetPropertyValue("Period", ref _period, value);
      }
    }

    private string _AccountName;
    [Persistent, Size(50)]
    public string AccountName
    {
      get
      {
        return _AccountName;
      }
      set
      {
        SetPropertyValue("AccountName", ref _AccountName, value);
      }
    }

    private string _AccountNum;
    [Persistent, Size(20)]
    public string AccountNum
    {
      get
      {
        return _AccountNum;
      }
      set
      {
        SetPropertyValue("AccountNum", ref _AccountNum, value);
      }
    }

    private string _Code;
    [Persistent, Size(20)]
    public string Code
    {
      get
      {
        return _Code;
      }
      set
      {
        SetPropertyValue("Code", ref _Code, value);
      }
    }

    private bool _IsActive;
    [Persistent]
    public bool IsActive
    {
      get
      {
        return _IsActive;
      }
      set
      {
        SetPropertyValue("IsActive", ref _IsActive, value);
      }
    }

    private PER_Person _CreatedBy;
    [Persistent]
    public PER_Person CreatedBy
    {
      get
      {
        return _CreatedBy;
      }
      set
      {
        SetPropertyValue("CreatedBy", ref _CreatedBy, value);
      }
    }

    private DateTime? _CreatedDT;
    [Persistent]
    public DateTime? CreatedDT
    {
      get
      {
        return _CreatedDT;
      }
      set
      {
        SetPropertyValue("CreatedDT", ref _CreatedDT, value);
      }
    }

    #region Constructors

    public BNK_Detail() : base() { }
    public BNK_Detail(Session session) : base(session) { }

    #endregion
  }
}
