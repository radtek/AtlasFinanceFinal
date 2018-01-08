using System;
using System.ComponentModel;
using Atlas.Common.Attributes;

namespace Falcon.Exporter.CiReport.Infrastructure.Structures.Reports
{
  public class CiReportBranchSyncStatus
  {
    [Description("Name")]
    [Order(1)]
    public string Name { get; set; }

    [Description("Last Updated")]
    [Order(2)]
    [Format("dd/MM/yyyy HH:mm:ss")]
    public DateTime LastUpdatDate { get; set; }
  }
}