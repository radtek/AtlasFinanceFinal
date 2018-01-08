/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-213 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    NuCard information
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

using DevExpress.Xpo;

using Atlas.Domain.Interface;

#endregion


namespace Atlas.Domain.Model
{
  public class NUC_NuCard : XPCustomObject, IAudit
  {
    private Int64 _NuCardId;
    [Key(AutoGenerate = true)]
    public Int64 NuCardId
    {
      get { return _NuCardId; }
      set { SetPropertyValue("NuCardId", ref _NuCardId, value); }
    }

    private PER_Person _AllocatedPerson;
    [Persistent("PersonId")]
    [Indexed]
    public PER_Person AllocatedPerson
    {
      get { return _AllocatedPerson; }
      set { SetPropertyValue("AllocatedPerson", ref _AllocatedPerson, value); }
    }

    private BRN_Branch _IssuedByBranch;
    [Persistent("IssuedByBranchId")]
    public BRN_Branch IssuedByBranch
    {
      get { return _IssuedByBranch; }
      set { SetPropertyValue("IssuedByBranch", ref _IssuedByBranch, value); }
    }

    private NUC_NuCardProfile _NuCardProfile;
    [Persistent("NucardProfileId"), Indexed]
    public NUC_NuCardProfile NuCardProfile
    {
      get { return _NuCardProfile; }
      set { SetPropertyValue("NuCardProfile", ref _NuCardProfile, value); }
    }

    private string _SequenceNum;
    [Persistent]
    public string SequenceNum
    {
      get { return _SequenceNum; }
      set { SetPropertyValue("SequenceNum", ref _SequenceNum, value); }
    }

    private string _TrackingNum;
    [Persistent, Indexed]
    public string TrackingNum
    {
      get { return _TrackingNum; }
      set { SetPropertyValue("TrackingNum", ref _TrackingNum, value); }
    }

    private string _CardNum;
    [Persistent, Indexed]
    public string CardNum
    {
      get { return _CardNum; }
      set { SetPropertyValue("CardNum", ref _CardNum, value); }
    }

    private DateTime _IssueDT;
    [Persistent]
    public DateTime IssueDT
    {
      get { return _IssueDT; }
      set { SetPropertyValue("IssueDT", ref _IssueDT, value); }
    }

    private DateTime _ExpiryDT;
    [Persistent, Indexed]
    public DateTime ExpiryDT
    {
      get { return _ExpiryDT; }
      set { SetPropertyValue("ExpiryDT", ref _ExpiryDT, value); }
    }

    private NUC_NuCardStatus _Status;
    [Persistent, Indexed]
    public NUC_NuCardStatus Status
    {
      get { return _Status; }
      set { SetPropertyValue("Status", ref _Status, value); }
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

    public NUC_NuCard() : base() { }
    public NUC_NuCard(Session session) : base(session) { }

    #endregion
  }
}
