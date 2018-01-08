/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for NucardBatchCards
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
  public class NUC_NuCardBatchCard : XPLiteObject
  {
    private Int64 _NuCardBatchCardId;
    [Key(AutoGenerate = true)]
    public Int64 NuCardBatchCardId
    {
      get
      {
        return _NuCardBatchCardId;
      }
      set
      {
        SetPropertyValue("NuCardBatchCardId", ref _NuCardBatchCardId, value);
      }
    }

    private NUC_NuCardBatch _NuCardBatch;
    [Persistent("NucardBatchId")]
    [Indexed]
    [Association("NUC_Batch")]
    public NUC_NuCardBatch NuCardBatch
    {
      get
      {
        return _NuCardBatch;
      }
      set
      {
        SetPropertyValue("NuCardBatch", ref _NuCardBatch, value);
      }
    }

    private NUC_NuCard _Nucard;
    [Persistent("NucardId")]
    [Indexed]
    public NUC_NuCard NuCard
    {
      get
      {
        return _Nucard;
      }
      set
      {
        SetPropertyValue("NuCard", ref _Nucard, value);
      }
    }


    #region Constructors

    public NUC_NuCardBatchCard() : base() { }
    public NUC_NuCardBatchCard(Session session) : base(session) { }

    #endregion
  }
}
