using Atlas.Domain.Model;
using Atlas.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;
using log4net;

using Atlas.Common.Extensions;


namespace Atlas.Bureau.Server.Cache
{

  /// <summary>
  /// Caching block to maintain a persist collection of policies for credit vetting.
  /// </summary>
  public static class CreditPolicyCache
  {
    private static HashSet<Risk.Policy> _creditPolicyCache = null;
    private static readonly ILog _log = LogManager.GetLogger(typeof(CreditPolicyCache));
    private static readonly object synlock = new object();

    public static void BuildCache()
    {
      using (var UoW = new UnitOfWork())
      {
        var creditPolicyCollection = new XPQuery<BUR_Policy>(UoW).Where(p => p.Enabled).ToList();
        _creditPolicyCache = new HashSet<Risk.Policy>();

        foreach (var creditPolicy in creditPolicyCollection)
        {

          if (!_creditPolicyCache.Contains(creditPolicy.Type))
          {
            _log.Info(string.Format("Adding Credit Policy [{0}]", creditPolicy.Type.ToStringEnum()));
            _creditPolicyCache.Add(creditPolicy.Type);
          }
        }
      }
    }

    /// <summary>
    /// Retrieves the value from the key cache dictionary
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static HashSet<Risk.Policy> GetCache()
    {
      lock (synlock)
      {
        return _creditPolicyCache;
      }
    }
  }
}
