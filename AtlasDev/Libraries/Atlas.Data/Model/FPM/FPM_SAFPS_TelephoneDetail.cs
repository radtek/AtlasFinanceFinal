namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_SAFPS_TelephoneDetail : XPLiteObject
  {
    private Int64 _telephoneDetailId;
    [Key(AutoGenerate = true)]
    [Persistent("TelephoneDetailId")]
    public Int64 TelephoneDetailId
    {
      get
      {
        return _telephoneDetailId;
      }
      set
      {
        SetPropertyValue("TelephoneDetailId", ref _telephoneDetailId, value);
      }
    }

    private FPM_SAFPS_Enquiry _sAFPS;
    [Persistent("SafpsId")]
    [Indexed]
    public FPM_SAFPS_Enquiry SAFPS
    {
      get
      {
        return _sAFPS;
      }
      set
      {
        SetPropertyValue("SAFPS", ref _sAFPS, value);
      }
    }

    private string _type;
    [Persistent, Size(30)]
    public string Type
    {
      get
      {
        return _type;
      }
      set
      {
        SetPropertyValue("Type", ref _type, value);
      }
    }

    private string _no;
    [Persistent, Size(50)]
    public string No
    {
      get
      {
        return _no;
      }
      set
      {
        SetPropertyValue("No", ref _no, value);
      }
    }

    private string _city;
    [Persistent, Size(50)]
    public string City
    {
      get
      {
        return _city;
      }
      set
      {
        SetPropertyValue("City", ref _city, value);
      }
    }

    private string _country;
    [Persistent, Size(60)]
    public string Country
    {
      get
      {
        return _country;
      }
      set
      {
        SetPropertyValue("Country", ref _country, value);
      }
    }

    #region Constructors

    public FPM_SAFPS_TelephoneDetail() : base() { }
    public FPM_SAFPS_TelephoneDetail(Session session) : base(session) { }

    #endregion
  }
}
