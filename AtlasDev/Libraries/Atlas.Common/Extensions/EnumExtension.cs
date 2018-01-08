using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Atlas.Common.Extensions
{
  public static class EnumExtension
  {
    public static string ToStringEnum(this Enum value)
    {
      if (value == null)
        return string.Empty;

      var type = value.GetType();
      var fieldInfo = type.GetField(value.ToString());
      var attribs = fieldInfo.GetCustomAttributes(typeof (DescriptionAttribute), false) as DescriptionAttribute[];
      return attribs != null && attribs.Length > 0 ? attribs[0].Description : null;
    }

    public static int ToInt(this Enum value)
    {
      if (value == null)
        throw new InvalidCastException("Cannot cast null enum to int");

      return Convert.ToInt32(value);
    }

    /// <summary>
    /// Returns a enumerator based on string description
    /// </summary>
    public static T FromStringToEnum<T>(this string description)
    {
      var memberInfos = typeof (T).GetMembers();

      if (memberInfos.Length > 0)
      {
        foreach (var memberInfo in memberInfos)
        {
          var attributes = memberInfo.GetCustomAttributes(typeof (DescriptionAttribute), false);
          if (attributes.Length > 0)
          {
            if (((DescriptionAttribute) attributes[0]).Description == description)
            {
              return (T) Enum.Parse(typeof (T), memberInfo.Name);
            }
          }
        }
      }
      return default(T);
    }
  }

  public static class EnumUtil
  {
    // Returns array of Enum Values
    public static IEnumerable<T> GetValues<T>()
    {
      return Enum.GetValues(typeof (T)).Cast<T>();
    }
  }
}