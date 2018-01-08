namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_SAFPS_Subject : XPLiteObject
  {
    private Int64 _subjectId;
    [Key(AutoGenerate = true)]
    [Persistent("SubjectId")]
    public Int64 SubjectId
    {
      get
      {
        return _subjectId;
      }
      set
      {
        SetPropertyValue("SubjectId", ref _subjectId, value);
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

    private string _subjectNo;
    [Persistent]
    public string SubjectNo
    {
      get
      {
        return _subjectNo;
      }
      set
      {
        SetPropertyValue("SubjectNo", ref _subjectNo, value);
      }
    }

    private string _category;
    [Persistent, Size(100)]
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

    private string _categoryNo;
    [Persistent, Size(100)]
    public string CategoryNo
    {
      get
      {
        return _categoryNo;
      }
      set
      {
        SetPropertyValue("CategoryNo", ref _categoryNo, value);
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

    private string _subject;
    [Persistent, Size(100)]
    public string Subject
    {
      get
      {
        return _subject;
      }
      set
      {
        SetPropertyValue("Subject", ref _subject, value);
      }
    }

    private string _passport;
    [Persistent, Size(30)]
    public string Passport
    {
      get
      {
        return _passport;
      }
      set
      {
        SetPropertyValue("Passport", ref _passport, value);
      }
    }

    private string _incidentDate;
    [Persistent]
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


    #region Constructors

    public FPM_SAFPS_Subject() : base() { }
    public FPM_SAFPS_Subject(Session session) : base(session) { }

    #endregion
  }
}
