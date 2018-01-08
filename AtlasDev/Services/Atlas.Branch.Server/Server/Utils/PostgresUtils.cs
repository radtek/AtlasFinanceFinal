/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Useful PostgreSQL utilities
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
using System.Globalization;
using System.Text;


namespace Atlas.Data.Utils
{
  public static class PostgresUtils
  {
    /// <summary>
    /// Creates a safe SQL textual expression of given value, with unicode
    /// </summary>   
    /// <param name="value">The value to be converted to a string</param>
    /// <param name="length"></param>
    /// <returns>A PostgreSQL safe representation of the objects value</returns>
    public static string PSQLStringyfy(object value, string fieldName = "")
    {
      if (value == null || value == DBNull.Value)
      {
        return "null";
      }

      try
      {
        var valueType = value.GetType();

        if (valueType == typeof(string))
        {
          #region Create safe string using unicode escaping, if string contains unsafe/non basic ASCII characters
          var stringVal = value as string;
          var sb = new StringBuilder();

          var valueBytes = UTF8Encoding.UTF8.GetBytes(stringVal);
          var escaped = string.Empty;           // The PostgreSQL escape prefix (if required)
          for (var i = 0; i < valueBytes.Length; i++)
          {
            if (valueBytes[i] >= 32 && valueBytes[i] < 127 && valueBytes[i] != 92 && valueBytes[i] != 96 && valueBytes[i] != 39) // Avoid: ' \ `
            {
              sb.Append((char)valueBytes[i]);
            }
            else
            {
              escaped = "U&";
              //  Do unicode
              sb.AppendFormat("\\{0:x4}", valueBytes[i]);
            }
          }
          #endregion

          return string.Format("{0}'{1}'", escaped, sb.ToString());
        }

        else if (valueType == typeof(DateTime))
        {
          var dateVal = value as DateTime?;
          return string.Format("'{0:yyyy-MM-dd HH:mm:ss}'", dateVal);
        }

        else if (valueType == typeof(Decimal))
        {
          var decimalVal = value as Decimal?;
          return decimalVal.Value.ToString("F2", CultureInfo.InvariantCulture);
        }

        else if (valueType == typeof(float))
        {
          var floatVal = value as float?;
          return floatVal.Value.ToString("F2", CultureInfo.InvariantCulture);
        }

        else if (valueType == typeof(Boolean))
        {
          var booleanVal = value as bool?;
          return booleanVal.Value ? "true" : "false";
        }

        else if (valueType == typeof(double))
        {
          var doubleVal = value as double?;
          return fieldName == "sr_recno" ? doubleVal.Value.ToString("F0") : doubleVal.Value.ToString("F4", CultureInfo.InvariantCulture);
        }

        else
        {
          throw new Exception(string.Format("Unknown data type {0}", valueType.ToString()));
        }
      }
      catch
      {
        throw;
      }
    }

  }
}
