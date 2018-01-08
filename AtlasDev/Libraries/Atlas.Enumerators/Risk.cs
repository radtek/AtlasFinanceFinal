using System;
using System.ComponentModel;


namespace Atlas.Enumerators
{
  public class Risk
  {
    #region Risk Service Type

    public enum ServiceType
    {
      [Description("Credit")]
      Credit = 0,
      [Description("Fraud")]
      Fraud = 1,
      [Description("XDS Authentication")]
      XDS_Authentication = 2,
      [Description("Atlas Naedo")]
      Atlas_Naedo = 3,
      [Description("Atlas Online Credit")]
      Atlas_Online_Credit = 4,
      [Description("CS ID Photo Verification")]
      CS_ID_Photo_Verification = 5
    }

    #endregion

    public enum BatchJobStatus
    {
      [Description("Response file ready for collection")]
      Response_Ready = 0,
      [Description("Job is Finished")]
      Job_Is_Finished = 1,
      [Description("Job is Queued")]
      Job_Is_Queued = 2,
      [Description("Job Error")]
      Job_Error = 3,
      [Description("Job Pending")]
      Job_Pending = 4,
      [Description("Job Staging")]
      Job_Staging = 5,
      [Description("Job Cancelled")]
      Job_Cancelled = 6,
      [Description("Pending Pickup")]
      Pending_Pickup = -100,
      [Description("Job Processed")]
      Job_Processed = 100,
      [Description("Response Item Problems")]
      Response_Item_Problem = 200
    }

    public enum BatchTransactionType
    {
      [Description("CSREG")]
      Registration = 0,
      [Description("CSUPD")]
      Update = 1,
      [Description("CSENQ")]
      Enquiry = 2,
      [Description("NLR")]
      NLR = 3
    }

    public enum BatchSubTransactionType
    {
      [Description("CLIENT")]
      Client = 0,
      [Description("LOAN")]
      Loan = 1,
      [Description("LOAN2")]
      Loan2 = 2,
      [Description("PAYMENT")]
      Payment = 3,
      [Description("ADDRESS")]
      Address = 4,
      [Description("TELEPHONE")]
      Telephone = 5,
      [Description("EMPLOYER")]
      Employer = 6,
      [Description("GLOBAL2")]
      Global = 7,
      [Description("CONFLICT")]
      Conflict = 8,
      [Description("LOANREG")]
      LoanReg = 9,
      [Description("LOANCLOSE")]
      LoanClose = 10,
      [Description("BATB2")]
      BatB2 = 11
    }

    public enum ResponseFormat
    {
      [Description("HTML")]
      HTML = 0,
      [Description("XML")]
      XML = 1,
      [Description("XHML")]
      XHML = 2,
      [Description("MHT")]
      MHT = 3,
      [Description("XPDF")]
      XPDF = 4,
      [Description("XMHT")]
      XMHT = 5
    }

    public enum CreditCheckDestination
    {
      [Description("http://cc.compuscan.co.za/servlet/CSIntegrationX")]
      LIVE = 0,
      [Description("http://cc-dev.compuscan.co.za/servlet/CSIntegrationX ")]
      TEST = 1
    }

    public enum BatchDestination
    {
      [Description("http://cc.compuscan.co.za/servlet/CSBatchParser")]
      LIVE = 0,
      [Description("http://cc-dev.compuscan.co.za/servlet/CSBatchParser")]
      TEST = 1
    }


    public enum EnquiryPurpose
    {
      NotSet = 0,
      Fraud_Investigation = 1,
      Fraud_Prevention = 2,
      Emplyoment = 3,
      Book_Assessment = 4,
      Credit_Limit = 5,
      Insurance_Application = 6,
      Education_Employment = 7,
      Unclaimed_Funds = 8,
      Tracing = 9,
      Score_Development = 10,
      Affordability_Assessment = 11,
      Credit_Assessment = 12,
      Debt_Review = 13,
      Marketing_Services = 14,
      Debt_Collection = 15,
      Account_Management = 16,
      Credit_Ombud_Enquiry = 17,
      Consumer_Enquiry = 18,
      Other = 19
    }

    public enum BureauAccountType
    {
      [Description("National Loans Register")]
      NLR = 0,
      [Description("Credit Providers Association")]
      CPA = 1
    }

    public enum RiskEnquiryType
    {
      [Description("Credit Enquiry")]
      Credit = 0,
      [Description("Fraud Enquiry")]
      Fraud = 1
    }

    public enum RiskTransactionType
    {
      [Description("Not Set")]
      Not_Set = 0,
      [Description("NLR")]
      NLR = 1,
      [Description("Score")]
      Score = 2,
    }

    public enum BureauAccountSource
    {
      [Description("CPA")]
      CPA = 0,
      [Description("NLR")]
      NLR = 1,
      [Description("ITC")]
      ITC = 2,
      [Description("Manual")]
      Manual = 3
    }

    public enum BatchSendFileFormat
    {
      [Description("CSV")]
      CSV = 0,
      [Description("PIPE")]
      Pipe = 1,
      [Description("XML")]
      XML = 2
    }

    public enum Policy
    {
      [Description("There are 3 or more NLR accounts written off")]
      ThreeOrMoreNLRAccountsWrittenOff = 1,
      [Description("There are 3 or more Judgments in the current year")]
      ThreeOrMoreJudgmentsInCurrentYear = 2,
      [Description("There has been 3 or more Defaults in current year")]
      ThreeOrMoreDefaultsCurrentYear = 3,
      [Description("The applicant is deceased")]
      ApplicantIsDeceased = 4,
      [Description("The applicant has a dispute indicator")]
      ApplicantHasDisputeIndicator = 5,
      [Description("The applicant is under debt review")]
      ApplicantIsUnderDebtReview = 6,
      [Description("The applicant is under administration")]
      ApplicantIsUnderAdministration = 7,
      [Description("There has been 1 or more Judgments in the last 12 months")]
      OneOrMoreJudgmentsInLastTwelveMonths = 8,
      [Description("There has been 1 or more Adverse records in the last 12 months")]
      OneOrMoreAdverseRecordsLastTwelveMonths = 9,
      [Description("There are 1 or more accounts in 90+ days in arrears")]
      OneOrMoreAccountsInNinetyDaysArrears = 10,
      [Description("The applicant is under sequestration")]
      ApplicantIsUnderSequestration = 11,
      [Description("The applicant status is not verified")]
      ApplicantStatusIsNotVerified = 12,
      [Description("Insufficient data for scoring purposes")]
      InsufficientDataForScoringPurposes = 13,
      [Description("SAFPS - Bank Account")]
      SAFPS_BankAccount_No = 14,
      [Description("SAFPS - Cell No Listed")]
      SAFPS_Cell_No = 15,
      [Description("SAFPS - Home No Listed")]
      SAFPS_Home_No = 16,
      [Description("SAFPS - Single Incident Listed")]
      SAFPS_Single_Incident = 17,
      [Description("SAFPS - Multiple Incidents Listed")]
      SAFPS_Multiple_Incident = 18,
      [Description("SAFPS - Subject Listed")]
      SAFPS_Subject = 19,
      [Description("SAFPS - Case Listed")]
      SAFPS_Case = 20
    }

    public enum Band
    {
      [Description("Minimum Risk")]
      Minimum_Risk = 1,
      [Description("Low Risk")]
      Low_Risk = 2,
      [Description("Average Risk")]
      Medium = 3,
      [Description("Override")]
      Override = 4,
      [Description("High Risk")]
      High_Risk = 5,
      [Description("Very High Risk")]
      Very_High_Risk = 6,
      [Description("Declined")]
      Declined = 7
    }

    public enum BandGroup
    {
      [Description("Good")]
      Good = 1,
      [Description("Bad")]
      Bad = 2,
      [Description("Declined")]
      Declined = 3
    }
  }
}
