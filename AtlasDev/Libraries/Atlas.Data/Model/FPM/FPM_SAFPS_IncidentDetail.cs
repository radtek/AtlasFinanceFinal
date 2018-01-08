namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_SAFPS_IncidentDetail : XPLiteObject
  {
    private Int64 _incidentDetailId;
    [Key(AutoGenerate = true)]
    [Persistent("IncidentDetailId")]
    public Int64 IncidentDetailId
    {
      get
      {
        return _incidentDetailId;
      }
      set
      {
        SetPropertyValue("IncidentDetailId", ref _incidentDetailId, value);
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

    private bool _victim;
    [Persistent]
    public bool Victim
    {
      get
      {
        return _victim;
      }
      set
      {
        SetPropertyValue("Victim", ref _victim, value);
      }
    }

    private string _membersReference;
    [Persistent, Size(50)]
    public string MembersReference
    {
      get
      {
        return _membersReference;
      }
      set
      {
        SetPropertyValue("MembersReference", ref _membersReference, value);
      }
    }

    private string _category;
    [Persistent, Size(50)]
    public string Category
    {
      get
      {
        return _category;
      }
      set
      {
        SetPropertyValue("Category", ref _category, value);
      }
    }

    private string _subCategory;
    [Persistent, Size(100)]
    public string SubCategory
    {
      get
      {
        return _subCategory;
      }
      set
      {
        SetPropertyValue("SubCategory", ref _subCategory, value);
      }
    }


    private string _incidentDate;
    [Persistent, Size(30)]
    public string IncidentDate
    {
      get
      {
        return _incidentDate;
      }
      set
      {
        SetPropertyValue("IncidentDate", ref _incidentDate, value);
      }
    }

    private string _subRole;
    [Persistent, Size(100)]
    public string SubRole
    {
      get
      {
        return _subRole;
      }
      set
      {
        SetPropertyValue("SubRole", ref _subRole, value);
      }
    }

    private string _city;
    [Persistent, Size(100)]
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

    private string _detail;
    [Persistent, Size(Int32.MaxValue)]
    public string Detail
    {
      get
      {
        return _detail;
      }
      set
      {
        SetPropertyValue("Detail", ref _detail, value);
      }
    }

    private string _forensic;
    [Persistent, Size(250)]
    public string Forensic
    {
      get
      {
        return _forensic;
      }
      set
      {
        SetPropertyValue("Forensic", ref _forensic, value);
      }
    }

    #region Constructors

    public FPM_SAFPS_IncidentDetail() : base() { }
    public FPM_SAFPS_IncidentDetail(Session session) : base(session) { }

    #endregion
  }
}
