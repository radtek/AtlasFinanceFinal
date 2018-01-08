using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class DBT_ServiceScheduleBank : XPLiteObject
  {
    private int _serviceScheduleBankId;
    [Key(AutoGenerate = true)]
    public int ServiceScheduleBankId
    {
      get
      {
        return _serviceScheduleBankId;
      }
      set
      {
        SetPropertyValue("ServiceScheduleBankId", ref _serviceScheduleBankId, value);
      }
    }

    private DBT_ServiceSchedule _serviceSchedule;
    [Persistent("ServiceScheduleId")]
    [Indexed]
    public DBT_ServiceSchedule ServiceSchedule
    {
      get
      {
        return _serviceSchedule;
      }
      set
      {
        SetPropertyValue("ServiceSchedule", ref _serviceSchedule, value);
      }
    }

    private BNK_Bank _bank;
    [Persistent("BankId")]
    [Indexed]
    public BNK_Bank Bank
    {
      get
      {
        return _bank;
      }
      set
      {
        SetPropertyValue("Bank", ref _bank, value);
      }
    }

     private bool _enabled;
    [Persistent]
    public bool Enabled
    {
      get
      {
        return _enabled;
      }
      set
      {
        SetPropertyValue("Enabled", ref _enabled, value);
      }
    }

    public DBT_ServiceScheduleBank() : base() { }
    public DBT_ServiceScheduleBank(Session session) : base(session) { }
  }
}