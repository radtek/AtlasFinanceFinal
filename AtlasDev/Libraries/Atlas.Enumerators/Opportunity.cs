using System;
using System.ComponentModel;

namespace Atlas.Enumerators
{
  public sealed class Opportunity
  {
    public enum OpportunityStatus
    {
      [Description("Not set")]
      NotSet = 0,

      [Description("New")]
      New = 1,

      [Description("Successful")]
      Successful = 2,

      [Description("Unsuccessful")]
      Unsuccessful = 3
    }



  }
}
