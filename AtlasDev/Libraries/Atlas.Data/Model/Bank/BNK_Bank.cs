
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for Bank
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
  public class BNK_Bank : XPLiteObject
  {
    private Int64 _bankId;
    [Key(AutoGenerate = false)]
    public Int64 BankId
    {
      get
      {
        return _bankId;
      }
      set
      {
        SetPropertyValue("BankId", ref _bankId, value);
      }
    }

    [NonPersistent]
    public Enumerators.General.BankName Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.General.BankName>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.General.BankName>();
      }
    }

    private string _description;
    [Persistent,Size(50)]    
    public string Description
    {
        get
        {
            return _description;
        }
        set
        {
            SetPropertyValue("Description", ref _description, value);
        }
    }

    private string _universalCode;
    [Persistent, Size(20)]
    [Indexed(Name = "IDX_UniversalCode")]
    public string UniversalCode
    {
        get
        {
            return _universalCode;
        }
        set
        {
            SetPropertyValue("UniversalCode", ref _universalCode, value);
        }
    }

    private string _swiftCode;
    [Persistent, Size(10)]
    [Indexed(Name = "IDX_SWIFTCODE")]
    public string SwiftCode
    {
      get
      {
        return _swiftCode;
      }
      set
      {
        SetPropertyValue("SwiftCode", ref _swiftCode, value);
      }
    }

    #region Constructors

    public BNK_Bank() : base() { }
    public BNK_Bank(Session session) : base(session) { }

    #endregion
  }
}
