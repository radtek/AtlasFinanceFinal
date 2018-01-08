
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for TransactionSource
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
using Atlas.Common.Extensions;
using Atlas.Domain.Interface;

namespace Atlas.Domain.Model
{
  public class TransactionSource : XPLiteObject
  {
    private Int64 _TransactionSourceId;
    [Key(AutoGenerate = true)]
    public Int64 TransactionSourceId
    {
      get
      {
        return _TransactionSourceId;
      }
      set
      {
        SetPropertyValue("TransactionSourceId", ref _TransactionSourceId, value);
      }
    }

    [NonPersistent]
    public Enumerators.NuCard.TransactionSourceType Type
    {
      get { return Description.FromStringToEnum<Enumerators.NuCard.TransactionSourceType>(); }
      set { value = Description.FromStringToEnum<Enumerators.NuCard.TransactionSourceType>(); }
    }

    private string _Description;
    [Persistent, Size(10)]
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

    #region Constructors

    public TransactionSource() : base() { }
    public TransactionSource(Session session) : base(session) { }

    #endregion
  }
}
