using System;
using System.Linq;

namespace Atlas.Common.Utils
{
  public static class DateUtils
  {
    public static DateTime GetMonthStartDate(DateTime date)
    {
      return new DateTime(date.Year, date.Month, 1);
    }

    public static DateTime GetMonthEndDate(DateTime date)
    {
      return GetMonthStartDate(date).AddMonths(1).AddDays(-1);
    }

    /// <summary>
    /// Returns the highest value of the passed params
    /// </summary>
    /// <param name="dateValues"></param>
    /// <returns>the highest date value including nulls</returns>
    public static DateTime? MaxDateTime(params DateTime?[] dateValues)
    {
      return dateValues.OrderByDescending(d => d).FirstOrDefault();
    }

    /// <summary>
    /// Return the lowest value from the list of dates passed
    /// </summary>
    /// <param name="dateValues"></param>
    /// <returns>returns the lowest date value including nulls</returns>
    public static DateTime? MinDateTime(params DateTime?[] dateValues)
    {
      return dateValues.OrderBy(d => d).FirstOrDefault();
    }
  }
}
