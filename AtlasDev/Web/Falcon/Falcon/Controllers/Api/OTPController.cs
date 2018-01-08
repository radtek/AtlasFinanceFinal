using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Falcon.Controllers.Api
{
  public sealed class OTPController : AppApiController
  {
    public OTPController()
    {
    }


    [HttpPost]
    public HttpResponseMessage Verify([FromBody]dynamic value)
    {
      try
      {
        if (string.IsNullOrEmpty(value["hash"].ToString()))
          return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "OTP reset token may not be null.");

        var valid = Services.WebServiceClient.OTP_VerifyToHash(value["hash"].ToString(), int.Parse(value["OTP"].ToString()));

        if (valid == null)
          return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "OTP has expired.");

        if (valid.m_Item1)
          return Request.CreateResponse(HttpStatusCode.OK, new { isValid = true, Msg = "OTP is valid.", Hash = valid.m_Item2 });
        else
          return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "OTP is invalid." );
      }
      catch (Exception)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "An error has occurred while attempting to validate your OTP.");
      }
    }
  }
}