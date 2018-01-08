/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty) Ltd.
 * 
 * 
 *  Description:
 *  ------------------
 *    String extension methods
 * 
 * 
 *  Author:
 *  ------------------
 *    Keith Blows 
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *  
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Text;
using System.Web.Script.Serialization;


namespace Atlas.Common.Extensions
{
  /// <summary>
  /// String extension methods
  /// </summary>
  public static class StringExtensions
  {
    /// <summary>
    /// Case insensitive version of String.Replace().
    /// </summary>
    /// <param name="s">String that contains patterns to replace</param>
    /// <param name="oldValue">Pattern to find</param>
    /// <param name="newValue">New pattern to replaces old</param>
    /// <param name="comparisonType">String comparison type</param>
    /// <returns></returns>
    public static string Replace(this string s, string oldValue, string newValue, StringComparison comparisonType)
    {
      if (s == null)
        return null;

      if (String.IsNullOrEmpty(oldValue))
        return s;

      var result = new StringBuilder(Math.Min(4096, s.Length));
      var pos = 0;

      while (true)
      {
        var i = s.IndexOf(oldValue, pos, comparisonType);
        if (i < 0)
          break;

        result.Append(s, pos, i - pos);
        result.Append(newValue);

        pos = i + oldValue.Length;
      }
      result.Append(s, pos, s.Length - pos);

      return result.ToString();
    }


    /// <summary>Returns a string array that contains the substrings in this string, that are separated a given fixed length.</summary>
    /// <param name="s">This string object.</param>
    /// <param name="length">Size of each substring.
    ///     <para>CASE: length > 0 , RESULT: String is split from left to right.</para>
    ///     <para>CASE: length == 0 , RESULT: String is returned as the only entry in the array.</para>
    ///     <para>CASE: length < 0 , RESULT: String is split from right to left.</para>
    /// </param>
    /// <returns>String array that has been split into substrings of equal length.</returns>
    /// <example>
    ///     <code>
    ///         string s = "1234567890";
    ///         string[] a = s.Split(4); // a == { "1234", "5678", "90" }
    ///     </code>
    /// </example>       
    public static string[] Split(this string s, int length)
    {
      var str = new System.Globalization.StringInfo(s);

      var lengthAbs = Math.Abs(length);

      if (str == null || str.LengthInTextElements == 0 || lengthAbs == 0 || str.LengthInTextElements <= lengthAbs)
        return new string[] { str.ToString() };

      var array = new string[(str.LengthInTextElements % lengthAbs == 0 ? str.LengthInTextElements / lengthAbs : (str.LengthInTextElements / lengthAbs) + 1)];

      if (length > 0)
        for (int iStr = 0, iArray = 0; iStr < str.LengthInTextElements && iArray < array.Length; iStr += lengthAbs, iArray++)
          array[iArray] = str.SubstringByTextElements(iStr, (str.LengthInTextElements - iStr < lengthAbs ? str.LengthInTextElements - iStr : lengthAbs));
      else // if (length < 0)
        for (int iStr = str.LengthInTextElements - 1, iArray = array.Length - 1; iStr >= 0 && iArray >= 0; iStr -= lengthAbs, iArray--)
          array[iArray] = str.SubstringByTextElements((iStr - lengthAbs < 0 ? 0 : iStr - lengthAbs + 1), (iStr - lengthAbs < 0 ? iStr + 1 : lengthAbs));

      return array;
    }


    public static string ToJSON(this object obj)
    {
      var serializer = new JavaScriptSerializer();
      return serializer.Serialize(obj);
    }


    public static string ToJSON(this object obj, int depth)
    {
      JavaScriptSerializer serializer = new JavaScriptSerializer();
      serializer.RecursionLimit = depth;
      return serializer.Serialize(obj);
    }


    public static string ConvertSecondsToMinutesString(this double seconds)
    {
      var minutes = (Math.Round(seconds / 60, 0)).ToString();
      return (string.Format("{0}:{1}", minutes.Length < 2 ? minutes.PadLeft(2, '0') : minutes, (Math.Round(seconds, 0) % 60).ToString().PadLeft(2, '0')));
    }
  }
}
