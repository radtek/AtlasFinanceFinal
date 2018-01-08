using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using log4net;
using DevExpress.Xpo;

using Atlas.Common.Extensions;
using Atlas.Domain.Model;
using Atlas.Enumerators;


namespace Atlas.Bureau.Server.Cache
{

  /// <summary>
  /// Caching block to maintain a persist collection of possible decline or review codes for credit vetting
  /// </summary>
  public static class ReasonCodeCache
  {
    private static Dictionary<string, FPM.DecisionOutCome> _reasonCodeCache = null;
    private static readonly ILog _log = LogManager.GetLogger(typeof(ReasonCodeCache));
    private static object synlock = new object();
		private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

    public static void BuildCache()
    {
      using (var UoW = new UnitOfWork())
      {
        var reasonCodeCollection = new XPQuery<FPM_DecisionCode>(UoW).ToList();
        _reasonCodeCache = new Dictionary<string, FPM.DecisionOutCome>();

        foreach (var reasonCode in reasonCodeCollection)
        {

          if (!_reasonCodeCache.ContainsKey(reasonCode.ReasonCode))
          {
            _log.Info(string.Format("Adding Reason Code [{0} => {1}]", reasonCode.ReasonCode, reasonCode.DecisionOutCome.ToStringEnum()));
            _reasonCodeCache.Add(reasonCode.ReasonCode, reasonCode.DecisionOutCome);
          }
        }
      }
    }

    /// <summary>
    /// Retrieves the value from the key cache dictionary
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static Dictionary<string, FPM.DecisionOutCome> GetCache()
    {
			cacheLock.EnterReadLock();
			try
			{
				_log.Info(string.Format("Size {0}", _reasonCodeCache != null ? _reasonCodeCache.Count.ToString() : "Null"));

				return _reasonCodeCache;
			}
			finally
			{
				cacheLock.ExitReadLock();
			}
    }
  }
}
