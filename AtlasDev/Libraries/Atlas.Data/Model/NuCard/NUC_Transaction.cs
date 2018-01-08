
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for Transaction
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
  public class NUC_Transaction : XPCustomObject, IAudit
  {
    private Int64 _NucardTransactionId;
    [Key(AutoGenerate = true)]
    public Int64 NucardTransactionId
    {
      get
      {
        return _NucardTransactionId;
      }
      set
      {
        SetPropertyValue("NucardTransactionId", ref _NucardTransactionId, value);
      }
    }

    private NUC_NuCard _NuCard;
    [Persistent("NuCardId")]
    [Indexed]
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

    private string _Description;
    [Persistent, Size(150)]
    public string Description
    {
      get
      {
        return _Description;
      }
      set
      {
        SetPropertyValue("Description", ref _Description, value);
      }
    }

    private string _ReferenceNum;
    [Persistent, Size(150), Indexed]
    public string ReferenceNum
    {
      get
      {
        return _ReferenceNum;
      }
      set
      {
        SetPropertyValue("ReferenceNum", ref _ReferenceNum, value);
      }
    }

    private Decimal? _Amount;
    [Persistent]
    public Decimal? Amount
    {
      get
      {
        return _Amount;
      }
      set
      {
        SetPropertyValue("Amount", ref _Amount, value);
      }
    }

    private DateTime? _LoadDT;
    [Persistent]
    public DateTime? LoadDT
    {
      get
      {
        return _LoadDT;
      }
      set
      {
        SetPropertyValue("LoadDT", ref _LoadDT, value);
      }
    }

    private bool _IsPending;
    [Persistent]
    public bool IsPending
    {
      get
      {
        return _IsPending;
      }
      set
      {
        SetPropertyValue("IsPending", ref _IsPending, value);
      }
    }

    private TransactionSource _Source;
    [Persistent("TransactionSourceId")]
    public TransactionSource Source
    {
      get
      {
        return _Source;
      }
      set
      {
        SetPropertyValue("Source", ref _Source, value);
      }
    }

    private string _ServerTransactionId;
    [Persistent("ServerTransactionId"), Size(260)]
    public string ServerTransactionId
    {
      get { return _ServerTransactionId; }
      set { SetPropertyValue("ServerTransactionId", ref _ServerTransactionId, value); }
    }

    private Enumerators.General.ApplicationIdentifiers _SourceApplication;
    [Persistent]
    public Enumerators.General.ApplicationIdentifiers SourceApplication
    {
      get { return _SourceApplication; }
      set { SetPropertyValue("SourceApplication", ref _SourceApplication, value); }
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

    private PER_Person _DeletedBy;
    [Persistent]
    public PER_Person DeletedBy
    {
      get
      {
        return _DeletedBy;
      }
      set
      {
        SetPropertyValue("DeletedBy", ref _DeletedBy, value);
      }
    }

    private PER_Person _LastEditedBy;
    [Persistent]
    public PER_Person LastEditedBy
    {
      get
      {
        return _LastEditedBy;
      }
      set
      {
        SetPropertyValue("LastEditedBy", ref _LastEditedBy, value);
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

    private DateTime? _DeletedDT;
    [Persistent]
    public DateTime? DeletedDT
    {
      get
      {
        return _DeletedDT;
      }
      set
      {
        SetPropertyValue("DeletedDT", ref _DeletedDT, value);
      }
    }

    private DateTime? _LastEditedDT;
    [Persistent]
    public DateTime? LastEditedDT
    {
      get
      {
        return _LastEditedDT;
      }
      set
      {
        SetPropertyValue("LastEditedDT", ref _LastEditedDT, value);
      }
    }   

    #region Constructors

    public NUC_Transaction() : base() { }
    public NUC_Transaction(Session session) : base(session) { }

    #endregion
  }
}
