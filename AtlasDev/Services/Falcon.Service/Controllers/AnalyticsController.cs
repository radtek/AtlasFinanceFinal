using System;
using Falcon.Service.Core;
using Falcon.Common.Structures;
using Serilog;
using XSockets.Core.XSocket;
using XSockets.Core.XSocket.Helpers;

namespace Falcon.Service.Controllers
{ 
  public class AnalyticsController : XSocketController
  {
    private static readonly ILogger _log = Log.Logger.ForContext<Analytics>();
    public string Branch { get; set; }
    private string Init()
    {
      var conn = RedisConnection.Current.GetDatabase();

      string result = conn.StringGet(string.Format("avs.analytics.branch.{0}", Branch));

      if (!string.IsNullOrEmpty(result))
      {
        _log.Information("[FalconService][AvsAnalyticsController] - Finished getting statsics for {branch} with result {data}", string.Format("avs.analytics.branch.{0}", Branch), Convert.ToString(result));
        return Convert.ToString(result);
      }
      else
      {
        _log.Information("[FalconService][AvsAnalyticsController] - Finished getting statsics for {branch} with result {data}", string.Format("avs.analytics.branch.{0}", Branch), "0");
        return Convert.ToString("0");
      }
    }

    public void Stats(Analytics data)
    {
      if (data.SourceRequest == 1)
      {
        if (string.IsNullOrEmpty(Branch))
          Branch = data.Branch;

        this.SendTo(p => p.Branch == data.Branch, new { AvsCount = Init(), Branch = data.Branch }, "stats");
      }
      else
      {
        this.SendTo(p => p.Branch == data.Branch, new { AvsCount = data.AvsCount, Branch = data.Branch }, "stats");
      }
    }

    public void SendForceRefresh()
    {
      this.SendToAll("do_force_refresh", "update_avs");
    }
  }
}