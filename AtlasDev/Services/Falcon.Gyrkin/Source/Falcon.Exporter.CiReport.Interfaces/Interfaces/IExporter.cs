using Falcon.Exporter.CiReport.Infrastructure.Interfaces.Models;
using Serilog;

namespace Falcon.Exporter.CiReport.Infrastructure.Interfaces
{
  public interface IExporter
  {
    byte[] ExportCiReport(IExportCiReportModel exportCiReportModel, ILogger logger);
  }
}