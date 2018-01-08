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

  public sealed class FPM_ConsumerTelephone : XPLiteObject
  {
    private Int64 _ConsumerTelephoneId;
    [Key(AutoGenerate = true)]
    public Int64 ConsumerTelephoneId
    {
      get
      {
        return _ConsumerTelephoneId;
      }
      set
      {
        SetPropertyValue("ConsumerTelephoneId", ref _ConsumerTelephoneId, value);
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

    private string _TelephoneNumberDial;
    [Persistent, Size(11)]
    public string TelephoneNumberDial
    {
      get { return _TelephoneNumberDial; }
      set
      {
        SetPropertyValue("TelephoneNumberDial", ref _TelephoneNumberDial, value);
      }
    }

    private string _TelephoneNumber;
    [Persistent, Size(15)]
    public string TelephoneNumber
    {
      get
      {
        return _TelephoneNumber;
      }
      set
      {
        SetPropertyValue("TelephoneNumber", ref _TelephoneNumber, value);
      }
    }

    private int _TelephoneTotal24Hours;
    [Persistent]
    public int TelephoneTotal24Hours
    {
      get
      {
        return _TelephoneTotal24Hours;
      }
      set
      {
        SetPropertyValue("TelephoneTotal24Hours", ref _TelephoneTotal24Hours, value);
      }
    }

    private int _TelephoneTotal48Hours;
    [Persistent]
    public int TelephoneTotal48Hours
    {
      get
      {
        return _TelephoneTotal48Hours;
      }
      set
      {
        SetPropertyValue("TelephoneTotal48Hours", ref _TelephoneTotal48Hours, value);
      }
    }

    private int _TelephoneTotal96Hours;
    [Persistent]
    public int TelephoneTotal96Hours
    {
      get
      {
        return _TelephoneTotal96Hours;
      }
      set
      {
        SetPropertyValue("TelephoneTotal96Hours", ref _TelephoneTotal96Hours, value);
      }
    }

    private int _TelephoneTotal30Days;
    [Persistent]
    public int TelephoneTotal30Days
    {
      get
      {
        return _TelephoneTotal30Days;
      }
      set
      {
        SetPropertyValue("TelephoneTotal30Days", ref _TelephoneTotal30Days, value);
      }
    }

    private string _CellPhoneNumber;
    [Persistent, Size(20)]
    public string CellPhoneNumber
    {
      get
      {
        return _CellPhoneNumber;
      }
      set
      {
        SetPropertyValue("CellPhoneNumber", ref _CellPhoneNumber, value);
      }
    }

    private int _CellPhoneTotal24Hours;
    [Persistent]
    public int CellPhoneTotal24Hours
    {
      get
      {
        return _CellPhoneTotal24Hours;
      }
      set
      {
        SetPropertyValue("CellPhoneTotal24Hours", ref _CellPhoneTotal24Hours, value);
      }
    }

    private int _CellPhoneTotal48Hours;
    [Persistent]
    public int CellPhoneTotal48Hours
    {
      get
      {
        return _CellPhoneTotal48Hours;
      }
      set
      {
        SetPropertyValue("CellPhoneTotal48Hours", ref _CellPhoneTotal48Hours, value);
      }
    }

    private int _CellPhoneTotal96Hours;
    [Persistent]
    public int CellPhoneTotal96Hours
    {
      get
      {
        return _CellPhoneTotal96Hours;
      }
      set
      {
        SetPropertyValue("CellPhoneTotal96Hours", ref _CellPhoneTotal96Hours, value);
      }
    }


    private int _CellPhoneTotal30Days;
    [Persistent]
    public int CellPhoneTotal30Days
    {
      get
      {
        return _CellPhoneTotal30Days;
      }
      set
      {
        SetPropertyValue("CellPhoneTotal30Days", ref _CellPhoneTotal30Days, value);
      }
    }

    #region Constructors

    public FPM_ConsumerTelephone () : base() { }
    public FPM_ConsumerTelephone(Session session) : base(session) { }

    #endregion
  }
}
