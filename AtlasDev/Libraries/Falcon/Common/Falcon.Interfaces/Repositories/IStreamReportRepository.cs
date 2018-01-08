using System;
using System.Collections.Generic;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures.Reports.Stream;
using Falcon.Common.Interfaces.Structures.Stream;

namespace Falcon.Common.Interfaces.Repositories
{
  public interface IStreamReportRepository
  {
    void CacheReportData(Stream.GroupType groupType, DateTime startDate, DateTime endDate);
    //void CacheReportData(DateTime startDate, DateTime endDate, TimeSpan? expiryTime = null, bool ignoreIfKeyExists = false);
    byte[] GetPerformanceReport(Stream.GroupType groupType, DateTime startDate, DateTime endDate, long[] branchIds = null);
    //byte[] GetPerformanceReport(Atlas.Enumerators.Stream.GroupType groupType, DateTime startDate, DateTime endDate, long[] branchIds = null, long[] userIds = null);
    List<IPerformanceSummary> GetOverview(Stream.GroupType groupType, DateTime startDate, DateTime endDate, long regionId, int categoryId, long[] branchIds = null, int drillDownLevel = 1);
    List<IAccountStreamAction> GetAccounts(Stream.GroupType groupType, DateTime startDate, DateTime endDate, long regionId, int categoryId, long[] branchIds, long allocatedUserId, string  colIndex);
    byte[] GetAccountsExport(Stream.GroupType groupType, DateTime startDate, DateTime endDate, long regionId, int categoryId, long[] branchIds, long allocatedUserId, string column);
  }
}
