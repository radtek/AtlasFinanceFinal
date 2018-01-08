using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PYT_PayoutValidation:XPLiteObject
  {
    private int _payoutValidationId;
    [Key(AutoGenerate=true)]
    public int PayoutValidationId
    {
      get
      {
        return _payoutValidationId;
      }
      set
      {
        SetPropertyValue("PayoutValidationId", ref _payoutValidationId, value);
      }
    }

    private PYT_Payout _payout;
    [Persistent("PayoutId")]
    public PYT_Payout Payout
    {
      get
      {
        return _payout;
      }
      set
      {
        SetPropertyValue("Payout", ref _payout, value);
      }
    }

    private PYT_Validation _validation;
    [Persistent("ValidationId")]
    public PYT_Validation Validation
    {
      get
      {
        return _validation;
      }
      set
      {
        SetPropertyValue("Validation", ref _validation, value);
      }
    }


    #region Constructors

    public PYT_PayoutValidation() : base() { }
    public PYT_PayoutValidation(Session session) : base(session) { }

    #endregion
  }
}
