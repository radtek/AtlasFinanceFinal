namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_SAFPS_EmploymentDetail : XPLiteObject
  {
    private Int64 _employmentDetailId;
    [Key(AutoGenerate = true)]
    [Persistent("EmploymentDetailId")]
    public Int64 EmploymentDetailId
    {
      get
      {
        return _employmentDetailId;
      }
      set
      {
        SetPropertyValue("EmploymentDetailId", ref _employmentDetailId, value);
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

    private string _name;
    [Persistent, Size(100)]
    public string Name
    {
      get
      {
        return _name;
      }
      set
      {
        SetPropertyValue("Name", ref _name, value);
      }
    }


    private string _telephone;
    [Persistent, Size(40)]
    public string Telephone
    {
      get
      {
        return _telephone;
      }
      set
      {
        SetPropertyValue("Telephone", ref _telephone, value);
      }
    }


    private string _registeredName;
    [Persistent, Size(100)]
    public string RegisteredName
    {
      get
      {
        return _registeredName;
      }
      set
      {
        SetPropertyValue("RegisteredName", ref _registeredName, value);
      }
    }


    private string _companyNo;
    [Persistent, Size(50)]
    public string CompanyNo
    {
      get
      {
        return _companyNo;
      }
      set
      {
        SetPropertyValue("CompanyNo", ref _companyNo, value);
      }
    }

    private string _occupation;
    [Persistent, Size(100)]
    public string Occupation
    {
      get
      {
        return _occupation;
      }
      set
      {
        SetPropertyValue("Occupation", ref _occupation, value);
      }
    }

    private string _from;
    [Persistent, Size(100)]
    public string From
    {
      get
      {
        return _from;
      }
      set
      {
        SetPropertyValue("From", ref _from, value);
      }
    }

    private string _to;
    [Persistent, Size(100)]
    public string To
    {
      get
      {
        return _to;
      }
      set
      {
        SetPropertyValue("To", ref _to, value);
      }
    }

    #region Constructors

    public FPM_SAFPS_EmploymentDetail() : base() { }
    public FPM_SAFPS_EmploymentDetail(Session session) : base(session) { }

    #endregion
  }
}
