/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for BankAccountType
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
  public class BNK_AccountType : XPLiteObject
  {
    private Int64 _AccountTypeId;
    [Key(AutoGenerate = false)]
    public Int64 AccountTypeId
    {
      get
      {
        return _AccountTypeId;
      }
      set
      {
        SetPropertyValue("AccountTypeId", ref _AccountTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.General.BankAccountType Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.BankAccountType>(); }
      set { value = Description.FromStringToEnum<Enumerators.General.BankAccountType>(); }
    }

    private string _Description;
    [Persistent,Size(500)]
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

    public BNK_AccountType() : base() { }
    public BNK_AccountType(Session session) : base(session) { }

    #endregion
  }
}