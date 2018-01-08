using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PYT_ServiceScheduleBank : XPLiteObject
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

    private PYT_ServiceSchedule _serviceSchedule;
    [Persistent("ServiceScheduleId")]
    public PYT_ServiceSchedule ServiceSchedule
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

    #region Constructors

    public PYT_ServiceScheduleBank() : base() { }
    public PYT_ServiceScheduleBank(Session session) : base(session) { }

    #endregion
  }
}
