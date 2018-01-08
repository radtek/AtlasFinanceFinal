using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Common.Extensions;
using Atlas.Online.Web.Security;
using Atlas.Online.Web.WebService;
using DevExpress.Xpo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace Atlas.Online.Web.Controllers.Api
{
  [Authorize]
  [ValidateHttpAntiForgeryToken]
  public class OtpController : AppApiController
  {
    /// <summary>
    /// Send an OTP to the currently logged in users mobile.
    /// </summary>
    /// <param name="first">If true, uses the SendFirstOTP service call which will only send an OTP if the client has not sent one already.</param>
    /// <returns>Response, with redirect instructions or OTP Send data</returns>
    [HttpGet]
    public HttpResponseMessage RequestOtp(bool first = false)
    {
      Uri redirectTo;
      HttpResponseMessage response;

      if (!ValidateOtpRequest(out redirectTo))
      {
        return RedirectTo(redirectTo);
      }
      else
      {
        response = Request.CreateResponse(HttpStatusCode.OK);
      }

      OtpSendResult result = Services.WebServiceClient.OTP_Send(CurrentClient.ClientId, first);

      response.Content = new ObjectContent<Object>(result, DefaultFormatter);

      return response;
    }

    [HttpPost]
    public HttpResponseMessage Validate(JObject data)
    {
      Uri redirectTo;

      if (!ValidateOtpRequest(out redirectTo))
      {
        return RedirectTo(redirectTo);
      }

      // Sanitize otp

      var otp = Regex.Replace(data.Value<string>("otp"), @"[^.0-9]", String.Empty).Length > 10 ?
        Convert.ToInt32(Regex.Replace(data.Value<string>("otp"), @"[^.0-9]", String.Empty).Substring(0, 9)) :
         Convert.ToInt32(Regex.Replace(data.Value<string>("otp"), @"[^.0-9]", String.Empty));

      return Request.CreateResponse(HttpStatusCode.OK, new
      {
        Validated = Services.WebServiceClient.OTP_Verify(CurrentClient.ClientId, otp)
      });
    }

    private bool ValidateOtpRequest(out Uri redirectTo)
    {
      // Check that the given cell number belongs to the current client
      if (CurrentClient == null)
      {
        redirectTo = new Uri(Url.Link("Default", new { controller = "Account", action = "Login" }));
        return false;
      }

      if (CurrentClient.OTPVerified)
      {
        redirectTo = new Uri(Url.Link("Default", new { controller = "Application", action = "Index" }));
        return false;
      }

       redirectTo = null;
       return true;
    }

  }
}
