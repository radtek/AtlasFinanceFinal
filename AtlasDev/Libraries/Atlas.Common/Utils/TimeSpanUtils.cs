/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty) Ltd.
 * 
 *  Description:
 *  ------------------
 *    Useful string utilities
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *       
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;


namespace Atlas.Common.Utils
{
  /// <summary>
  /// Useful TimeSpan
  /// </summary>
  public static class TimeSpanUtils
  {
    /// <summary>
    /// Returns a TimeSpan in a simple, readable English format
    /// </summary>
    /// <param name="value">The timespan</param>
    /// <returns>English string containing a simplified representation of the timespan</returns>
    public static string GetReadableTimeSpan(TimeSpan value)
    {
      string duration;

      if (value.TotalMinutes < 1)
        duration = string.Format("{0} second{1}", value.Seconds, value.Seconds == 1 ? string.Empty : "s");

      else if (value.TotalHours < 1)
        duration = string.Format("{0} minute{1}, {2} second{3}",
          value.Minutes, value.Minutes == 1 ? string.Empty : "s",
          value.Seconds, value.Seconds == 0 ? string.Empty : "s");

      else if (value.TotalDays < 1)
        duration = string.Format("{0} hour{1}, {2} minute{3}",
          value.Hours, value.Hours == 1 ? string.Empty : "s",
          value.Minutes, value.Minutes == 0 ? string.Empty : "s");
      else
        duration = string.Format("{0} day{1}, {2} hour{3}",
          value.Days, value.Days == 1 ? string.Empty : "s",
          value.Hours, value.Hours == 1 ? string.Empty : "s");

      return duration;
    }
  }
}
