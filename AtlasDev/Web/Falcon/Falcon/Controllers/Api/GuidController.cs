using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace Falcon.Controllers.Api
{
  public sealed class GuidController : AppApiController
  {
    
    [HttpGet]
    public HttpResponseMessage Get()
    {
      return Request.CreateResponse(HttpStatusCode.OK, new { guid = Guid.NewGuid() });
    }
  }
}