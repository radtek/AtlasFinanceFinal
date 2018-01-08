
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for RelationType
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
  public class RelationType : XPLiteObject
  {
  

    private Int64 _RelationTypeId;
    [Key(AutoGenerate = false)]
    public Int64 RelationTypeId
    {
      get
      {
        return _RelationTypeId;
      }
      set
      {
        SetPropertyValue("RelationTypeId", ref _RelationTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.General.RelationType Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.RelationType>(); }
      set { value = Description.FromStringToEnum<Enumerators.General.RelationType>(); }
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

    public RelationType() : base() { }
    public RelationType(Session session) : base(session) { }

    #endregion
  }
}
