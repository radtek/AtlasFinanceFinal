using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Workflow.Common
{
  public static class Util
  {
    public static DateTime GetNextSundayDate()
    {
      return DateTime.Today.AddDays(7 - (int)DateTime.Today.DayOfWeek);
    }

    public static int ConvertToMilliseconds(int interval, Enumerators.Workflow.PeriodFrequency? periodFrequency)
    {
      var delay = 0;
      switch (periodFrequency)
      {
        case Enumerators.Workflow.PeriodFrequency.Minutes:
          delay = interval * 60 * 1000;
          break;
        case Enumerators.Workflow.PeriodFrequency.Hours:
          delay = interval * 60 * 60 * 1000;
          break;
        case Enumerators.Workflow.PeriodFrequency.Days:
          delay = interval * 24 * 60 * 60 * 1000;
          break;
      }
      return delay;
    }
  }
}
