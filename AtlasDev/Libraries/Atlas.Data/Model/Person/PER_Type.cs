
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for Person Type
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

  public class PER_Type : XPLiteObject
  {

    private Int64 _typeId;
    [Key(AutoGenerate = false)]
    public Int64 TypeId
    {
      get
      {
        return _typeId;
      }
      set
      {
        SetPropertyValue("TypeId", ref _typeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.General.PersonType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.General.PersonType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.General.PersonType>();
      }
    }

    private string _description;
    [Persistent,Size(20)]    
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
    
    #region Constructors

    public PER_Type() : base() { }
    public PER_Type(Session session)
      : base(session)
    {

    }

    #endregion
  }
}
