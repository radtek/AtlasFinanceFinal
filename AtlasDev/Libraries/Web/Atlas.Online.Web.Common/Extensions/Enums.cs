using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Atlas.Online.Web.Common.Extensions
{
  public static class EnumStringExtension
  {
    public static string ToStringEnum(this Enum value)
    {
      Type type = value.GetType();
      FieldInfo fieldInfo = type.GetField(value.ToString());
      DescriptionAttribute[] attribs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
      return attribs.Length > 0 ? attribs[0].Description : null;
    }
    /// <summary>
    /// Returns a enumerator based on string description, otherwise returns the default of the enum
    /// </summary>
    public static T FromStringToEnum<T>(this string description)
    {
      if (String.IsNullOrEmpty(description))
      {
        return default(T);
      }

      MemberInfo[] memberInfos = typeof(T).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
      foreach (MemberInfo memberInfo in memberInfos)
      {
        DescriptionAttribute attribute = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
        if ((attribute != null && attribute.Description.Equals(description, StringComparison.OrdinalIgnoreCase)) || (memberInfo.Name == description))
        {
          return (T)Enum.Parse(typeof(T), memberInfo.Name);
        }
      }
      
      throw new InvalidOperationException(String.Format("The string '{0}' could not be mapped to Enum of type {1}", description, typeof(T).Name));
    }

    public static bool TryToEnum<T>(this string description, out T result)
    {
      object obj;
      if (TryToEnum(description, typeof(T), out obj))
      {
        result = (T)obj;
        return true;
      }

      result = default(T);
      return false;
    }

    public static bool TryToEnum(this string description, Type enumType, out object result)
    {
      try
      {
        result = Enum.Parse(enumType, description);
        return true;
      }
      catch (ArgumentException) 
      {
        result = null;
        return false; 
      }
    }

    /// <summary>
    /// Returns the descriptions attribute string for a given enum, or null
    /// </summary>
    /// <param name="enumType">Type of the enum</param>
    /// <param name="memberName">Member to reflect</param>
    /// <returns>The description string or null</returns>
    public static string GetDescription(this Enum instance)
    {
      return GetDescription(instance.GetType(), instance.ToString());
    }

    public static string GetDescription(Type type, string memberName)
    {
      var member = type.GetMember(memberName).FirstOrDefault();
      if (member == null)
      {
        throw new InvalidOperationException(String.Format("{0}is not a member of {1}", member, type.FullName));
      }

      var description = member.GetCustomAttribute(typeof(DescriptionAttribute), false);
      if (description == null)
      {
        return null;
      }

      return ((DescriptionAttribute)description).Description;
    }
  }

  public static class EnumUtil
  {
    // Returns array of Enum Values
    public static IEnumerable<T> GetValues<T>()
    {
      return Enum.GetValues(typeof(T)).Cast<T>();
    }
  }
}
