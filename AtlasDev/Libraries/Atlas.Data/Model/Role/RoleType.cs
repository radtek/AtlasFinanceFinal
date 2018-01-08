
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for RoleType
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
  public class RoleType : XPLiteObject
  {

    private Int64 _RoleTypeId;
    [Key(AutoGenerate = false)]
    public Int64 RoleTypeId
    {
      get
      {
        return _RoleTypeId;
      }
      set
      {
        SetPropertyValue("RoleTypeId", ref _RoleTypeId, value);
      }
    }

    private Int16 _Level;
    [Persistent]
    public Int16 Level
    {
      get
      {
        return _Level;
      }
      set
      {
        SetPropertyValue("Level", ref _Level, value);
      }
    }

    [NonPersistent]
    public Enumerators.General.RoleType Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.RoleType>(); }
      set { value = Description.FromStringToEnum<Enumerators.General.RoleType>(); }
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

    public RoleType() : base() { }
    public RoleType(Session session)
      : base(session)
    {

    }

    #endregion
  }
}
