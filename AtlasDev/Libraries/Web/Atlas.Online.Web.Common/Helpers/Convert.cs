using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Web.Common.Helpers
{
  public static class ConvertExt
  {
    public static IEnumerable<TypeCode> NumericTypeCodes
    {
      get
      {
        return new TypeCode[] {
          TypeCode.Byte,
          TypeCode.SByte,
          TypeCode.UInt16,
          TypeCode.UInt32,
          TypeCode.UInt64,
          TypeCode.Int16,
          TypeCode.Int32,
          TypeCode.Int64,
          TypeCode.Decimal,
          TypeCode.Double,
          TypeCode.Single,
        };
      }
    }

    public static object ToType(Type type, object value)
    {
      if (value == null) { value = 0; }

      TypeCode code = Type.GetTypeCode(type);
      string codeStr = code.ToString();

      var method = typeof(Convert).GetMethod(String.Format("To{0}", codeStr), new Type[] { value.GetType() });

      return method.Invoke(null, new object[] { value });
    }
  }
}
