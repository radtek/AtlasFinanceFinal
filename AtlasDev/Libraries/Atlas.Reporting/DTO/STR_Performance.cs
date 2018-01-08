using System.ComponentModel;

namespace Atlas.Reporting.DTO
{
  public class STR_Performance
  {
    public int RegionId { get; set; }

    [Description("Region")]
    public string Region { get; set; }

    public int BranchId { get; set; }

    [Description("Branch")]
    public string Branch { get; set; }

    public long AllocatedUserId { get; set; }

    [Description("Allocated User")]
    public string AllocatedUser { get; set; }

    public int CategoryId { get; set; }

    [Description("Category")]
    public string Category { get; set; }

    public int SubCategoryId { get; set; }

    //[Description("Sub-Category")]
    public string SubCategory { get; set; }

    public bool IsTotal { get; set; } // does not consolidate into parent group 

    //[Description("Cases")]
    //public int Cases { get; set; }

    //[Description("Accounts")]
    //public int Accounts { get; set; }

    //[Description("Debtors")]
    //public int Debtors { get; set; }

    //[Description("Cases")]
    //public int Cases { get; set; }

    [Description("System Closed Cases")]
    public int SystemClosedCases { get; set; }

    [Description("Force Closed Cases")]
    public int ForceClosedCases { get; set; }

    //[Description("Workable Cases")]
    //public int WorkableCases { get; set; }

    [Description("Current Accounts")]
    public int CurrentAccounts { get; set; }

    [Description("Current Clients")]
    public int CurrentClients { get; set; }

    [Description("Current Cases")]
    public int CurrentCases { get; set; }

    [Description("PTP's Obtained")]
    public int PtpPtcObtained { get; set; }

    [Description("PTP's Broken")]
    public int PtpPtcBroken { get; set; }

    [Description("Successful PTP's")]
    public int PtpPtcSuccessful { get; set; }

    [Description("Follow Ups")]
    public int FollowUps { get; set; }

    [Description("Not Actioned")]
    public int NoActionCount { get; set; }
    
    [Description("Not Interested")]
    public int NotInterested { get; set; }

    [Description("Hit Rate % PTP's vs Current Cases")]
    public float PtpPtcHitRate { get; set; }

    [Description("Hit Rate % Successful PTP's vs Current Cases")]
    public float PtpPtcHitRateSuccessful { get; set; }

    [Description("Escalations")]
    public int Escalations { get; set; }

    [Description("Transferred In")]
    public int TransferredIn { get; set; }

    [Description("Transferred Out")]
    public int TransferredOut { get; set; }

    [Description("SMS Count")]
    public int SMSCount { get; set; }
  }
}