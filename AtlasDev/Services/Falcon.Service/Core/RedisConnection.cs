using System;
using System.Configuration;
using System.Linq;
using Newtonsoft.Json;
using StackExchange.Redis;

//
// NOTES: https://github.com/StackExchange/StackExchange.Redis/blob/master/Docs/Basics.md:
// -----------------------------------------------------------------------------------------
//   Because the ConnectionMultiplexer does a lot, it is designed to be shared and reused between callers. 
//   You should not create a ConnectionMultiplexer per operation. 
//   It is fully thread-safe and ready for this usage.
//
//   The object returned from GetDatabase() is a cheap pass-thru object, and does not need to be stored.
//

namespace Falcon.Service.Core
{
  public sealed class RedisConnection
  {
    public static ConnectionMultiplexer Current
    {
      get { return Connection; }
    }


    private static IDatabase GetDatabase(int? databaseNo)
    {
      return (databaseNo == null) ? Connection.GetDatabase() : Connection.GetDatabase((int)databaseNo);
    }

    public static IDatabase GetDatabase(int databaseNo)
    {
      return GetDatabase(databaseNo);
    }

    public static IDatabase GetDatabase()
    {
      return GetDatabase(null);
    }


    public static IServer GetServer()
    {
      return
        Current.GetEndPoints().Select(endpoint => Current.GetServer(endpoint)).FirstOrDefault(server => !server.IsSlave);
    }


    public static T GetObjectFromString<T>(string key)
    {
      var conn = Current.GetDatabase();
      var value = conn.StringGet(key);
      return (string.IsNullOrEmpty(value)) ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }


    public static void SetStringFromObject<T>(string key, T obj, TimeSpan? expiration = null)
    {
      var conn = Current.GetDatabase();
      conn.StringSet(key, JsonConvert.SerializeObject(obj));
      conn.KeyExpire(key, expiration);
    }


    #region Private fields

    private static readonly Lazy<ConnectionMultiplexer> _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
    {
      var configurationOptions = new ConfigurationOptions
      {
        EndPoints = {ConfigurationManager.AppSettings["redis.host"]},
        AllowAdmin = true,
        AbortOnConnectFail = false
      };

      return ConnectionMultiplexer.Connect(configurationOptions);
    });

    private static ConnectionMultiplexer Connection
    {
      get { return _lazyConnection.Value; }
    }

    #endregion

  }
}