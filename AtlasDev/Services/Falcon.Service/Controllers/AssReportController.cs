using System;
using Falcon.Service.Core;
using Serilog;
using XSockets.Core.XSocket;

namespace Falcon.Service.Controllers
{
  public class AssReportController : XSocketController
  {
    private static readonly ILogger _log = Log.Logger.ForContext<AssReportController>();
    public string Branch { get; set; }
    private string Init()
    {
      var conn = RedisConnection.Current.GetDatabase();

      string result = conn.StringGet(string.Format("ass.report.branch.{0}", Branch));

      if (!string.IsNullOrEmpty(result))
      {
        _log.Information(string.Format("[FalconService][AssReportController] - Finished getting statsics for {branch} with result {data}", string.Format("ass.report.branch.{0}", Branch), Convert.ToString(result)));
        return Convert.ToString(result);
      }
      else
      {
        _log.Information(string.Format("[FalconService][AssReportController] - Finished getting statsics for {branch} with result {data}", string.Format("ass.report.branch.{0}", Branch), "0"));
        return Convert.ToString("0");
      }
    }
  }
}
