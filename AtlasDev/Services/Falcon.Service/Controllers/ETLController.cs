using Falcon.Common.Structures;
using Serilog;
using XSockets.Core.XSocket;
using XSockets.Core.XSocket.Helpers;

namespace Falcon.Service.Controllers
{
  public class ETLController : XSocketController
  {
    private static readonly ILogger _log = Log.Logger.ForContext<ETL>();
    public long? BatchId { get; set; }

    public void Get(ETL data)
    {
      if (data.SourceRequest == 1)
      {
        if (BatchId == null)
        BatchId = data.BatchId;

        this.SendTo(p => p.BatchId == data.BatchId, new { Test = "Moo" }, "Get");
      }
    }
  }
}