namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_SAFPS_OtherIdDetail : XPLiteObject
  {
    private Int64 _otherIdDetailId;
    [Key(AutoGenerate = true)]
    [Persistent("OtherIdDetailId")]
    public Int64 OtherIdDetailId
    {
      get
      {
        return _otherIdDetailId;
      }
      set
      {
        SetPropertyValue("OtherIdDetailId", ref _otherIdDetailId, value);
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

    private string _iDNo;
    [Persistent, Size(30)]
    public string IDNo
    {
      get
      {
        return _iDNo;
      }
      set
      {
        SetPropertyValue("IDNo", ref _iDNo, value);
      }
    }

    private string _type;
    [Persistent, Size(20)]
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

    private string _issueDate;
    [Persistent, Size(30)]
    public string IssueDate
    {
      get
      {
        return _issueDate;
      }
      set
      {
        SetPropertyValue("IssueDate", ref _issueDate, value);
      }
    }
    private string _country;
    [Persistent, Size(50)]
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

    public FPM_SAFPS_OtherIdDetail() : base() { }
    public FPM_SAFPS_OtherIdDetail(Session session) : base(session) { }

    #endregion
  }
}
