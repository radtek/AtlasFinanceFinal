
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for BankDetails
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
  public class BNK_Period : XPLiteObject
  {

    private int _periodId;
    [Key(AutoGenerate = false)]
    public int PeriodId
    {
      get
      {
        return _periodId;
      }
      set
      {
        SetPropertyValue("PeriodId", ref _periodId, value);
      }
    }


    [NonPersistent]
    public Enumerators.General.BankPeriod Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.General.BankPeriod>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.General.BankPeriod>();
      }
    }

    private string _description;
    [Persistent, Size(50)]
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

    public BNK_Period() : base() { }
    public BNK_Period(Session session) : base(session) { }

    #endregion
  }
}
