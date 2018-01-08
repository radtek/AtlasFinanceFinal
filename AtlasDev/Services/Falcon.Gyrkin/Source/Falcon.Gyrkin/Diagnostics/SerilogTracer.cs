using System;
using System.Net.Http;
using System.Web.Http.Tracing;
using Serilog;

namespace Falcon.Gyrkin.Diagnostics
{
  public class SeriLogTracer : ITraceWriter
  {
    private static readonly ILogger _log = Log.Logger.ForContext<SeriLogTracer>();
    public void Trace(HttpRequestMessage request, string category, TraceLevel level,
        Action<TraceRecord> traceAction)
    {
      TraceRecord rec = new TraceRecord(request, category, level);
      traceAction(rec);
      WriteTrace(rec);
    }

    protected void WriteTrace(TraceRecord rec)
    {
      
      var message = string.Format("{0};{1};{2}",
          rec.Operator, rec.Operation, rec.Message);
      _log.Information("Message: {message}, Category: {category}", message, rec.Category);
    }
  }
}