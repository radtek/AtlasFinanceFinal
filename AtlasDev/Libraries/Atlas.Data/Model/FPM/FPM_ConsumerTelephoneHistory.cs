namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_ConsumerTelephoneHistory : XPLiteObject
  {
    private Int64 _consumerTelephoneHistoryId;
    [Key(AutoGenerate = true)]
    public Int64 ConsumerTelephoneHistoryId
    {
      get
      {
        return _consumerTelephoneHistoryId;
      }
      set
      {
        SetPropertyValue("ConsumerTelephoneHistoryId", ref _consumerTelephoneHistoryId, value);
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


    private string _areaCode;
    [Persistent, Size(11)]
    public string AreaCode
    {
      get { return _areaCode; }
      set
      {
        SetPropertyValue("AreaCode", ref _areaCode, value);
      }
    }

    private string _number;
    [Persistent, Size(50)]
    public string Number
    {
      get
      {
        return _number;
      }
      set
      {
        SetPropertyValue("Number", ref _number, value);
      }
    }

    private string _date;
    [Persistent, Size(50)]
    public string Date
    {
      get
      {
        return _date;
      }
      set
      {
        SetPropertyValue("Date", ref _date, value);
      }
    }

    private string _years;
    [Persistent, Size(20)]
    public string Years
    {
      get
      {
        return _years;
      }
      set
      {
        SetPropertyValue("Years", ref _years, value);
      }
    }


    #region Constructors

    public FPM_ConsumerTelephoneHistory () : base() { }
		public FPM_ConsumerTelephoneHistory(Session session) : base(session) { }

    #endregion
  }
}
