using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;
using Atlas.Online.Web.Common.Helpers;

namespace Atlas.Online.Web.Common.Mappers
{
  public static class JMapper
  {
    public static T Map<T>(JToken data)
    {
      Type destType = typeof(T);
      return (T)Map(destType, data);
    }

    public static object Map(Type destType, JToken data) 
    {
      object obj = Activator.CreateInstance(destType);

      if (data.Count() == 0)
      {
        return obj;
      }

      PropertyInfo[] props = destType.GetProperties();

      foreach (var prop in props)
      {
        if (!prop.CanWrite) { continue; }

        object value;
        object objVal = data.Value<Object>(prop.Name);
        if (objVal is JContainer)
        {
          prop.SetValue(obj, Map(prop.PropertyType, (JToken)objVal));
          continue;
        }

        JValue jValue = (JValue)objVal;

        if (jValue == null) { continue; }

        // Support nullables 
        Type propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        if (propType.IsEnum)
        {
          value = Enum.ToObject(propType, Convert.ToInt32(jValue.Value));
        }
        else
        {
          value = jValue.Value;

          if (propType.IsNumeric())
          {
            if (value is String && String.IsNullOrEmpty(value.ToString()))
            {
              value = 0;
            }
            else
            {
              value = ConvertExt.ToType(propType, value);
            }
          }
        }

        try
        {
          prop.SetValue(obj, value);
        }
        catch (Exception)
        {
          // Do nothing - property will be defaulted
        }
      }

      return obj;
    }
  }
}
