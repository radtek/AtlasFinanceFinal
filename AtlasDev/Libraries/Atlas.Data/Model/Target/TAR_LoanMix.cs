using System;
using DevExpress.Xpo;

namespace Atlas.Domain.Model.Target
{
  public class TAR_LoanMix : XPLiteObject
  {
    private long _loanMixId;
    [Key(AutoGenerate = true)]
    public long LoanMixId
    {
      get
      {
        return _loanMixId;
      }
      set
      {
        SetPropertyValue("LoanMixId", ref _loanMixId, value);
      }
    }

    private DateTime _targetDate;
    [Persistent]
    public DateTime TargetDate
    {
      get
      {
        return _targetDate;
      }
      set
      {
        SetPropertyValue("TargetDate", ref _targetDate, value);
      }
    }

    private int _payNo;
    public int PayNo
    {
      get
      {
        return _payNo;
      }
      set
      {
        SetPropertyValue("PayNo", ref _payNo, value);
      }
    }

    private float _percent;
    public float Percent
    {
      get
      {
        return _percent;
      }
      set
      {
        SetPropertyValue("Percent", ref _percent, value);
      }
    }

    #region Constructors

    public TAR_LoanMix() { }
    public TAR_LoanMix(Session session) : base(session) { }
    
    #endregion
  }
}
