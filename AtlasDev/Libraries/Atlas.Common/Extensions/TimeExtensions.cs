using System;

namespace Atlas.Common.Extensions
{
  /// <summary>
  /// Round DateTime values to specific intervals
  /// http://stackoverflow.com/questions/1393696/rounding-datetime-objects
  /// </summary>
  public static class TimeExtensions
  {
    private static DateTime Floor(DateTime dateTime, TimeSpan interval)
    {
      return dateTime.AddTicks(-(dateTime.Ticks % interval.Ticks));
    }


    private static DateTime Ceiling(DateTime dateTime, TimeSpan interval)
    {
      var overflow = dateTime.Ticks % interval.Ticks;
      return overflow == 0 ? dateTime : dateTime.AddTicks(interval.Ticks - overflow);
    }


    private static DateTime Round(DateTime dateTime, TimeSpan interval)
    {
      var halfIntervalTicks = (interval.Ticks + 1) >> 1;
      return dateTime.AddTicks(halfIntervalTicks - ((dateTime.Ticks + halfIntervalTicks) % interval.Ticks));
    }

  }
}
