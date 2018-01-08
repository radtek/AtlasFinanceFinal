namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;
  using Atlas.Enumerators;

  public sealed class FPM_DecisionCode : XPLiteObject
  {
    private int _decisionCodeId;
    [Key(AutoGenerate = true)]
    public int DecisionCodeId
    {
      get
      {
        return _decisionCodeId;
      }
      set
      {
        SetPropertyValue("DecisionCodeId", ref _decisionCodeId, value);
      }
    }

    private string _reasonCode;
    [Persistent]
    public string ReasonCode
    {
      get { return _reasonCode; }
      set
      {
        SetPropertyValue("ReasonCode", ref _reasonCode, value);
      }
    }

    private FPM.DecisionOutCome _decisionOutCome;
    [Persistent]
    public FPM.DecisionOutCome DecisionOutCome
    {
      get { return _decisionOutCome; }
      set
      {
        SetPropertyValue("DecisionOutCome", ref _decisionOutCome, value);
      }
    }


    #region Constructors

    public FPM_DecisionCode() : base() { }
    public FPM_DecisionCode(Session session) : base(session) { }

    #endregion
  }
}
