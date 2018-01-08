using System;
using System.Collections.Generic;
using Falcon.Exporter.CiReport.Infrastructure.Interfaces.Models;
using Serilog;

namespace Falcon.Exporter.CiReport.V1.Models
{
  internal class ExportCiReportModel : IExportCiReportModel
  {
    public IExportCiReportModelWorksheet ConsolidatedWorksheet { get; set; }
    public ICollection<IExportCiReportModelWorksheet> Worksheets { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ILogger Log { get; set; }
  }
}