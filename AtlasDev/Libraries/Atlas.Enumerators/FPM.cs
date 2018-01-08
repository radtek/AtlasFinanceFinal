using System;
using System.ComponentModel;

namespace Atlas.Enumerators
{
  public class FPM
  {
    public enum DecisionOutCome
    {
      [Description("Accept")]
      Accept = 1,
      [Description("Reject")]
      Reject = 2,
      [Description("Review")]
      Review = 3
    }
  }
}
