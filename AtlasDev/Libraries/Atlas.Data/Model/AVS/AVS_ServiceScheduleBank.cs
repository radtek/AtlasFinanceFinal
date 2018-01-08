using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class AVS_ServiceScheduleBank : XPLiteObject
  {
    private Int32 _serviceScheduleBankId;
    [Key(AutoGenerate = true)]
    public Int32 ServiceScheduleBankId
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

    private AVS_ServiceSchedule _serviceSchedule;
    [Persistent("ServiceScheduleId")]
    public AVS_ServiceSchedule ServiceSchedule
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

    public AVS_ServiceScheduleBank() : base() { }
    public AVS_ServiceScheduleBank(Session session) : base(session) { }

    #endregion
  }
}
