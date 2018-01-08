using System;
using System.ComponentModel;

namespace Atlas.Enumerators
{
  public sealed class Credit
  {
    public enum Report
    {
      [Description("Summary")]
      Summary = 0,
      [Description("NLR")]
      NLR = 1,
      [Description("CC")]
      CC = 2
    }
  }
}
