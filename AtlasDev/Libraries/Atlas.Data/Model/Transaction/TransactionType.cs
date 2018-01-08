using System;

/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for TransactionType
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

using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using Atlas.Common.Extensions;
using Atlas.Domain.Interface;

namespace Atlas.Domain.Model
{
  public class TransactionType : XPLiteObject
  {

    private Int64 _TransactionTypeId;
    [Key(AutoGenerate = false)]
    public Int64 TransactionTypeId
    {
      get
      {
        return _TransactionTypeId;
      }
      set
      {
        SetPropertyValue("TransactionTypeId", ref _TransactionTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.General.TransactionType Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.TransactionType>(); }
      set { value = Description.FromStringToEnum<Enumerators.General.TransactionType>(); }
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

    public TransactionType() : base() { }
    public TransactionType(Session session) : base(session) { }

    #endregion
  }
}
