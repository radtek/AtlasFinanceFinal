using log4net;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.ThirdParty.Service
{
  public sealed class RedisConnection
  {
    private const string _redisConnectionFailed = "Redis connection failed.";
    private ConnectionMultiplexer _connection;
    private static volatile RedisConnection _instance;

    private static object syncLock = new object();
    private static object syncConnectionLock = new object();

    public static RedisConnection Current
    {
      get
      {
        if (_instance == null)
        {
          lock (syncLock)
          {
            if (_instance == null)
            {
              _instance = new RedisConnection();
            }
          }
        }

        return _instance;
      }
    }

    private RedisConnection()
    {
      _connection = newConnection();
    }

    private static ConnectionMultiplexer newConnection()
    {
      return ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["redis.host"]);
    }

    private IDatabase GetDatabase(int? databaseNo)
    {
      lock (syncConnectionLock)
      {
        try
        {
          if (_connection == null)
            _connection = newConnection();
        }
        catch (SocketException ex)
        {
          throw new Exception(_redisConnectionFailed, ex);
        }
      }

      if (databaseNo == null)
        return _connection.GetDatabase();
      else
        return _connection.GetDatabase((int)databaseNo);
    }
    public IDatabase GetDatabase(int databaseNo)
    {
      return this.GetDatabase(databaseNo);
    }
    public IDatabase GetDatabase()
    {
      return this.GetDatabase(null);
    }

    public static T GetObjectFromString<T>(string key)
    {
      var conn = RedisConnection.Current.GetDatabase();
      var bankString = _instance.GetDatabase().StringGet(key);
      if (string.IsNullOrEmpty(bankString))
        return default(T);
      else
        return JsonConvert.DeserializeObject<T>(bankString);
    }

    public static void SetStringFromObject<T>(string key, T obj, TimeSpan? expiration = null)
    {
      var conn = RedisConnection.Current.GetDatabase();
      conn.StringSet(key, JsonConvert.SerializeObject(obj));
      conn.KeyExpire(key, expiration);
    }
  }
}