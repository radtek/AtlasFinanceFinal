
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
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{

  public class PER_RelationType : XPLiteObject
  {
    [Key(AutoGenerate = false)]
    public Int64 RelationTypeId { get;set;}

    [NonPersistent]
    public Enumerators.General.RelationType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.General.RelationType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.General.RelationType>();
      }
    }

    [Persistent, Size(20)]
    public string Description { get; set; }
    
    #region Constructors

    public PER_RelationType() : base() { }
    public PER_RelationType(Session session)
      : base(session)
    {

    }

    #endregion
  }
}
