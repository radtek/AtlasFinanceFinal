using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class AVS_BankAccountPeriod:XPLiteObject
  {
    private Int32 _bankAccountPeriodId;
    [Key]
    public Int32 BankAccountPeriodId
    {
      get
      {
        return _bankAccountPeriodId;
      }
      set
      {
        SetPropertyValue("BankAccountPeriodId", ref _bankAccountPeriodId, value);
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
    [Persistent, Size(25)]
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

    private int _period; 
    [Persistent]
    public int Period
    {
      get
      {
        return _period;
      }
      set
      {
        SetPropertyValue("Period", ref _period, value);
      }
    }

    #region Constructors

    public AVS_BankAccountPeriod() : base() { }
    public AVS_BankAccountPeriod(Session session) : base(session) { }

    #endregion
  }
}
