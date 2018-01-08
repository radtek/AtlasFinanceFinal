/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for AddressType
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
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class ADR_Type : XPLiteObject
  {

    private Int64 _AddressTypeId;
    [Key(AutoGenerate = false)]
    public Int64 AddressTypeId
    {
      get
      {
        return _AddressTypeId;
      }
      set
      {
        SetPropertyValue("AddressTypeId", ref _AddressTypeId, value);
      }
    }
  
    private string _Description;
    [Persistent]
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

    public ADR_Type() : base() { }
    public ADR_Type(Session session) : base(session) { }

    #endregion
  }
}
