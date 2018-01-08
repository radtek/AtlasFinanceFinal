using System;
using DevExpress.Xpo;

namespace Atlas.Domain.Ass.Models
{
  public class ASS_CiReportVersion : XPLiteObject
  {
    private long _ciReportVersionId;

    [Key(AutoGenerate = true)]
    public long CiReportVersionId
    {
      get { return _ciReportVersionId; }
      set { SetPropertyValue("CiReportVersionId", ref _ciReportVersionId, value); }
    }

    private float _versionNo;

    [Persistent]
    [Indexed]
    public float VersionNo
    {
      get { return _versionNo; }
      set { SetPropertyValue("VersionNo", ref _versionNo, value); }
    }

    private DateTime _versionDate;

    [Persistent]
    [Indexed]
    public DateTime VersionDate
    {
      get { return _versionDate; }
      set { SetPropertyValue("VersionDate", ref _versionDate, value); }
    }

    private string _exporterLocation;

    [Persistent, Size(1000)]
    [Indexed]
    public string ExporterLocation
    {
      get { return _exporterLocation; }
      set { SetPropertyValue("ExporterLocation", ref _exporterLocation, value); }
    }

    private string _exporterLocationAbsoluteClassName;

    [Persistent, Size(1000)]
    [Indexed]
    public string ExporterLocationAbsoluteClassName
    {
      get { return _exporterLocationAbsoluteClassName; }
      set { SetPropertyValue("ExporterLocationAbsoluteClassName", ref _exporterLocationAbsoluteClassName, value); }
    }
    
    #region Constructors

    public ASS_CiReportVersion() : base() { }
    public ASS_CiReportVersion(Session session) : base(session) { }

    #endregion

  }
}