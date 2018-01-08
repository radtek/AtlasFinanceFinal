using Stream.Framework.Structures.Reports;

namespace Stream.Structures.Models.Reports
{
  public class PerformanceSummary : IPerformanceSummary
  {
    public int RegionId { get; set; }
    public string Region { get; set; }
    public int BranchId { get; set; }
    public string Branch { get; set; }
    public long AllocatedUserId { get; set; }
    public string AllocatedUser { get; set; }
    public int CategoryId { get; set; }
    public string Category { get; set; }
    public int SubCategoryId { get; set; }
    public string SubCategory { get; set; }
    public bool IsTotal { get; set; }
    public int Accounts { get; set; }
    public int Debtors { get; set; }
    public int Cases { get; set; }
    public int SystemClosedCases { get; set; }
    public int WorkableCases { get; set; }
    public int CurrentAccounts { get; set; }
    public int CurrentClients { get; set; }
    public int CurrentCases { get; set; }
    public int PtpPtcObtained { get; set; }
    public int PtpPtcBroken { get; set; }
    public int PtpPtcSuccessful { get; set; }
    public int FollowUps { get; set; }
    public int NoActionCount { get; set; }
    public float PtpPtcHitRate { get; set; }
    public float PtpPtcHitRateSuccessful { get; set; }
    public int Escalations { get; set; }
    public int TransferredIn { get; set; }
    public int TransferredOut { get; set; }
    public int SMSCount { get; set; }
  }
}