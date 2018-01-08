/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Useful dBASE/PostgreSQL utilities
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
using System.Text;

using SocialExplorer.IO.FastDBF;


namespace Atlas.Data.DBF.Utils
{
  public static class PSQLUtils
  {
    /// <summary>
    /// Creates a 'safe' PostgreSQL SQL string representation of the DBF column value
    /// </summary>
    /// <param name="p"></param>
    /// <returns>PostgreSQL SQL string representation of the value</returns>
    public static string PSQLStringyfy(string value, DbfColumn.DbfColumnType type, int length)
    {
      switch (type)
      {
        case DbfColumn.DbfColumnType.Character:
          if (string.IsNullOrWhiteSpace(value))
          {
            return string.Format("'{0}'", "".PadLeft(length));
          }

          #region Convert ASCII Characters > 127 to escaped \xHH  http://www.postgresql.org/docs/current/static/sql-syntax-lexical.html#SQL-SYNTAX-STRINGS-ESCAPE
          var valueBytes = ASCIIEncoding.ASCII.GetBytes(value);
          var cleanedBytes = new byte[valueBytes.Length * 4]; // worst case- entire string as a hex representation
          int currPos = 0;
          var escaped = string.Empty;
          var currLen = 0;
          for (int i = 0; i < valueBytes.Length; i++)
          {
            // NOTE I ignore bytes below ASCII 32 and above 165- these should not be present in string data!
            if (valueBytes[i] == 92) // \ Backslash
            {
              escaped = "E";
              cleanedBytes[currPos++] = 92;
              cleanedBytes[currPos++] = 92;
              currLen++;
            }
            else if (valueBytes[i] == 39) // ' Single apostrophe
            {
              escaped = "E";
              cleanedBytes[currPos++] = 92;
              cleanedBytes[currPos++] = 39;
              currLen++;
            }
            else if (valueBytes[i] >= 32 && valueBytes[i] < 127)
            {
              cleanedBytes[currPos++] = valueBytes[i];
              currLen++;
            }
            else if (valueBytes[i] > 127 && valueBytes[i] <= 165)
            {
              escaped = "E";
              //  Do \xHH where HH is hex
              cleanedBytes[currPos++] = 92;  // Blackslash
              cleanedBytes[currPos++] = 120; // 'h'
              var hexVal = valueBytes[i].ToString("X2");
              cleanedBytes[currPos++] = ASCIIEncoding.ASCII.GetBytes(hexVal)[0];
              cleanedBytes[currPos++] = ASCIIEncoding.ASCII.GetBytes(hexVal)[1];
              currLen++;
            }
          }
          #endregion
          while (currLen++ < length)
          {
            cleanedBytes[currPos++] = 32;
          }
          return string.Format("{0}'{1}'", escaped, ASCIIEncoding.ASCII.GetString(cleanedBytes, 0, currPos));

        case DbfColumn.DbfColumnType.Date:
          if (string.IsNullOrWhiteSpace(value))
          {
            return "null";
          }
          return string.Format("'{0}-{1}-{2} 00:00:00'", value.Substring(0, 4), value.Substring(4, 2), value.Substring(6, 2));

        case DbfColumn.DbfColumnType.Number:
          if (string.IsNullOrWhiteSpace(value))
          {
            return "0";
          }
          Decimal val;
          if (!Decimal.TryParse(value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign, 
            System.Globalization.CultureInfo.InvariantCulture, out val))
          {
            throw new Exception(string.Format("Number column contains invalid characters: '{0}'", value));
          }
          return value.Trim();

        case DbfColumn.DbfColumnType.Boolean:
          if (string.IsNullOrWhiteSpace(value))
          {
            return "null";
          }
          return value == "T" ? "true" : "false";


        default:
          throw new Exception(string.Format("Unknown data type {0}", type.ToString()));
      }
    }


    /// <summary>
    /// Returns SQL data type of the DBF column 
    /// </summary>
    /// <param name="colType"></param>
    /// <returns>string containing SQL data type</returns>
    public static string TypeAsSQLType(DbfColumn.DbfColumnType colType)
    {
      if (colType == DbfColumn.DbfColumnType.Character)
      {
        return "character";
      }
      else if (colType == DbfColumn.DbfColumnType.Date)
      {
        return "date";
      }
      else if (colType == DbfColumn.DbfColumnType.Number)
      {
        return "numeric";
      }
      else if (colType == DbfColumn.DbfColumnType.Boolean)
      {
        return "bool";
      }
      else
      {
        throw new Exception(string.Format("Unxpected data type {0}", colType.ToString()));
      }
    }

  }
}
