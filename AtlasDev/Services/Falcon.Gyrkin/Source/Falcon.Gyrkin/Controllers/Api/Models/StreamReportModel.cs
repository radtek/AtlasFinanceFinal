using System;

namespace Falcon.Gyrkin.Controllers.Api.Models
{
  public class StreamReportModel
  {
    public enum SpecifiedColumn
    {
      NewCasesBF,
      NewCases,
      NewCasessTotal,
      NewCasesAccountsAffected,
      TotalDistinctAccount
    }

    public class StreamFilterModel
    {
      public int GroupTypeId { get; set; }
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }
      public long[] BranchIds { get; set; }
    }

    public class StreamDetailFilterModel
    {
      public int GroupTypeId { get; set; }
      public long BranchId { get; set; }
      public int[] CaseStatusIds { get; set; }
      public int[] StreamIds { get; set; }
    }

    public class StreamOverviewModel
    {
      public int GroupTypeId { get; set; }
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }
      public long RegionId { get; set; }
      public int CategoryId { get; set; }
      public int SubCategoryId { get; set; }
      public long[] BranchIds { get; set; }
      public int DrillDownLevel { get; set; }
    }

    public class StreamAccounts
    {
      public int GroupTypeId { get; set; }
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }
      public long RegionId { get; set; }
      public int CategoryId { get; set; }
      public long[] BranchIds { get; set; }
      public long AllocatedUserId { get; set; }
      public string Column { get; set; }
    }
  }
}
