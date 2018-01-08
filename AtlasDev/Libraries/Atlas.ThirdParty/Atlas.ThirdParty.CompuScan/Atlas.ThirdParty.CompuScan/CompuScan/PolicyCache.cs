using System;
using System.Collections.Generic;
using System.Linq;

using Serilog;
using DevExpress.Xpo;

using Atlas.Enumerators;
using Atlas.Domain.Model;
using Atlas.Common.Extensions;


namespace Atlas.ThirdParty.CompuScan
{
  /// <summary>
  /// Caching block to maintain a persist collection of policies for credit vetting.
  /// </summary>
  public static class PolicyCache
  {
    private static HashSet<Risk.Policy> _creditPolicyCache = null;
    private static readonly ILogger _log = Log.Logger;
    private static object synlock = new object();


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
            _log.Information("Adding Credit Policy [{Policy}]", creditPolicy.Type.ToStringEnum());
            lock (synlock)
            {            
              _creditPolicyCache.Add(creditPolicy.Type);
            }           
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


    public static bool GetCachItem(Risk.Policy policy)
    {
      lock (synlock)
      {
        return _creditPolicyCache.Contains(policy);
      }
    }
  }
}