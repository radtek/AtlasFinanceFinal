namespace Atlas.Servers.Common.Config
{
  // http://www.dotnetperls.com/bool-parse
  /// <summary>
  /// Parse strings into true or false bools using relaxed parsing rules
  /// </summary>
  internal static class BoolParser
  {
    /// <summary>
    /// Get the boolean value for this string
    /// </summary>
    public static bool GetValue(string value)
    {
      return IsTrue(value);
    }

    /// <summary>
    /// Determine whether the string is not True
    /// </summary>
    public static bool IsFalse(string value)
    {
      return !IsTrue(value);
    }

    /// <summary>
    /// Determine whether the string is equal to True
    /// </summary>
    public static bool IsTrue(string value)
    {
      try
      {
        // 1
        // Avoid exceptions
        if (value == null)
        {
          return false;
        }

        value = value.Trim().ToLower();

        if (value == "true" || value == "t" || value == "1" || value == "yes" || value == "y")
        {
          return true;
        }

        return false;
      }
      catch
      {
        return false;
      }
    }
  }
}
