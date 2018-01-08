using System.ComponentModel;

namespace Stream.Framework.Enumerators
{
  public class Stream
  {
    public enum BudgetType
    {
      [Description("Compuscan Enquiries")] CompuscanEnquiries = 0
    }

    public enum ActuatorType
    {
      [Description("Escalation")] Escalation = 1
    }
    
    public enum FrequencyType
    {
      [Description("Daily")] Daily = 1,
      [Description("Weekly")] Weekly = 2,
      [Description("Bi-Weekly")] BiWeekly = 3,
      [Description("Monthly")] Monthly = 4
    }

    public enum SubCategory
    {
      [Description("First Instalment Missed (0/1)")] PossibleHandovers_FirstInstalmentMissed = 1,
      [Description("First Two Instalments Missed (0/2)")] PossibleHandovers_First2InstalmentsMissed = 2,
      [Description("First Three Instalments Missed (0/3)")] PossibleHandovers_First3InstalmentsMissed = 3,
      [Description("Default")] PossibleHandovers_Default = 4,
      [Description("First Instalment Missed (0/1)")] NextPossibleHandovers_FirstInstalmentMissed = 5,
      [Description("First Two Instalments Missed (0/2)")] NextPossibleHandovers_First2InstalmentsMissed = 6,
      [Description("First Three Instalments Missed (0/3)")] NextPossibleHandovers_First3InstalmentsMissed = 7,
      [Description("Default")] NextPossibleHandovers_Default = 8,
      [Description("First Instalment Missed (0/1)")] Arrears_FirstInstalmentMissed = 9,
      [Description("First Two Instalments Missed (0/2)")] Arrears_First2InstalmentsMissed = 10,
      [Description("First Three Instalments Missed (0/3)")] Arrears_First3InstalmentsMissed = 11,
      [Description("Default")] Arrears_Default = 12,
      [Description("Revived")] Sales_Revived = 13,
      [Description("Existing")] Sales_Existing = 14,
      [Description("Promo: July")] Sales_Promo = 15
    }

    public enum AccountNoteType
    {
      [Description("Action")] Action = 2,
      [Description("Normal")] Normal = 3
    }

    public enum EscalationType
    {
      [Description("None")] None = 1,
      [Description("Branch Manager")] BranchManager = 2,
      [Description("Admin Manager")] AdminManager = 3,
      [Description("Region Manager")] RegionManager = 4,
      [Description("Operation Executive")] OperationExecutive = 5,
      [Description("Director")] Director = 6
    }

    public enum PriorityType
    {
      [Description("Very Low")] VeryLow = 1,
      [Description("Low")] Low = 2,
      [Description("Lower Than Normal")] LowerThanNormal = 3,
      [Description("Normal")] Normal = 4,
      [Description("Higher Than Normal")] HigherThanNormal = 5,
      [Description("High")] High = 6,
      [Description("Very High")] VeryHigh = 7
    }

    public enum StreamType
    {
      [Description("New")] New = 1,
      [Description("PTP")] PTP = 2,
      [Description("PTC")] PTC = 3,
      [Description("Follow Up")] FollowUp = 4,
      [Description("PTP Broken")] PTPBroken = 5,
      [Description("Completed")] Completed = 6,
      [Description("PTC Broken")] PTCBroken = 7
    }

    public enum GroupType
    {
      [Description("Collections")] Collections = 1,
      [Description("Sales")] Sales = 2
    }

    public enum CommentGroupType
    {
      [Description("Sales - Not Interested")] Sales_NotInterested = 1,
      [Description("Sales - Follow Ups")] Sales_FollowUps = 2,
      [Description("Sales - PTC")] Sales_PTC = 3,
      [Description("Sales - No Action")] Sales_NoAction = 4,
      [Description("Sales - PTC Broken")] Sales_PTCBroken = 5,
      [Description("Collections - Follow Ups")] Collections_FollowUps = 6,
      [Description("Collections - PTP")] Collections_PTP = 7,
      [Description("Collections - No Action")] Collections_NoAction = 8,
      [Description("Collections - PTP Broken")] Collections_PTPBroken = 9,
      [Description("Collections - Escalate")] Collections_Escalate = 10,
      [Description("Sales - De-Escalate")] Sales_DeEscalate = 11,
      [Description("Collections - De-Escalate")] Collections_DeEscalate = 12,
      [Description("Sales - Transfer Case")] Sales_TransferCase = 13,
      [Description("Collections - Transfer Case")] Collections_TransferCase = 14,
      [Description("Sales - PTC Unchanged")] Sales_PTCUnchanged = 15,
      [Description("Collections - PTP Unchanged")] Collections_PTPUnchanged = 16,
      [Description("Sale: PTC Outcome - Yes")] Sale_PTCOutcomeYes = 17,
      [Description("Sale: PTC Outcome - No")] Sale_PTCOutcomeNo = 18,
      [Description("Sales: Force Closed")] Sale_ForceClosed = 19
    }

    public enum Comment
    {
      PTPBroken = 1,
      PTPSuccessful = 2,
      PaidUpOnAss = 3,
      UpToDate = 4,
      Complete = 5,
      EscalatedBySystem = 6,
      PTCBrokenBySystem = 7,
      PTCClientNotInterested = 8,
      PTPBrokenMoreThanThreeTimes = 9,
      PriorityIncrease = 10,
      ClientTookNewerLoan = 11,
      PTCSuccesful = 12,
      ClientDeceased = 13
    }

    //public enum SMS
    //{
    //  InitiateSMS = 1,
    //  PTPBroken = 2,
    //  PTPCreated = 3,
    //  NoContact = 4,
    //  PaymentThanks = 5,
    //  NoContactPTPBroken = 6,
    //  PTCBroken = 7,
    //  PTCCreated = 8,
    //  FirstWarning = 9,
    //  SecondWarning = 10,
    //  ThirdWarning = 11
    //}

    public enum TransactionType
    {
      [Description("Refund")] Refund = 1,
      [Description("Adjustment")] Adjustment = 2,
      [Description("Discount")] Discount = 3,
      [Description("Early Settlement")] EarlySettlement = 4,
      [Description("Payment")] Payment = 5,
      [Description("Cancel")] Cancel = 6,
      [Description("Write-Off")] WriteOff = 7,
      [Description("Reschedules")] Reschedules = 8,
      [Description("Handover")] Handover = 9,
      [Description("Journal")] Journal = 10,
      [Description("Part Payment")] PartPayment = 11,
      [Description("Scheduled")] Scheduled = 12,
      [Description("Split Repay")] SplitRepay = 13,
      [Description("Instalment")] Instalment = 14
    }

    public enum TransactionStatus
    {
      [Description("Reversal")] Reversal = 1,
      [Description("Paid")] Paid = 2,
      [Description("Part Payment")] PartPayment = 3,
      [Description("Handover")] Handover = 4,
      [Description("Journal")] Journal = 5,
      [Description("Cancel")] Cancel = 6,
      [Description("Write-Off")] WriteOff = 7,
      [Description("Refund")] Refund = 8,
      [Description("Early Settlement")] EarlySettlement = 9
    }
  }

  public class Action
  {
    public enum Type
    {
      [Description("Normal")] Normal = 1,
      [Description("Action")] Action = 2,
      [Description("Reminder")] Reminder = 3
    }
  }

  public class CaseStatus
  {
    public enum Type
    {
      [Description("New")] New = 1,
      [Description("In Progress")] InProgress = 2,
      [Description("On Hold")] OnHold = 3,
      [Description("Handed Over")] HandedOver = 4,
      [Description("Closed")] Closed = 5
    }
  }

  public class Category
  {
    public enum Type
    {
      [Description("Possible Handover")] PossibleHandover = 1,
      [Description("Next Possible Handover")] NextPossibleHandover = 2,
      [Description("Arrears")] Arrears = 3,
      [Description("Sales")] Sales = 4
    }
  }
}