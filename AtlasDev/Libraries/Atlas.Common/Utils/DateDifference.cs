/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Date Difference helper    
 * 
 * 
 *  Author:
 *  ------------------
 *     Fabian Franco-Roldan
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-09-03 - Initial revision
 * 
 *     
 * ----------------------------------------------------------------------------------------------------------------- */
using System;


namespace Atlas.Common.Utils
{
  public class DateDifference
  {
    #region Private Members

    /// <summary>
    /// Defining Number of days in month; index 0=> january and 11=> December
    /// february contain either 28 or 29 days, that's why here value is -1
    /// which will be calculate later.
    /// </summary>
    private readonly int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

    /// <summary>
    /// contain from date
    /// </summary>
    private readonly DateTime fromDate;

    /// <summary>
    /// contain To Date
    /// </summary>
    private readonly DateTime toDate;

    /// <summary>
    /// this three variable for output representation..
    /// </summary>
    private readonly int year;
    private readonly int month;
    private readonly int day;

    #endregion

    #region Constructor

    public DateDifference(DateTime d1, DateTime d2)
    {
      var increment = 0;

      if (d1 > d2)
      {
        fromDate = d2;
        toDate = d1;
      }
      else
      {
        fromDate = d1;
        toDate = d2;
      }

      /// 
      /// Day Calculation
      /// 

      if (fromDate.Day > toDate.Day)
      {
        increment = monthDay[fromDate.Month - 1];
      }
      /// if it is February month
      /// if it's to day is less then from day
      if (increment == -1)
      {
        increment = DateTime.IsLeapYear(fromDate.Year) ? 29 : 28;
      }
      if (increment != 0)
      {
        day = (toDate.Day + increment) - fromDate.Day;
        increment = 1;
      }
      else
      {
        day = toDate.Day - fromDate.Day;
      }

      ///
      ///month calculation
      ///
      if ((fromDate.Month + increment) > toDate.Month)
      {
        month = (toDate.Month + 12) - (fromDate.Month + increment);
        increment = 1;
      }
      else
      {
        month = (toDate.Month) - (fromDate.Month + increment);
        increment = 0;
      }

      ///
      /// year calculation
      ///
      year = toDate.Year - (fromDate.Year + increment);

    }

    #endregion

    #region Public Members

    public override string ToString()
    {
      //return base.ToString();
      return String.Format("{0} Year{1}, {2} month{3}, {4} day{5}",
        year, year > 1 ? "s" : string.Empty,
        month, month > 1 ? "s" : string.Empty,
        day, day > 1 ? "s" : string.Empty);
    }


    public int Years
    {
      get { return year; }
    }


    public int Months
    {
      get { return month; }
    }


    public int Days
    {
      get { return day; }
    }

    #endregion

  }
}
