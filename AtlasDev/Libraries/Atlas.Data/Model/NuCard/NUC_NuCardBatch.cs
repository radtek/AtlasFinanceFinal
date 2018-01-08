/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-213 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Stores details for a Batch of NuCards- used when receiving/sending NuCards
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
  public class NUC_NuCardBatch : XPCustomObject, IAudit
  {
    private Int64 _NuCardBatchId;
    [Key(AutoGenerate = true)]
    public Int64 NuCardBatchId
    {
      get { return _NuCardBatchId; }
      set { SetPropertyValue("NuCardBatchId", ref _NuCardBatchId, value); }
    }

    private BRN_Branch _DeliverToBranch;
    [Persistent, Indexed]
    public BRN_Branch DeliverToBranch
    {
      get { return _DeliverToBranch; }
      set { SetPropertyValue("DeliverToBranch", ref _DeliverToBranch, value); }
    }

    private BRN_Branch _ReceivedByBranch;
    [Persistent]
    [Indexed]
    public BRN_Branch ReceivedByBranch
    {
      get { return _ReceivedByBranch; }
      set { SetPropertyValue("ReceivedByBranch", ref _ReceivedByBranch, value); }
    }

    private string _BundleNum;
    [Persistent, Size(10)]
    public string BundleNum
    {
      get { return _BundleNum; }
      set { SetPropertyValue("BundleNum", ref _BundleNum, value); }
    }

    private string _SequenceStart;
    [Persistent, Size(10)]
    public string SequenceStart
    {
      get { return _SequenceStart; }
      set { SetPropertyValue("SequenceStart", ref _SequenceStart, value); }
    }

    private string _SequenceEnd;
    [Persistent, Size(10)]
    public string SequenceEnd
    {
      get { return _SequenceEnd; }
      set { SetPropertyValue("SequenceEnd", ref _SequenceEnd, value); }
    }

    private string _OutSequence;
    [Persistent, Size(100)]
    public string OutSequence
    {
      get { return _OutSequence; }
      set { SetPropertyValue("OutSequence", ref _OutSequence, value); }
    }

    private DateTime? _DeliveryDT;
    [Persistent, Indexed]
    public DateTime? DeliveryDT
    {
      get { return _DeliveryDT; }
      set { SetPropertyValue("DeliveryDT", ref _DeliveryDT, value); }
    }

    private DateTime? _ReceivedDT;
    [Persistent, Indexed]
    public DateTime? ReceivedDT
    {
      get { return _ReceivedDT; }
      set { SetPropertyValue("ReceivedDT", ref _ReceivedDT, value); }
    }

    private DateTime? _SentDT;
    [Persistent]
    public DateTime? SentDT
    {
      get { return _SentDT; }
      set { SetPropertyValue("SentDT", ref _SentDT, value); }
    }

    private PER_Person _SentBy;
    [Persistent]
    public PER_Person SentBy
    {
      get { return _SentBy; }
      set { SetPropertyValue("SentBy", ref _SentBy, value); }
    }

    private PER_Person _ReceivedBy;
    [Persistent]
    public PER_Person ReceivedBy
    {
      get { return _ReceivedBy; }
      set { SetPropertyValue("ReceivedBy", ref _ReceivedBy, value); }
    }

    private CPY_Company _Courier;
    [Persistent, Indexed]
    public CPY_Company Courier
    {
      get { return _Courier; }
      set { SetPropertyValue("Courier", ref _Courier, value); }

    }

    private int _QuantitySent;
    [Persistent]
    public int QuantitySent
    {
      get { return _QuantitySent; }
      set { SetPropertyValue("QuantitySent", ref _QuantitySent, value); }
    }

    private int _QuantityReceived;
    [Persistent]
    public int QuantityReceived
    {
      get { return _QuantityReceived; }
      set { SetPropertyValue("QuantityReceived", ref _QuantityReceived, value); }
    }

    private Enumerators.General.NucardBatchStatus _Status;
    [Persistent, Indexed]
    public Enumerators.General.NucardBatchStatus Status
    {
      get { return _Status; }
      set { SetPropertyValue("Status", ref _Status, value); }
    }

    private string _TrackingNum;
    [Persistent]
    public string TrackingNum
    {
      get { return _TrackingNum; }
      set { SetPropertyValue("TrackingNum", ref _TrackingNum, value); }
    }

    private string _Comment;
    [Persistent]
    public string Comment
    {

      get { return _Comment; }
      set { SetPropertyValue("Comment", ref _Comment, value); }
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


    [Association("NUC_Batch", typeof(NUC_NuCardBatchCard))]
    public XPCollection<NUC_NuCardBatchCard> NucardBatchCards { get { return GetCollection<NUC_NuCardBatchCard>("NucardBatchCards"); } }


    #region Constructors

    public NUC_NuCardBatch() : base() { }
    public NUC_NuCardBatch(Session session) : base(session) { }

    #endregion

  }
}
