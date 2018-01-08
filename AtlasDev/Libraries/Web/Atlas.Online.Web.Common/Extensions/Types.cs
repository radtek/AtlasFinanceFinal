using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Web.Common.Extensions
{
  public static class Types
  {
    public static bool IsNumeric(this Type instance)
    {
      switch (Type.GetTypeCode(instance))
      {
        case TypeCode.Byte:
        case TypeCode.SByte:
        case TypeCode.UInt16:
        case TypeCode.UInt32:
        case TypeCode.UInt64:
        case TypeCode.Int16:
        case TypeCode.Int32:
        case TypeCode.Int64:
        case TypeCode.Decimal:
        case TypeCode.Double:
        case TypeCode.Single:
          return true;
        default:
          return false;
      }
    }
  }
}
