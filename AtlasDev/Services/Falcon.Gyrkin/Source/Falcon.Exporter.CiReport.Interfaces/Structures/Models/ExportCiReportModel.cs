using System;
using System.Collections.Generic;
using Falcon.Exporter.CiReport.Infrastructure.Interfaces.Models;
using Falcon.Exporter.CiReport.Infrastructure.Structures.Reports;

namespace Falcon.Exporter.CiReport.Infrastructure.Structures.Models
{
  public class ExportCiReportModel : IExportCiReportModel
  {
    public ICollection<IExportCiReportModelWorksheet> Worksheets { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
  }

  public class ExportCiReportModelWorksheet : IExportCiReportModelWorksheet
  {
    public long Order { get; set; }
    public string Description { get; set; }
    public ICollection<Reports.CiReport> CiReports { get; set; }
    public ICollection<PossibleHandover> PossibleHandovers { get; set; }
    public ICollection<CiReportScore> CiReportScores { get; set; }
    public ICollection<CiReportLowMean> BranchesLowMeans { get; set; }
    public ICollection<CiReportBranchSyncStatus> LastBranchSyncStatuses { get; set; }
    public ICollection<CiReportBranchSummary> CiReportBranchSummaries { get; set; }
    public VarianceStatus VarianceStatus { get; set; }
  }
}