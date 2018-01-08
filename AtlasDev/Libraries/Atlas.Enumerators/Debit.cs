using System.ComponentModel;

namespace Atlas.Enumerators
{
  public class Debit
  {
    public enum DebitType
    {
      [Description("Regular")]
      Regular = 1,
      [Description("Additional")]
      Additional = 2
    }

    public enum FailureType
    {
      [Description("Continue")]
      Continue = 1,
      [Description("Break")]
      Break = 2,
      [Description("Retry")]
      Retry = 3
    }

    public enum Status
    {
      [Description("New")]
      New = 1,
      [Description("Cancelled")]
      Cancelled = 2,
      [Description("On Hold")]
      OnHold = 3,
      [Description("Batched")]
      Batched = 4,
      [Description("Submitted")]
      Submitted = 5,
      [Description("Successful")]
      Successful = 6,
      [Description("Failed")]
      Failed = 7,
      [Description("Disputed")]
      Disputed = 8
    }

    public enum ControlType
    {
      [Description("Predefined")]
      Predefined = 1,
      [Description("Predictive")]
      Predictive = 2
    }

    public enum ControlStatus
    {
      [Description("New")]
      New = 1,
      [Description("In Process")]
      InProcess = 2,
      [Description("Completed")]
      Completed = 3,
      [Description("Cancelled")]
      Cancelled = 4,
      [Description("Cancelled - Validation Errors")]
      Cancelled_ValidationErrors = 5,
      [Description("Completed With Failed Debit")]
      CompletedWithFailedDebit = 6
    }

    public enum BatchStatus
    {
      [Description("New")]
      New = 1,
      [Description("Validated with Errors")]
      ValidatedWithErrors = 2,
      [Description("Submitted - Waiting in Outbox")]
      SubmittedWaitingOutbox = 3,
      [Description("Submitted - Waiting for Reply")]
      SubmittedWaitingForReply = 4,
      [Description("Submitted - Waiting for Output")]
      SubmittedWaitingForOutput = 5,
      [Description("Completed")]
      Completed = 6,
      [Description("Completed with Errors")]
      CompletedWithErrors = 7,
      [Description("Failed")]
      Failed = 8
    }

    public enum ValidationType
    {
      [Description("Invalid Account Status")]
      InvalidAccountStatus = 1,
      [Description("Bank Account Inactive / Invalid")]
      BankAccountInvalidOrInactive = 2,
      [Description("Account within closing balancing threshold")]
      AccountWithinClosingBalance = 3,
      [Description("Account does not exist")]
      AccountDoesNotExist = 4,
      [Description("Action date has passed")]
      ActionDateHasPassed = 5,
      [Description("AVS does not exist or is still pending")]
      AVSDoesNotExistOrPending= 6
    }

    public enum TrackingDay
    {
      [Description("No Tracking")]
      No_Track = 0,
      [Description("1 Day Tracking")]
      One_Day_Tracking = 1,
      [Description("2 Day Tracking")]
      Two_Day_Tracking = 2,
      [Description("3 Day Tracking")]
      Three_Day_Tracking = 3,
      [Description("4 Day Tracking")]
      Four_Day_Tracking = 4,
      [Description("5 Day Tracking")]
      Five_Day_Tracking = 5,
      [Description("6 Day Tracking")]
      Six_Day_Tracking = 6,
      [Description("7 Day Tracking")]
      Seven_Day_Tracking = 7,
      [Description("8 Day Tracking")]
      Eight_Day_Tracking = 8,
      [Description("9 Day Tracking")]
      Nine_Day_Tracking = 9,
      [Description("10 Day Tracking")]
      Ten_Day_Tracking = 10,
      [Description("14 Day Tracking")]
      Fourteen_Day_Tracking = 14,
      [Description("21 One Day Tracking")]
      TwentyOne_Day_Tracking = 21,
      [Description("32 Day Tracking")]
      ThirtyTwo_Day_Tracking = 32,
      [Description("NAEDO Recall")]
      NAEDO_Recall = 50
    }

    public enum AVSCheckType
    {
      [Description("None")]
      None = 1,
      [Description("Control Creation")]
      ControlCreation = 2,
      [Description("On Every Debit")]
      OnEveryDebit = 3
    }
  }
}