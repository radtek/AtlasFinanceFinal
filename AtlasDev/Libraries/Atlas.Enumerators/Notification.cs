using System;
using System.ComponentModel;

namespace Atlas.Enumerators
{
  public class Notification
  {
    public enum Group
    {
      [Description("Stream")]
      Stream = 1
    }

    public enum NotificationType
    {
      [Description("Email")]
      Email = 1,
      [Description("SMS")]
      SMS = 2,
    }

    public enum NotificationStatus
    {
      [Description("New")]
      New = 1,
      [Description("Sent")]
      Sent = 2,
      [Description("Failed")]
      Failed = 3,
      [Description("Delivered")]
      Delivered = 4
    }

    public enum NotificationPriority
    {
      [Description("Low")]
      Low = 1,
      [Description("Normal")]
      Normal = 2,
      [Description("High")]
      High = 3
    }

    public enum NotificationTemplate
    {
      [Description("Approved")]
      Approved = 0,
      [Description("Payment")]
      Payment = 1,
      [Description("Declined - Bank Details")]
      Declined_BankDetails = 2,
      [Description("Declined - Credit Risk")]
      Declined_CreditRisk = 3,
      [Description("Declined - Affordability")]
      Declined_Affordability = 4,
      [Description("Pending")]
      Pending = 5,
      [Description("RTC CutOff")]
      RTC_CutOff = 6,
      [Description("Paid Up")]
      Paid_Up = 7,
      [Description("Appeal Received")]
      Appeal_Received = 8,
      [Description("Contact Us")]
      Contact_Us = 9,
      [Description("Quote Expire Daily")]
      Quote_Expire_Daily = 10,
      [Description("Quote Expired")]
      Quote_Expired = 11,
      [Description("Instalment Reminder")]
      Instalment_Reminder = 12,
      [Description("Overdue Account")]
      Overdue_Account = 13,
      [Description("Registration")]
      Registration = 14,
      [Description("Core")]
      Core = 15,
      [Description("Forgot Password")]
      Forgot_Password = 16,
      [Description("Application Expired")]
      Application_Expired = 17,
      [Description("Application Expiring")]
      Application_Expiring = 18,
      [Description("BankDetails Failed")]
      BankDetails_Failed = 19,
      [Description("Account Review")]
      Account_Review = 20,
      [Description("Declined - Company Policy")]
      Declined_CompanyPolicy = 21,
      [Description("Declined - Fraud")]
      Declined_Fraud = 22,
      [Description("Declined - Authentication")]
      Declined_Authentication = 23,
      [Description("AVS Passed - Accept Quote")]
      AVSPassed_AcceptQuote = 24,
      [Description("Stream - SMS - Initiate")]
      Stream_SMS_Initiate = 25,
      [Description("Stream - SMS - PTP Broken")]
      Stream_SMS_PTPBroken = 26,
      [Description("Stream - SMS - PTP Created")]
      Stream_SMS_PTPCreated = 27,
      [Description("Stream - SMS - No Contact")]
      Stream_SMS_NoContact = 28,
      [Description("Stream - SMS - Payment Thanks")]
      Stream_SMS_PaymentThanks = 29,
      //[Description("Stream - SMS - No Contact PTP Broken")]
      //Stream_SMS_NoContactPTPBroken = 30,
      [Description("Stream - SMS - PTC Broken")]
      Stream_SMS_PTCBroken = 31,
      [Description("Stream - SMS - PTC Created")]
      Stream_SMS_PTCCreated = 32,
      [Description("Stream - SMS - First Warning")]
      Stream_SMS_FirstWarning = 33,
      [Description("Stream - SMS - Second Warning")]
      Stream_SMS_SecondWarning = 34,
      [Description("Stream - SMS - Third Warning")]
      Stream_SMS_ThirdWarning = 35,
      [Description("Stream - Letter - Final Demand")]
      Stream_Letter_FinalDemand = 36
    }
  }
}
