using System;
using System.Collections.Generic;
using Falcon.Exporter.CiReport.Infrastructure.Structures.Reports;

namespace Falcon.Exporter.CiReport.Infrastructure.Interfaces.Models
{
  public interface IExportCiReportModel
  {
    ICollection<IExportCiReportModelWorksheet> Worksheets { get; set; }
    DateTime StartDate { get; set; }
    DateTime EndDate { get; set; }
  }

  public interface IExportCiReportModelWorksheet
  {
    long Order { get; set; }
    string Description { get; set; }
    ICollection<Structures.Reports.CiReport> CiReports { get; set; }
    ICollection<PossibleHandover> PossibleHandovers { get; set; }
    ICollection<CiReportScore> CiReportScores { get; set; }
    ICollection<CiReportLowMean> BranchesLowMeans { get; set; }
    ICollection<CiReportBranchSyncStatus> LastBranchSyncStatuses { get; set; }
    ICollection<CiReportBranchSummary> CiReportBranchSummaries { get; set; }
    VarianceStatus VarianceStatus { get; set; }
  }
}