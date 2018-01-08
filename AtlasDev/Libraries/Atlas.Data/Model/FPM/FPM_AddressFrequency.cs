// -----------------------------------------------------------------------
// <copyright file="RiskEnquiry.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_AddressFrequency : XPLiteObject
  {
    private Int64 _AddressFrequencyId;
    [Key(AutoGenerate = true)]
    public Int64 AddressFrequencyId
    {
      get
      {
        return _AddressFrequencyId;
      }
      set
      {
        SetPropertyValue("AddressFrequencyId", ref _AddressFrequencyId, value);
      }
    }

    private FPM_FraudScore _FraudScore;
    [Indexed]
    [Persistent("FraudScoreId")]
    public FPM_FraudScore FraudScore
    {
      get
      {
        return _FraudScore;
      }
      set
      {
        SetPropertyValue("FraudScore", ref _FraudScore, value);
      }
    }

    private int _Last24Hours;
    [Persistent]
    public int Last24Hours
    {
      get { return _Last24Hours; }
      set
      {
        SetPropertyValue("Last24Hours", ref _Last24Hours, value);
      }
    }

    private int _Last48Hours;
    [Persistent]
    public int Last48Hours
    {
      get
      {
        return _Last48Hours;
      }
      set
      {
        SetPropertyValue("Last48Hours", ref _Last48Hours, value);
      }
    }

    private int _Last96Hours;
    [Persistent]
    public int Last96Hours
    {
      get
      {
        return _Last96Hours;
      }
      set
      {
        SetPropertyValue("Last96Hours", ref _Last96Hours, value);
      }
    }

    private int _Last30Days;
    [Persistent]
    public int Last30Days
    {
      get
      {
        return _Last30Days;
      }
      set
      {
        SetPropertyValue("Last30Days", ref _Last30Days, value);
      }
    }

    private string _AddressMessage;
    [Persistent, Size(9)]
    public string AddressMessage
    {
      get
      {
        return _AddressMessage;
      }
      set
      {
        SetPropertyValue("AddressMessage", ref _AddressMessage, value);
      }
    }

    #region Constructors

    public FPM_AddressFrequency () : base() { }
    public FPM_AddressFrequency(Session session) : base(session) { }

    #endregion
  }
}
