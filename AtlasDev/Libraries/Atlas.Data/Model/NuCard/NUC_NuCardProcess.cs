/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    NuCard processes
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
using Atlas.Common.Extensions;
using DevExpress.Xpo;
using Atlas.Domain.Interface;

namespace Atlas.Domain.Model
{
  public class NUC_NuCardProcess : XPCustomObject
  {    
    private Int64 _NuCardProcessId;
    [Key(AutoGenerate = true)]
    public Int64 NuCardProcessId
    {
      get
      {
        return _NuCardProcessId;
      }
      set
      {
        SetPropertyValue("NuCardStatusId", ref _NuCardProcessId, value);
      }
    }

    private NUC_NuCardProcess _DependantNuCardProcess;
    [Persistent("DependantNuCardProcessId")]
    public NUC_NuCardProcess DependantNuCardProcess
    {
      get
      {
        return _DependantNuCardProcess;
      }
      set
      {
        SetPropertyValue("DependantNuCardProcess", ref _DependantNuCardProcess, value);
      }
    }

    private NUC_NuCard _NuCard;
    [Persistent("NucardId")]
    public NUC_NuCard NuCard 
    {
       get
      {
        return _NuCard;
      }
      set
      {
        SetPropertyValue("NuCard", ref _NuCard, value);
      }

    }

    private PER_Person _AssignedUser;
    [Persistent("AssignedPersonId")]
    public PER_Person AssignedUser
    {
      get
      {
        return _AssignedUser;
      }
      set
      {
        SetPropertyValue("AssignedUser", ref _AssignedUser, value);
      }
    }

    private NUC_Transaction _Transaction;
    [Persistent("TransactionId")]
    public NUC_Transaction Transaction
    {
      get
      {
        return _Transaction;
      }
      set
      {
        SetPropertyValue("Transaction", ref _Transaction, value);
      }
    }

    private bool _IsApproved;
    [Persistent]
    public bool IsApproved 
    {
      get 
      {
        return _IsApproved;
      }
      set 
      {
        SetPropertyValue("IsApproved", ref _IsApproved, value);
      }
    }

    private bool _IsDeclined;
    [Persistent]
    public bool IsDeclined
    {
      get
      {
        return _IsDeclined;
      }
      set
      {
        SetPropertyValue("IsDeclined", ref _IsDeclined, value);
      }
    }

    private DateTime _ApprovedDT;
    [Persistent]
    public DateTime ApprovedDT
    {
      get
      {
        return _ApprovedDT;
      }
      set
      {
        SetPropertyValue("ApprovedDT", ref _ApprovedDT, value);
      }
    }

    private DateTime _DeclinedDT;
    [Persistent]
    public DateTime DeclinedDT
    {
      get
      {
        return _DeclinedDT;
      }
      set
      {
        SetPropertyValue("DeclinedDT", ref _DeclinedDT, value);
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

    public NUC_NuCardProcess() : base() { }
    public NUC_NuCardProcess(Session session) : base(session) { }

    #endregion
  }
}
