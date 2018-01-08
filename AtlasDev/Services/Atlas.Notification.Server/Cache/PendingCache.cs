using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using log4net;

namespace Atlas.Notification.Server.Cache
{
  public static class PendingCache
  {
    #region Private Members

    private readonly static Dictionary<long, Enumerators.Notification.NotificationPriority> _pendingCache = new Dictionary<long, Enumerators.Notification.NotificationPriority>();

    private static readonly ILog _log = LogManager.GetLogger(typeof(PendingCache));

    private static object synclock = new object();

    #endregion

    public static void Push(long pk, Enumerators.Notification.NotificationPriority priority)
    {
      lock (synclock)
      {
        if (_pendingCache.ContainsKey(pk))
        {
          _log.Fatal(string.Format("Item [{0}] already exists in the cache!", pk));
          return;
        }
        _pendingCache.Add(pk, priority);
      }
    }

    public static void Pop(long pk)
    {
      lock (synclock)
      {
        if (!_pendingCache.ContainsKey(pk))
        {
          _log.Warn(string.Format("Item [{0}] does not exist in the cache", pk));
          return;
        }
        _pendingCache.Remove(pk);
      }
    }

    public static IEnumerable<long> GetItems(Enumerators.Notification.NotificationPriority priority)
    {
      lock (synclock)
      {
        return _pendingCache.Where(x => x.Value == priority).Select(p => p.Key);
      }
    }

    public static int Count()
    {
      lock (synclock)
      {
        return _pendingCache.Count();
      }
    }

    public static Dictionary<long, Enumerators.Notification.NotificationPriority> GetAll()
    {
      lock (synclock)
      {
        return _pendingCache;
      }
    }

    public static void Clear()
    {
      lock(synclock)
      {
        _pendingCache.Clear();
      }
    }

    public static void Write()
    {
      lock (synclock)
      {

        using (var fs = File.OpenWrite(string.Format("{0}/{1}", AppDomain.CurrentDomain.BaseDirectory, "cache.bin")))
        using (var deflate = new DeflateStream(fs, CompressionMode.Compress))
        using (var writer = new BinaryWriter(deflate))
        {
          writer.Write(Count());
          foreach (var pair in _pendingCache)
          {
            writer.Write(pair.Key);
            writer.Write((int)pair.Value);
          }
        }
      }
    }

    public static bool Load()
    {
      if (File.Exists(string.Format("{0}/{1}", AppDomain.CurrentDomain.BaseDirectory, "cache.bin")))
      {
        using (var fs = File.OpenRead(string.Format("{0}/{1}", AppDomain.CurrentDomain.BaseDirectory, "cache.bin")))
        using (var ds = new DeflateStream(fs, CompressionMode.Decompress))
        using (var reader = new BinaryReader(ds))
        {
          var count = reader.ReadInt32();
          while(count-->0)
          {
            _pendingCache.Add(reader.ReadInt64(), ((Enumerators.Notification.NotificationPriority)reader.ReadInt32()));
          }
        }
      }
      return true;
    }
  }
}