using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Serilog;

namespace Falcon.Gyrkin.Controllers.Api
{
  public sealed class ExceptionController : ApiController
  {

    private readonly ILogger _logger;
    public ExceptionController(ILogger logger)
    {
      _logger = logger;
    }

    public HttpResponseMessage Fatal()
    {
      try
      {
        throw new Exception("Fatal exception");
      }
      catch (Exception ex)
      {
        _logger.Fatal(ex, "Avs - Transactions");
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, string.Format("Stack Trace: {0} , Inner: {1}", ex.StackTrace,ex.InnerException));
      }
    }
  }
}
