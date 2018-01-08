using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Web.Common.Serializers
{
  // Using DataContractJsonSerializer
  public static class JsonContract
  {
    public static string Serialize<T>(T obj)
    {
      string value = string.Empty;
      DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
      using (var ms = new MemoryStream())
      {
        serializer.WriteObject(ms, obj);
        value = Encoding.Default.GetString(ms.ToArray());
        ms.Dispose();
      }
      return value;
    }

    public static T Deserialize<T>(string json)
    {
      T obj = Activator.CreateInstance<T>();
      using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
      {
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
        obj = (T)serializer.ReadObject(ms);
        ms.Close();
        ms.Dispose();
      }
      return obj;
    }
  }
}
