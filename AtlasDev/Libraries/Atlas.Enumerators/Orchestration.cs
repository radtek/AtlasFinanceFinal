using System;
using System.ComponentModel;

namespace Atlas.Enumerators
{
  public class Orchestration
  {
    public enum AVSTransaction
    {
      [Description("Duration Exceeded")]
      Duration_Exceeded = 1,
      [Description("Do AVS")]
      Do_AVS = 2,
      [Description("AVS Current")]
      AVS_Current = 3,
      [Description("AVS Failed")]
      AVS_Failed = 4,
      [Description("AVS Pending")]
      AVS_Pending = 5,
      [Description("Error")]
      Error = 6
    }
  }
}
