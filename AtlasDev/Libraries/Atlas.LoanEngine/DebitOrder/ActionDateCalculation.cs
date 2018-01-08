using System;
using System.Collections.Generic;
using System.Linq;


namespace Atlas.LoanEngine.DebitOrder
{
  public static class ActionDateCalculation
  {  
    #region Public Method

    public static DateTime CalculateNextInstalmentDate(DateTime baseDate, Enumerators.Account.PeriodFrequency periodFrequency)
    {
      var nextInstalmentDate = baseDate;
      switch (periodFrequency)
      {
        case Enumerators.Account.PeriodFrequency.Daily: nextInstalmentDate = baseDate.AddDays(1);
          break;
        case Enumerators.Account.PeriodFrequency.Weekly: nextInstalmentDate = baseDate.AddDays(14);
          break;
        case Enumerators.Account.PeriodFrequency.BiWeekly: nextInstalmentDate = baseDate.AddDays(14);
          break;
        case Enumerators.Account.PeriodFrequency.Monthly: nextInstalmentDate = baseDate.AddMonths(1);
          break;
      }

      return nextInstalmentDate;
    }


    public static DateTime CalculateActionDate(List<DateTime> publicHoliday, Enumerators.Account.PayRule payRule, DateTime salaryDate)
    {
      int daysInMonth = 0;

      switch (payRule)
      {
        case Enumerators.Account.PayRule.Fri_Sat_Sun_To_Mon:
          if (salaryDate.DayOfWeek == DayOfWeek.Friday || salaryDate.DayOfWeek == DayOfWeek.Saturday || salaryDate.DayOfWeek == DayOfWeek.Sunday)
          {
            while (salaryDate.DayOfWeek != DayOfWeek.Monday)
              salaryDate = salaryDate.AddDays(+1);
          }
          break;
        case Enumerators.Account.PayRule.Sat_Sun_To_Fri:
          if (salaryDate.DayOfWeek == DayOfWeek.Saturday || salaryDate.DayOfWeek == DayOfWeek.Sunday)
          {
            while (salaryDate.DayOfWeek != DayOfWeek.Friday)
              salaryDate = salaryDate.AddDays(-1);
          }
          break;
        case Enumerators.Account.PayRule.Sat_Sun_To_Mon:
          if (salaryDate.DayOfWeek == DayOfWeek.Saturday || salaryDate.DayOfWeek == DayOfWeek.Sunday)
          {
            while (salaryDate.DayOfWeek != DayOfWeek.Monday)
              salaryDate = salaryDate.AddDays(+1);
          }
          break;
        case Enumerators.Account.PayRule.Sat_To_Fri_Sun_To_Mon:
          if (salaryDate.DayOfWeek == DayOfWeek.Saturday)
          {
            while (salaryDate.DayOfWeek != DayOfWeek.Friday)
              salaryDate = salaryDate.AddDays(-1);
          }
          else if (salaryDate.DayOfWeek == DayOfWeek.Sunday)
          {
            while (salaryDate.DayOfWeek != DayOfWeek.Monday)
              salaryDate = salaryDate.AddDays(+1);
          }
          break;
        case Enumerators.Account.PayRule.Sun_To_Mon:
          if (salaryDate.DayOfWeek == DayOfWeek.Sunday)
          {
            while (salaryDate.DayOfWeek != DayOfWeek.Monday)
              salaryDate = salaryDate.AddDays(+1);
          }
          break;
        case Enumerators.Account.PayRule.Sat_Sun_Mon_To_Friday:
          if (salaryDate.DayOfWeek == DayOfWeek.Saturday || salaryDate.DayOfWeek == DayOfWeek.Sunday || salaryDate.DayOfWeek == DayOfWeek.Monday)
          {
            while (salaryDate.DayOfWeek != DayOfWeek.Friday)
              salaryDate = salaryDate.AddDays(-1);

            if (!publicHoliday.Contains(salaryDate))
              salaryDate = salaryDate.AddDays(-1);
          }
          break;
        case Enumerators.Account.PayRule.Last_Working_Day_Of_Month:
          var i = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
          while (i > 0)
          {
            var current = new DateTime(DateTime.Now.Year, DateTime.Now.Month, i);
            if (current.DayOfWeek < DayOfWeek.Saturday && current.DayOfWeek > DayOfWeek.Sunday && !publicHoliday.Contains(current))
            {
              salaryDate = current;
              i = 0;
            }
            else
            {
              i = (i - 1);
            }
          }
          break;
        case Enumerators.Account.PayRule.Second_Last_Working_Day_Of_Month:
          break;
        case Enumerators.Account.PayRule.Last_Sunday:
          salaryDate = LastDateCalculation(DayDirection.Backward, DayOfWeek.Sunday);

          break;
        case Enumerators.Account.PayRule.Last_Monday:
          salaryDate = LastDateCalculation(DayDirection.Backward, DayOfWeek.Monday);

          break;
        case Enumerators.Account.PayRule.Last_Tuesday:
          salaryDate = LastDateCalculation(DayDirection.Backward, DayOfWeek.Tuesday);

          break;
        case Enumerators.Account.PayRule.Last_Wednesday:
          salaryDate = LastDateCalculation(DayDirection.Backward, DayOfWeek.Wednesday);

          break;
        case Enumerators.Account.PayRule.Last_Thursday:
          salaryDate = LastDateCalculation(DayDirection.Backward, DayOfWeek.Thursday);

          break;
        case Enumerators.Account.PayRule.Last_Friday:
          salaryDate = LastDateCalculation(DayDirection.Backward, DayOfWeek.Friday);

          break;
        case Enumerators.Account.PayRule.Last_Saturday:
          salaryDate = LastDateCalculation(DayDirection.Backward, DayOfWeek.Saturday);

          break;
        case Enumerators.Account.PayRule.Second_Last_Friday:
          daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

          salaryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, daysInMonth);
          int fridayCount = 0;
          while (fridayCount <= 1)
          {
            salaryDate = salaryDate.AddDays(-1);

            if (salaryDate.DayOfWeek == DayOfWeek.Friday)
              fridayCount += 1;
          }
          break;
        case Enumerators.Account.PayRule.Friday_Before_Or_On_the_25th:
          if (salaryDate.DayOfWeek == DayOfWeek.Saturday)
            salaryDate = salaryDate.AddDays(-1);
          else if (salaryDate.DayOfWeek == DayOfWeek.Sunday)
            salaryDate = salaryDate.AddDays(-2);
          else if (salaryDate.DayOfWeek == DayOfWeek.Monday)
            salaryDate = salaryDate.AddDays(-3);
          break;
        default:
          break;
      }

      return salaryDate;
    }

    #endregion


    #region Private Methods

    private static DateTime LastDateCalculation(DayDirection direction, DayOfWeek evaluationDate)
    {
      var daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

      var totalMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, daysInMonth);

      while (totalMonth.DayOfWeek != evaluationDate)
      {
        if (direction == DayDirection.Backward)
          totalMonth = totalMonth.AddDays(-1);
        else
          totalMonth = totalMonth.AddDays(+1);
      }

      return totalMonth;
    }

    #endregion


    #region Private Properties

    private enum DayDirection
    {
      Forward = 1,
      Backward = 2
    }

    #endregion

  }
}