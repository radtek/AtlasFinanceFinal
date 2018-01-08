using System;
using System.Collections.Generic;

namespace Falcon.Common.Interfaces.Services
{
  public interface ICiReportService
  {
    byte[] GetCiReport(DateTime startDate, DateTime endDate, List<long> branchIds);
  }
}