using Atlas.Domain.Model;
using Atlas.Enumerators;
using DevExpress.Xpo;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Common.Extensions;

namespace Atlas.ThirdParty.Fraud
{
  /// <summary>
  /// Caching block to maintain a persist collection of possible decline or review codes for fraud vetting
  /// </summary>
  public static class ReasonCodeCache
  {
    private static Dictionary<string, FPM.DecisionOutCome> _reasonCodeCache = null;
    private static readonly ILog _log = LogManager.GetLogger(typeof(ReasonCodeCache));
    private static object synlock = new object();

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
      lock (synlock)
      {
        return _reasonCodeCache;
      }
    }
  }
}