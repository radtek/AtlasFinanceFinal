using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Atlas.Online.Web.Common.Serializers
{
  public static class Base64
  {
    public static string Serialize(object obj)
    {
      var formatter = new ObjectStateFormatter();
      return formatter.Serialize(obj);
    }

    public static TObj Deserialize<TObj>(string encoded)
    {
      var formatter = new ObjectStateFormatter();
      return (TObj)formatter.Deserialize(encoded);
    }
  }

}
