/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-213 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *   Status of a NuCard
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

#region Using

using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Common.Extensions;
using Atlas.Domain.Interface;

#endregion


namespace Atlas.Domain.Model
{
  public class NUC_NuCardStatus : XPLiteObject
  {
    private Int64 _NuCardStatusId;
    [Key(AutoGenerate = false)]
    public Int64 NuCardStatusId
    {
      get { return _NuCardStatusId; }
      set { SetPropertyValue("NuCardStatusId", ref _NuCardStatusId, value); }
    }

    [NonPersistent]
    public Enumerators.NuCard.NuCardStatus Type
    {
      get { return Description.FromStringToEnum<Enumerators.NuCard.NuCardStatus>(); }
      set { value = Description.FromStringToEnum<Enumerators.NuCard.NuCardStatus>(); }
    }

    private string _Description;
    [Persistent]
    public string Description
    {
      get { return _Description; }
      set { SetPropertyValue("Description", ref _Description, value); }
    }


    #region Constructors

    public NUC_NuCardStatus() : base() { }
    public NUC_NuCardStatus(Session session) : base(session) { }

    #endregion
    
  }
}
