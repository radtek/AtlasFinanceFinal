using System;


namespace Atlas.Domain.DTO.ASS
{
  public class ASS_CiReportVersionDTO
  {
    public long CiReportVersionId { get; set; }
    public float VersionNo { get; set; }
    public DateTime VersionDate { get; set; }
    public string ExporterLocation { get; set; }
    public string ExporterLocationAbsoluteClassName { get; set; }
  }
}