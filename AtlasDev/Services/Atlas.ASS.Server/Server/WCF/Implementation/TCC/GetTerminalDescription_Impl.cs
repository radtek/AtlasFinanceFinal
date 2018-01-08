using System;

using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;


namespace Atlas.Server.WCF.Implementation.TCC
{
  internal static class GetTerminalDescription_Impl
  {
    internal static string Execute(ILogging log, IConfigSettings config, ICacheServer cache,  int terminalID)
    {      
      var methodName = "GetTerminalDescription";
      try
      {
        if (terminalID <= 0)
        {
          return string.Empty;
        }

        var terminal = cache.Get<TCCTerminal_Cached>(terminalID);
        return terminal != null ? terminal.Description : string.Empty;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        return string.Empty;
      }
    }

  }
}
