using System;
using System.Collections.Concurrent;
using Serilog;

namespace Falcon.Gyrkin.ControllerDiscovery
{
  public static class ControllerCacher
  {
    private readonly static ConcurrentDictionary<string, Type> _controllerCache = new ConcurrentDictionary<string, Type>();
    private readonly static ILogger _log = Log.Logger.ForContext(typeof(ControllerCacher));

    public static Type Get(string controller)
    {
      try
      {
        Type type;
        _controllerCache.TryGetValue(controller, out type);
        return type;
      }
      catch (ArgumentException e)
      {
        _log.Fatal(e.Message);
      }
      return null;
    }

    public static void Set(string controller, Type type)
    {
      try
      {
        _controllerCache.TryAdd(controller, type);
      }
      catch (ArgumentException e)
      {
        _log.Fatal(e.Message);
      }
    }
  }
}
