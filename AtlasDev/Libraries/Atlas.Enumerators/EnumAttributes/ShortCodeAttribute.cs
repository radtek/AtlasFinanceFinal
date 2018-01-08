using System;


namespace Atlas.Enumerators.EnumAttributes
{
  /// <summary>
  /// Helps with lookups for 'short codes' used by ASS/Evolution
  /// </summary>
  [System.AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
  public class ShortCodeAttribute : Attribute
  {
    public string ShortCode { get; set; }


    public ShortCodeAttribute(string shortCode)
    {
      ShortCode = shortCode;
    }


    public static string GetShortCode(Type tp, string name)
    {
      var mi = tp.GetMember(name);
      if (mi != null && mi.Length > 0)
      {
        var attr = Attribute.GetCustomAttribute(mi[0], typeof(ShortCodeAttribute)) as ShortCodeAttribute;
        if (attr != null)
        {
          return attr.ShortCode;
        }
      }
      return null;
    }


    public static string GetShortCode(object enm)
    {
      if (enm != null)
      {
        var mi = enm.GetType().GetMember(enm.ToString());
        if (mi != null && mi.Length > 0)
        {
          var attr = GetCustomAttribute(mi[0], typeof(ShortCodeAttribute)) as ShortCodeAttribute;
          if (attr != null)
          {
            return attr.ShortCode;
          }
        }
      }
      return null;
    }
    

    public static T GetEnumFromShortCode<T>(string shortCode) where T : struct, IConvertible
    {
      if (!typeof(T).IsEnum)
      {
        throw new ArgumentException("T must be an enumerated type");
      }

      var enumValues = Enum.GetValues(typeof(T));
      foreach(var enumVal in enumValues)
      {
        if (GetShortCode(enumVal) == shortCode)
        {
          return (T)enumVal;
        }
      }

      return default(T);
    }
  }
}
