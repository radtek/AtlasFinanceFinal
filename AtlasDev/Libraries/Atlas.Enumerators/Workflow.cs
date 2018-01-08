using System;
using System.ComponentModel;

namespace Atlas.Enumerators
{
  public class Workflow
  {
    public enum WorkflowProcess
    {
      [Description("AtlasOnline")]
      AtlasOnline = 1
    }

    public enum WorkflowDirection
    {
      Forward = 1,
      Backward = 2,
      Jump = 3
    }

    public enum EscalationLevel
    {
      [Description("Level 1")]
      Level1 = 1,
      [Description("Level 2")]
      Level2 = 2,
      [Description("Level 3")]
      Level3 = 3
    }

    public enum WorkflowDataExtType
    {
      [Description("Atlas.Workflow.ProcessDataExt.SendStatement.SendStatement")]
      SendStatment = 1
    }

    public enum ScheduleProcessStatus
    {
      [Description("Ready")]
      Ready = 1,
      [Description("Executing")]
      Executing = 2,
      [Description("Disabled")]
      Disabled = 3
    }

    public enum ScheduleFrequency
    {
      [Description("Once-Off")]
      OnceOff = 1,
      [Description("Hourly")]
      Hourly = 2,
      [Description("Daily")]
      Daily = 3,
      [Description("Weekly")]
      Weekly = 4,
      [Description("Monthly")]
      Monthly = 5
    }

    public enum PeriodFrequency
    {
      [Description("Minutes")]
      Minutes = 1,
      [Description("Hours")]
      Hours = 2,
      [Description("Days")]
      Days = 3
    }

    public enum JobState
    {
      [Description("Executing")]
      Executing = 1,
      [Description("Completed")]
      Completed = 2,
      [Description("Failed")]
      Failed = 3,
      [Description("Stopped")]
      Stopped = 4,
      [Description("Pending")]
      Pending = 5,
      [Description("Ready")]
      Ready = 6,
      [Description("Faulty")]
      Faulty = 7
    }

    public enum Trigger
    {
      [Description("Event")]
      Event = 1,
      [Description("Auto")]
      Auto = 2
    }

    public enum ConditionClass
    {
      Account = 1
    }

    public enum Process
    {
      [Description("New Application")]
      AtlasOnline_NewApplication = 1
    }

    public enum ProcessStep
    {
      [Description("Processing")]
      Processing = 1,
      [Description("XDS Authentication")]
      XDSAuthentication = 2,
      [Description("Fraud")]
      Fraud = 3,
      [Description("Review")]
      Review = 4,
      [Description("Technical Error")]
      Technical_Error = 5,
      [Description("Quotation")]
      Quotation = 6,
      [Description("AVS")]
      AVS = 7,
      [Description("Payout")]
      Payout = 8
    }
  }
}