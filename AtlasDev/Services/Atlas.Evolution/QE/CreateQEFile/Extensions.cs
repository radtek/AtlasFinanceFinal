using System;


namespace Atlas.Evolution.Server.Code.Utils
{
  public static class Extensions
  {
    public static uint ToYyyyMmDd(this DateTime? date)
    {     
      return (date != null && date.Value != DateTime.MinValue) ? (uint)(date.Value.Year * 10000 + date.Value.Month * 100 + date.Value.Day) : 0;
    }

    public static string ToYyyyMmDdString(this DateTime? date)
    {
      return (date != null && date.Value != DateTime.MinValue) ? string.Format("{0:yyyyMMdd}", date.Value) : "00000000";
    }


    public static uint ToYyyyMmDd(this DateTime date)
    {
      return (date != DateTime.MinValue) ? (uint)(date.Year * 10000 + date.Month * 100 + date.Day) : 0;
    }

    public static string ToYyyyMmDdString(this DateTime date)
    {
      return (date != DateTime.MinValue) ? string.Format("{0:yyyyMMdd}", date) : "00000000";
    }

  }
}
