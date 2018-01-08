/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty) Ltd.
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
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Security.Cryptography;


namespace Atlas.Common.Utils
{  
  // Case-insensitive multiple occurrence Replace
  public static class StringUtils
  {
    /// <summary>
    ///  Case-insensitive Replace
    /// </summary>
    /// <param name="str"></param>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string ReplaceString(string str, string oldValue, string newValue, StringComparison comparison)
    {
      var sb = new StringBuilder();

      var previousIndex = 0;
      var index = str.IndexOf(oldValue, comparison);
      while (index != -1)
      {
        sb.Append(str.Substring(previousIndex, index - previousIndex));
        sb.Append(newValue);
        index += oldValue.Length;

        previousIndex = index;
        index = str.IndexOf(oldValue, index, comparison);
      }
      sb.Append(str.Substring(previousIndex));

      return sb.ToString();
    }


    /// <summary>
    /// Cleans cellular number for use with SMS sending-- appends country code 27 if required, removes leading 0
    /// </summary>
    /// <param name="cellNo"></param>
    /// <returns></returns>
    public static string CleanCellForSMS(string cellNo)
    {
      if (cellNo == null)
        return null;

      var cellNum = cellNo.Trim();

      // Remove all non numeric
      cellNum = Regex.Replace(cellNum, @"[^\d]", "");

      // Strip off leading 0
      if (cellNum.StartsWith("0"))
        cellNum = cellNum.Substring(1, cellNum.Length - 1);

      // Add ZA country code, if none
      if (cellNum.Length == 9)
        cellNum = "27" + cellNum;

      return cellNum;
    }


    /// <summary>
    /// Remove all non Numeric letters from the text supplied
    /// </summary>
    /// <param name="text">text to be cleaned up</param>
    /// <returns>numeric only of the text</returns>
    public static string RemoveNonNumeric(string text)
    {
      if (string.IsNullOrEmpty(text))
        return string.Empty;

      return Regex.Replace(text, @"[^\d]", "").Trim();
    }


    /// <summary>
    /// Converts a number to nicer display, i.e. 1024 = 1KB
    /// http://sharpertutorials.com/pretty-format-bytes-kb-mb-gb/
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public static string FormatBytesShort(Int64 bytes, int scale = 1024)
    {
      var orders = new string[] { "GB", "MB", "KB", "Bytes" };
      var max = (long)Math.Pow(scale, orders.Length - 1);

      foreach (var order in orders)
      {
        if (bytes > max)
        {
          return string.Format("{0:##.##}{1}", Decimal.Divide(bytes, max), order);
        }

        max /= scale;
      }

      return "0 Bytes";
    }


    /// <summary>
    /// This will safely split an e-mail address list like: 
    /// Blows, Keith <keithb@eta.co.za>; John Smith <johns@ertc.com>
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public static List<string> SplitEMailAddressList(string address)
    {
      if (string.IsNullOrEmpty(address))
      {
        return null;
      }

      var result = new List<string>();

      address = address.Replace(';', ',');

      var atIdx = 0;
      var commaIdx = 0;
      var lastComma = 0;
      var index = 0;
      foreach (var ch in address)
      {
        if (ch == '@')
          atIdx = index;

        if (ch == ',')
          commaIdx = index;

        if (commaIdx > atIdx && atIdx > 0)
        {
          var temp = address.Substring(lastComma, commaIdx - lastComma);
          result.Add(temp.Trim(", ".ToCharArray()));
          lastComma = commaIdx;
          atIdx = int.MaxValue;
        }

        if (index == address.Length - 1)
        {
          var temp = address.Substring(lastComma, address.Length - lastComma);
          result.Add(temp.Trim(", ".ToCharArray()));
        }

        index++;
      }

      if (commaIdx < 2)
      {
        // if we get here we can assume either there was no comma, or there was only one comma as part of the last, first combo
        result.Add(address);
      }

      return result;
    }


    /// <summary>
    /// Generates a random base32 string
    /// </summary>
    /// <returns></returns>
    public static string RandomBase32()
    {
      var b = Guid.NewGuid().ToByteArray().ToList();
      b.AddRange(System.BitConverter.GetBytes(DateTime.Now.Ticks).ToList());
      return Atlas.Common.OTP.Base32Encoding.ToString(b.ToArray());
    }


    /// <summary>
    /// Generate a cryptographically-secure random string
    /// </summary>
    /// <param name="length">Length</param>
    /// <param name="allowedChars"></param>
    /// <returns></returns>
    public static string RandomString(int length, string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
    {
      if (length < 0) throw new ArgumentOutOfRangeException("length", "length cannot be less than zero.");
      if (string.IsNullOrEmpty(allowedChars)) throw new ArgumentException("allowedChars may not be empty.");

      const int byteSize = 0x100;
      var allowedCharSet = allowedChars.ToArray();
      if (byteSize < allowedCharSet.Length) throw new ArgumentException(String.Format("allowedChars may contain no more than {0} characters.", byteSize));

      // Guid.NewGuid and System.Random are not particularly random. By using a
      // cryptographically-secure random number generator, the caller is always
      // protected, regardless of use.
      using (var rng = new RNGCryptoServiceProvider())
      {
        var result = new StringBuilder();
        var buf = new byte[128];
        while (result.Length < length)
        {
          rng.GetBytes(buf);
          for (var i = 0; i < buf.Length && result.Length < length; ++i)
          {
            // Divide the byte into allowedCharSet-sized groups. If the
            // random value falls into the last group and the last group is
            // too small to choose from the entire allowedCharSet, ignore
            // the value in order to avoid biasing the result.
            var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
            if (outOfRangeStart <= buf[i]) continue;
            result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
          }
        }
        return result.ToString();
      }
    }


    /// <summary>
    /// Determine if string is base-64 string
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsBase64String(string value)
    {
      if (string.IsNullOrEmpty(value))
      {
        return false;
      }
      try
      {
        Convert.FromBase64String(value);
        return true;
      }
      catch (FormatException)
      {
        return false;
      }
    }


    /// <summary>
    /// Determine if number is numeric
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNumeric(string value)
    {
      var i = 0L;
      return long.TryParse(value, out i);
    }


    /// <summary>
    /// Split string into two parts
    /// </summary>
    /// <param name="length"></param>
    /// <param name="no"></param>
    /// <returns></returns>
    public static Tuple<string, string> SplitNo(int length, string no)
    {
      if (no.Length < length)
        throw new IndexOutOfRangeException("Split length is longer than No");

      var a = no.Substring(0, length);
      var b = no.Substring(length, no.Length - length);

      return new Tuple<string, string>(a, b);
    }


    /// <summary>
    ///  Returns a nice English expression for a TimeSpan
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToEnglish(TimeSpan value)
    {
      string duration;

      if (value.TotalMinutes < 1)
        duration = string.Format("{0} second{1}", value.Seconds, value.Seconds == 1 ? string.Empty : "s");

      else if (value.TotalHours < 1)
        duration = string.Format("{0} minute{1}, {2} second{3}",
          value.Minutes, value.Minutes == 1 ? string.Empty : "s",
          value.Seconds, value.Seconds == 1 ? string.Empty : "s");

      else if (value.TotalDays < 1)
        duration = string.Format("{0} hour{1}, {2} minute{3}",
          value.Hours, value.Hours == 1 ? string.Empty : "s",
          value.Minutes, value.Minutes == 1 ? string.Empty : "s");

      else
        duration = string.Format("{0} day{1}, {2} hour{3}",
          value.Days, value.Days == 1 ? string.Empty : "s",
          value.Hours, value.Hours == 1 ? string.Empty : "s");

      return duration;
    }


    public static string AddOrdinal(int value)
    {
      if (value <= 0) return value.ToString();

      switch (value % 100)
      {
        case 11:
        case 12:
        case 13:
          return value + "th";
      }

      switch (value % 10)
      {
        case 1:
          return value + "st";
        case 2:
          return value + "nd";
        case 3:
          return value + "rd";
        default:
          return value + "th";
      }
    }
  }
}
