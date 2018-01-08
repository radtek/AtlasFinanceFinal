/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty) Ltd.
 *  
 * 
 *  Description:
 *  ------------------
 *    Integer extension methods
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
 * ----------------------------------------------------------------------------------------------------------------- */
using System;


namespace Atlas.Common.Extensions
{
  /// <summary>
  /// Extension methods for integers
  /// </summary>
  public static class IntegerExtensions
  {
    /// <summary>
    /// Round off a value to the nearest 'roundValueTo' value
    /// </summary>
    /// <param name="value"></param>
    /// <param name="roundValueTo"></param>
    /// <returns></returns>
    public static int RoundOff(this int value, int roundValueTo)
    {
      return ((int)Math.Round(value / roundValueTo * 1.0)) * roundValueTo;
    }

    public static string ToOrdinalString(this int num)
    {
      switch (num % 100)
      {
        case 11:
        case 12:
        case 13:
          return string.Format("{0}th", num);
      }

      switch (num % 10)
      {
        case 1:
          return string.Format("{0}st", num);
        case 2:
          return string.Format("{0}nd", num);
        case 3:
          return string.Format("{0}rd", num);
        default:
          return string.Format("{0}th", num);
      }
    }
  }
}
