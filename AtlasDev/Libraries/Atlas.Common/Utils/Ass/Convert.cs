/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2014 Atlas Finance (Pty) Ltd.
 * 
 *  Description:
 *  ------------------
 *    ASS conversion utils
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *    2014-06-18- Created
 *    
 *    2014-06-24- V1 stable- had to remove a lot of characters to comply with DX Snap reporting
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * 
 * 
 * 
 * 
 * Sample PRG value (line-breaks added):
 * ------------------------------------------------------------------------------
  * 
{ "OP10" => "16:46:08", "OP4" => sToD( '20061011' ),
* "OP5" => .T., "OP6" =>   11111111, "OP7" => 04/26/07 13:30:45.670,
"OP9" => sToD( '20140613' ), 
"OPT1" => "2134234!@#$%^&*()'", 
"OPT2" => E"34534543'!@#$%^\/{}&*()?><:{}[]|\"", 
"OPT3" => 123.33, "TEST" => "Testing" }

 Create:
 ----------------------------
   local hHash := Hash()
   hHash["Key"] := AValue
   string = ValToPrg( hHash )

*/


namespace Atlas.Common.Utils.Ass
{
  public static class Convert
  {
    /// <summary>
    /// Converts an xHarbour PRG hash string representation to a Dictionary<string, object>
    /// </summary>
    /// <param name="prgString">The PRG hash string</param>
    /// <returns>A best-effort Dictionary version of the xharbour hash PRG string</returns>
    public static Dictionary<string, object> PrgToDictionary(string prgString)
    {
      #region Check parameter
      prgString = prgString.TrimEnd(new char[] { ' ', (char)26 });
      if (string.IsNullOrEmpty(prgString) || !prgString.StartsWith("{") || !prgString.EndsWith("}"))
      {
        throw new FormatException("String must start and end with curly braces");
      }
      if (!prgString.Contains("=>"))
      {
        throw new FormatException("String must contain '=>' between hash key and its associated value");
      }
      #endregion

      var values = new Dictionary<string, object>();
      var chars = prgString.ToArray();

      var keyName = new StringBuilder();
      var value = new StringBuilder();
      var stage = ParseStage.KeyWait;
      var valueType = ValueTypeDetected.IsBoolean;
      var nextCharIsEscaped = false;
      var keyDelimiter = '\"';
      
      foreach (var ch in chars)
      {
        switch (stage)
        {
          #region Waiting for key to start. Default key is delimited with ", if key contains a ", will be delimited with ', if contains ' and ", with be delimited with [ ]
          case ParseStage.KeyWait:
            if (ch == '\"' || ch == '\'' || ch == '[')
            {
              stage = ParseStage.KeyName;
              keyDelimiter = ch == '[' ? ']' : ch;
              keyName.Clear();
            }
            else if ((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z')) // function
            {
              stage = ParseStage.KeyName;
              keyDelimiter = ' ';
              keyName.Clear();
              keyName.Append(ch);
            }
            break;
          #endregion

          #region In key name
          case ParseStage.KeyName:
            if (ch == keyDelimiter)
            {
              stage = ParseStage.KeyEnd;
            }
            else
            {
              keyName.Append(ch);
            }
            break;
          #endregion

          #region Key name ended- waiting for '>' to indicate value
          case ParseStage.KeyEnd:
            if (ch == '>')
            {
              stage = ParseStage.ValueWait;
              valueType = ValueTypeDetected.Unknown;
              nextCharIsEscaped = false;
              value.Clear();
            }
            break;
          #endregion

          #region Waiting for value to start
          case ParseStage.ValueWait:
            if (ch == 'E')
            {
              valueType = ValueTypeDetected.IsEscapedString;
            }
            else if (ch == '\"')
            {
              if (valueType != ValueTypeDetected.IsEscapedString)
              {
                valueType = ValueTypeDetected.IsString;
              }
              stage = ParseStage.InValue;
            }
            else if (ch == '.') // boolean
            {
              valueType = ValueTypeDetected.IsBoolean;
              stage = ParseStage.InValue;
            }
            else if (ch >= '0' && ch <= '9')
            {
              value.Append(ch);
              valueType = ValueTypeDetected.IsNumber;
              stage = ParseStage.InValue;
            }
            else if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'))
            {
              value.Append(ch);
              valueType = ValueTypeDetected.IsFunction;
              stage = ParseStage.InValue;
            }
            else if (ch != ' ')
            {
              throw new FormatException(string.Format("Unexpected value starting character: '{0}'", ch));
            }
            break;
          #endregion

          #region Process the value
          case ParseStage.InValue:
            if (valueType == ValueTypeDetected.IsString || valueType == ValueTypeDetected.IsEscapedString)
            {
              if (valueType == ValueTypeDetected.IsEscapedString && ch == '\\')
              {
                nextCharIsEscaped = true;
              }
              else if (valueType == ValueTypeDetected.IsEscapedString && nextCharIsEscaped)
              {
                if (ch == '\"' || ch == '\\')
                {
                  value.Append(ch);
                }
                //else if (ch == 'r')
                //{
                //  value.AppendFormat("\r");
                //}

                nextCharIsEscaped = false;
              }
              else if ((valueType == ValueTypeDetected.IsString || valueType == ValueTypeDetected.IsEscapedString) && ch == '\"')
              {
                stage = ParseStage.KeyWait;
                if (!(value.Length > 0 && value[0] == 27))
                {
                  values.AddSafe(keyName.ToString(), value.ToString());
                }
              }
              else
              {
                value.Append(ch);
              }
            }

            else if (valueType == ValueTypeDetected.IsBoolean)
            {
              if (ch == 'T' || ch == 't')
              {
                values.AddSafe(keyName.ToString(), true);
              }
              else if (ch == 'F' || ch == 'f')
              {
                values.AddSafe(keyName.ToString(), false);
              }
              else
              {
                throw new FormatException(string.Format("Unexpected character for boolean value: '{0}'", ch));
              }
              stage = ParseStage.KeyWait;
            }

            else if (valueType == ValueTypeDetected.IsNumber)
            {
              if (ch == ',' || ch == '}')
              {
                var final = value.ToString().Trim();
                var splitted = final.Split(new char[] { '/', ':', '.', ' ' });

                // Is date: 04/26/07 13:30:45.670
                if (final.Length == 21 && splitted.Length == 7)
                {
                  DateTime dateVal;
                  if (!DateTime.TryParseExact(final, "dd/MM/yy HH:mm:ss.fff", null, System.Globalization.DateTimeStyles.None, out dateVal))
                  {
                    throw new FormatException(string.Format("Invalid date (check the 'SET DATE FORMAT' setting): '{0}'", final));
                  }

                  values.AddSafe(keyName.ToString(), dateVal);
                }
                // Is date 04/26/2007 13:30:45.670
                else if (final.Length == 23 && splitted.Length == 7)
                {
                  DateTime dateVal;
                  if (!DateTime.TryParseExact(final, "dd/MM/yyyy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out dateVal))
                  {
                    throw new FormatException(string.Format("Invalid date (check the 'SET DATE FORMAT' setting): '{0}'", final));
                  }

                  values.AddSafe(keyName.ToString(), dateVal);
                }
                else
                {
                  Decimal decVal;
                  if (!Decimal.TryParse(value.ToString(), System.Globalization.NumberStyles.Number,
                    System.Globalization.NumberFormatInfo.InvariantInfo, out decVal))
                  {
                    throw new FormatException(string.Format("Invalid number: '{0}'", value.ToString()));
                  }

                  values.AddSafe(keyName.ToString(), decVal);
                }
                stage = ParseStage.KeyWait;
              }
              else
              {
                value.Append(ch);
              }
            }

            else if (valueType == ValueTypeDetected.IsFunction)
            {
              if (ch == ',' || ch == '}')
              {
                var final = value.ToString().Trim();

                // Is sToD?
                if (final.ToLower().StartsWith("stod("))
                {
                  int end = 0;
                  var start = final.IndexOf("'");
                  if (start > 0)
                  {
                    end = final.IndexOf("'", start + 1);
                  }
                  if (end > start && end - start == 9)
                  {
                    DateTime dateVal;
                    if (!DateTime.TryParseExact(final.Substring(start + 1, 8), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out dateVal))
                    {
                      throw new FormatException(string.Format("Invalid date: {0}", final));
                    }
                    values.AddSafe(keyName.ToString(), dateVal);
                  }
                  else
                  {
                    throw new FormatException(string.Format("Invalid StoD string value: {0}", final));
                  }
                }
                else if (final.ToLower() == "nil")
                {
                  values.AddSafe(keyName.ToString(), null);
                }
                else
                {
                  throw new FormatException(string.Format("Unknown function/value: {0}", final));
                }

                stage = ParseStage.KeyWait;
              }
              else
              {
                value.Append(ch);
              }
            }
            else
            {
              value.Append(ch);
            }

            break;

          #endregion
        }
      }

      return values;
    }


    /// <summary>
    /// Tries to determine the value type
    /// </summary>
    /// <param name="value">The value string</param>
    /// <returns>The converted object (if possible), else simply returns the string value passed in</returns>
    private static object TryDetermineType(string value)
    {
      if (string.IsNullOrEmpty(value))
      {
        return "";
      }

      var cleaned = value.Trim();

      // Starts with a letter- definitely a string
      if (cleaned.Length >= 13 || System.Text.RegularExpressions.Regex.IsMatch(cleaned, @"^[A-Za-z]+"))
      {
        return value.Replace('_', ' ');
      }

      // Boolean?
      if (cleaned.ToUpper() == "Y" || cleaned.ToUpper() == "N")
      {
        return cleaned.ToUpper() == "Y";
      }

      // Is integer?
      if ((cleaned == "0") || (cleaned.Length < 10 && System.Text.RegularExpressions.Regex.IsMatch(cleaned, @"^[1-9][0-9]*$")))
      {
        return int.Parse(cleaned);
      }

      // Is percent?
      if (cleaned.Length <= 6 &&
        System.Text.RegularExpressions.Regex.IsMatch(cleaned, @"(^\$(\d{1,3},?(\d{3},?)*\d{3}(\.\d{1,3})?|\d{1,3}(\.\d{2})?)$|^\d{1,2}(\.\d{1,2})? *%$|^100%$)"))
      {
        cleaned = cleaned.Trim('%');
        Decimal decVal;
        if (Decimal.TryParse(cleaned, System.Globalization.NumberStyles.Number,
          System.Globalization.NumberFormatInfo.InvariantInfo, out decVal))
        {
          return decVal / 100;
        }
      }

      // Is Decimal?
      if (cleaned.Contains('.') &&
        System.Text.RegularExpressions.Regex.IsMatch(cleaned, @"^(((\d{1,3})(,\d{3})*)|(\d+))(.\d+)?$"))
      {
        Decimal decVal;
        if (Decimal.TryParse(cleaned, System.Globalization.NumberStyles.Number,
          System.Globalization.NumberFormatInfo.InvariantInfo, out decVal))
        {
          return decVal;
        }
      }

      // Is date in format dd/mm/yy
      if (cleaned.Length == 8 && System.Text.RegularExpressions.Regex.IsMatch(cleaned, @"^\d\d/\d\d/\d\d"))
      {
        DateTime dateVal;

        // Try convert to date
        var day = int.Parse(cleaned.Substring(0, 2));
        var month = int.Parse(cleaned.Substring(3, 2));
        var year = int.Parse(cleaned.Substring(6, 2));
        year += (year < 30) ? 2000 : 1900;
        try
        {
          dateVal = new DateTime(year, month, day);
          return dateVal;
        }
        catch { }
      }

      // Is date in format dd/mm/yyyy
      if (cleaned.Length == 10 && System.Text.RegularExpressions.Regex.IsMatch(cleaned, @"^\d\d/\d\d/\d\d\d\d"))
      {
        DateTime dateVal;

        var day = int.Parse(cleaned.Substring(0, 2));
        var month = int.Parse(cleaned.Substring(3, 2));
        var year = int.Parse(cleaned.Substring(6, 4));
        try
        {
          dateVal = new DateTime(year, month, day);
          return dateVal;
        }
        catch { }
      }

      value = value.Replace('_', ' ');
      return value;
    }


    /// <summary>
    /// Cleans up a key name to be easy to read and DX Snap report compatible
    /// </summary>
    /// <param name="value">The key vlaue to clean</param>
    /// <returns>A cleaned key value, suitable for Snap reports field name</returns>
    private static string CleanKeyName(string value)
    {
      // Snap does not like non-alphanumerics
      var cleaned = System.Text.RegularExpressions.Regex.Replace(value, "[^0-9.a-zA-Z_]", "_");
      // Remove repeated underscores
      cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"[_]{2,}", "_");
      cleaned = cleaned.Trim('_');
      return cleaned;
    }


    /// <summary>
    /// Adds safely to the Dictionary- cleans the key, ensures key does not exist and detects the data type
    /// </summary>
    /// <param name="dict">The Dictionary to add the value to</param>
    /// <param name="key">The Dictionary key name</param>
    /// <param name="value">The Dictionary key value</param>
    private static void AddSafe(this Dictionary<string, object> dict, string key, object value)
    {
      var cleanedKeyName = CleanKeyName(key);
      if (!string.IsNullOrEmpty(cleanedKeyName) && !dict.ContainsKey(cleanedKeyName))
      {
        var stringValue = value as string;
        if (stringValue != null)
        {
          dict.Add(cleanedKeyName, TryDetermineType(stringValue));
        }
        else
        {
          dict.Add(cleanedKeyName, value);
        }
      }
    }


    /// <summary>
    /// State-machine parsing state
    /// </summary>
    private enum ParseStage { KeyWait, KeyName, KeyEnd, ValueWait, InValue, InValueEscaped }

    /// <summary>
    /// Value type detected
    /// </summary>
    public enum ValueTypeDetected { Unknown, IsString, IsEscapedString, IsFunction, IsBoolean, IsNumber }
  }
}