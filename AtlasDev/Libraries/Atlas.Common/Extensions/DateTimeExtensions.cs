using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Common.Extensions
{
  public static class DateTimeExtensions
  {
    public static DateTime AddWorkdays(this DateTime originalDate, int workDays)
    {
      return AddWorkDaysWithHolidays(originalDate, workDays, new List<DateTime>());
    }

    public static DateTime AddWorkdays(this DateTime originalDate, int workDays, List<DateTime> publicHolidays)
    {
      return AddWorkDaysWithHolidays(originalDate, workDays, publicHolidays);
    }

    private static DateTime AddWorkDaysWithHolidays(DateTime originalDate, int workDays, List<DateTime> publicHolidays)
    {
      var tmpPublicHolidays = publicHolidays.Select(p => p.Date).ToList();
      if (workDays <= 0)
      {
        return originalDate;
      }
      var tmpWorkDays = workDays;
      for (var i = 1; i <= tmpWorkDays; i++)
      {
        var tempDate = originalDate.Date.AddDays(i);
        if (tempDate.DayOfWeek == DayOfWeek.Sunday || tempDate.DayOfWeek == DayOfWeek.Saturday)
        {
          tmpWorkDays++;
        }
        else if (tmpPublicHolidays.Contains(tempDate))
        {
          tmpWorkDays++;
        }
      }

      return originalDate.AddDays(tmpWorkDays);
    }

  }
}
