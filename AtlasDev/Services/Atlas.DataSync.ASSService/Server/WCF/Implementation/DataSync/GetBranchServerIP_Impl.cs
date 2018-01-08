using System;
using System.Linq;

using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Cache.DataUtils;


namespace ASSServer.WCF.Implementation.DataSync
{
  public static class GetBranchServerIP_Impl
  {
    public static string Execute(ILogging log, ICacheServer cache, string branchCode)
    {
      var methodName = "GetBranchServerIP";

      try
      {
        log.Information("{MethodName} starting, {Branch}", methodName, branchCode);

        var server = CacheUtils.GetBranchServerViaBranchNum(cache, branchCode);
        if (server?.Machine != null )
        {
          var machine = cache.Get<COR_Machine_Cached>(server.Machine.Value);
          var ip = machine.MachineIPAddresses.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(s => s.StartsWith("192.168."));
          if (ip != null)
          {
            return ip;
          }
        }
        
        throw new Exception(string.Format("Branch '{0}' does not have a registered branch server, in a valid IP address range", branchCode));
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        return string.Empty;
      }
    }

  }
}
