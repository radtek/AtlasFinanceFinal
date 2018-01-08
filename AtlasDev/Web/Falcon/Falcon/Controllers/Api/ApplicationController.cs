using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace Falcon.Controllers.Api
{
  public sealed class ApplicationController : AppApiController
  {

    public ApplicationController()
    {
    }


    [HttpGet]
    public HttpResponseMessage LocateClient(string idNo)
    {
      var result = Services.WebServiceClient.Person_GetByIdentityNo(idNo);

      if (result == null)
        return Request.CreateResponse(HttpStatusCode.NotFound, "Client was not found.");


      return Request.CreateResponse(HttpStatusCode.OK, new { result });
    }
  }
}