namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_SAFPS_CaseDetail : XPLiteObject
  {
    private Int64 _caseDetailId;
    [Key(AutoGenerate = true)]
    [Persistent("CaseDetailId")]
    public Int64 CaseDetailId
    {
      get
      {
        return _caseDetailId;
      }
      set
      {
        SetPropertyValue("CaseDetailId", ref _caseDetailId, value);
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

    private string _caseNo;
    [Persistent, Size(50)]
    public string CaseNo
    {
      get
      {
        return _caseNo;
      }
      set
      {
        SetPropertyValue("CaseNo", ref _caseNo, value);
      }
    }

    private string _reportDate;
    [Persistent, Size(50)]
    public string ReportDate
    {
      get
      {
        return _reportDate;
      }
      set
      {
        SetPropertyValue("ReportDate", ref _reportDate, value);
      }
    }

    private string _officer;
    [Persistent, Size(50)]
    public string Officer
    {
      get
      {
        return _officer;
      }
      set
      {
        SetPropertyValue("Officer", ref _officer, value);
      }
    }

    private string _createdBy;
    [Persistent, Size(50)]
    public string CreatedBy
    {
      get
      {
        return _createdBy;
      }
      set
      {
        SetPropertyValue("CreatedBy", ref _createdBy, value);
      }
    }

    private string _station;
    [Persistent, Size(50)]
    public string Station
    {
      get
      {
        return _station;
      }
      set
      {
        SetPropertyValue("Station", ref _station, value);
      }
    }

    private string _type;
    [Persistent, Size(60)]
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

    private string _status;
    [Persistent, Size(60)]
    public string Status
    {
      get
      {
        return _status;
      }
      set
      {
        SetPropertyValue("Status", ref _status, value);
      }
    }

    private string _reason;
    [Persistent, Size(60)]
    public string Reason
    {
      get
      {
        return _reason;
      }
      set
      {
        SetPropertyValue("Reason", ref _reason, value);
      }
    }

    private string _reasonExtension;
    [Persistent, Size(60)]
    public string ReasonExtension
    {
      get
      {
        return _reasonExtension;
      }
      set
      {
        SetPropertyValue("ReasonExtension", ref _reasonExtension, value);
      }
    }

    private string _contactNo;
    [Persistent, Size(60)]
    public string ContactNo
    {
      get
      {
        return _contactNo;
      }
      set
      {
        SetPropertyValue("ContactNo", ref _contactNo, value);
      }
    }

    private string _email;
    [Persistent, Size(60)]
    public string Email
    {
      get
      {
        return _email;
      }
      set
      {
        SetPropertyValue("Email", ref _email, value);
      }
    }

    private string _fax;
    [Persistent, Size(60)]
    public string Fax
    {
      get
      {
        return _fax;
      }
      set
      {
        SetPropertyValue("Fax", ref _fax, value);
      }
    }

    private string _details;
    [Persistent, Size(Int32.MaxValue)]
    public string Details
    {
      get
      {
        return _details;
      }
      set
      {
        SetPropertyValue("Details", ref _details, value);
      }
    }


    #region Constructors

    public FPM_SAFPS_CaseDetail() : base() { }
    public FPM_SAFPS_CaseDetail(Session session) : base(session) { }

    #endregion
  }
}
