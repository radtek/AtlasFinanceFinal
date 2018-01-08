
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for CompanyType
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
  public class CPY_Type : XPLiteObject
  {
    private Int64 _CompanyTypeId;
    [Persistent]
    [Key(AutoGenerate = false)]
    public Int64 CompanyTypeId
    {
      get
      {
        return _CompanyTypeId;
      }
      set
      {
        SetPropertyValue("CompanyTypeId", ref _CompanyTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.General.CompanyType Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.CompanyType>(); }
      set { value = Description.FromStringToEnum<Enumerators.General.CompanyType>(); }
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

    public CPY_Type() : base() { }
    public CPY_Type(Session session) : base(session) { }

    #endregion

  }
}
