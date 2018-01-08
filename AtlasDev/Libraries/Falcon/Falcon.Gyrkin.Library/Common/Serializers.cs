﻿using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Falcon.Gyrkin.Library.Common
{
  // Using JSON.net
  public static class JsonNet
  {
    public static string Serialize<T>(T obj, TypeNameHandling typeNameHandling = TypeNameHandling.None)
    {
      return JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings()
      {
        TypeNameHandling = typeNameHandling,
        TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
      });
    }

    public static T Deserialize<T>(string json, TypeNameHandling typeNameHandling = TypeNameHandling.None)
    {
      return (T)JsonConvert.DeserializeObject(json, new JsonSerializerSettings
      {
        TypeNameHandling = typeNameHandling,
        TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
      });
    }
  }
}