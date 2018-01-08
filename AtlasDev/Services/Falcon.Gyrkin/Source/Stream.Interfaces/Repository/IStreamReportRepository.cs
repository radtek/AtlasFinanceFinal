using System;
using System.Collections.Generic;
using Stream.Framework.Structures;
using Stream.Framework.Structures.Reports;

namespace Stream.Framework.Repository
{
  public interface IStreamReportRepository
  {
    byte[] GetPerformanceReport(Enumerators.Stream.GroupType groupType, DateTime startDate, DateTime endDate, long[] branchIds = null);
    byte[] GetDetailReport(Enumerators.Stream.GroupType groupType, long branchId, int[] streamIds, int[] caseStatusIds);
    List<IPerformanceSummary> GetOverview(Enumerators.Stream.GroupType groupType, DateTime startDate, DateTime endDate, long regionId, int categoryId, int subCategoryId, long[] branchIds = null, int drillDownLevel = 1);
    List<IAccountStreamAction> GetAccounts(Enumerators.Stream.GroupType groupType, DateTime startDate, DateTime endDate, int categoryId, long[] branchIds, long allocatedUserId, string column);
    byte[] GetAccountsExport(Enumerators.Stream.GroupType groupType, DateTime startDate, DateTime endDate, long regionId, int categoryId, long[] branchIds, long allocatedUserId, string column);
  }
}
