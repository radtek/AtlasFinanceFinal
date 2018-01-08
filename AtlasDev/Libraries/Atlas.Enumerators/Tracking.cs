using System;
using System.ComponentModel;

namespace Atlas.Enumerators
{
  public class Tracking
  {
    public enum AlertType
    {
      [Description("Email")]
      Email = 0,
      [Description("SMS")]
      SMS = 1
    }

    public enum SeverityClassification
    {
      [Description("Minor")]
      Minor = 0,
      [Description("Low")]
      Low = 1,
      [Description("Medium")]
      Medium = 2,
      [Description("High")]
      High =3,
      [Description("Critical")]
      Critical = 4
    }

    public enum Elapse
    {
      [Description("Minutes")]
      Minutes = 0,
      [Description("Hour(s)")]
      Hours = 1,
      [Description("Day(s)")]
      Days = 2
    }
  }
}
