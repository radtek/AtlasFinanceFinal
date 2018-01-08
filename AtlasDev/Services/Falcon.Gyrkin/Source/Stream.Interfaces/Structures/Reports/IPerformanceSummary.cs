namespace Stream.Framework.Structures.Reports
{
  public interface IPerformanceSummary
  {
    int RegionId { get; set; }

    string Region { get; set; }

    int BranchId { get; set; }

    string Branch { get; set; }

    long AllocatedUserId { get; set; }

    string AllocatedUser { get; set; }

    int CategoryId { get; set; }

    string Category { get; set; }

    int SubCategoryId { get; set; }

    string SubCategory { get; set; }

    bool IsTotal { get; set; }

    int Accounts { get; set; }

    int Debtors { get; set; }

    int Cases { get; set; }

    int SystemClosedCases { get; set; }

    int WorkableCases { get; set; }

    int CurrentAccounts { get; set; }

    int CurrentClients { get; set; }

    int CurrentCases { get; set; }

    int PtpPtcObtained { get; set; }

    int PtpPtcBroken { get; set; }

    int PtpPtcSuccessful { get; set; }

    int FollowUps { get; set; }

    int NoActionCount { get; set; }

    float PtpPtcHitRate { get; set; }

    float PtpPtcHitRateSuccessful { get; set; }

    int Escalations { get; set; }

    int TransferredIn { get; set; }

    int TransferredOut { get; set; }

    int SMSCount { get; set; }
  }
}