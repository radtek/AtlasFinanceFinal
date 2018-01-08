
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for PersonRelation
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

namespace Atlas.Domain.Model
{
  public class PER_Relation : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public Int64 RelationId { get; set; }

    [Persistent("PersonId")]
    public PER_Person Person { get; set; }

    [Persistent("RelationTypeId")]
    public PER_RelationType Relation { get; set; }

    [Persistent]
    public PER_Person RelationPerson { get; set; }

    #region Constructors

    public PER_Relation() : base() { }
    public PER_Relation(Session session) : base(session) { }

    #endregion
  }
}
