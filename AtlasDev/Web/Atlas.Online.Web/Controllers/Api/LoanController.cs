using Atlas.Online.Web.Common.Serializers;
using Atlas.Online.Web.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using System.Web.UI;
using Atlas.Online.Web.Common.Extensions;
using Atlas.Online.Web.Security;
using System.Threading;
using Atlas.Online.Web.Models.Dto;

namespace Atlas.Online.Web.Controllers.Api
{
  [ValidateHttpAntiForgeryToken]
  public class LoanController : AppApiController
  { 
    // GET api/loan
    [HttpGet]
    public HttpResponseMessage Get()
    {
      bool canApply = true;
      if (CurrentClient != null)
      {
        var application = CurrentClient.CurrentApplication;
        canApply = (application == null || !application.IsFinal) && Services.WebServiceClient.APP_ApplyIn(CurrentClient.ClientId) <= 0;
      }

      var rules = Services.WebServiceClient.APP_SliderRules(CurrentClient != null ? (long?)CurrentClient.ClientId : null);

      return Request.CreateResponse(HttpStatusCode.OK, new
      {
        CanApply = canApply,
        Rules = rules
      });
    }

    // POST api/loan
    [HttpPost]
    public HttpResponseMessage Post([FromBody]JObject obj) 
    {
      LoanDto result = new LoanDto()
      {
        Amount = obj.Value<decimal>("Amount"),
        Period = obj.Value<int>("Period"),
        RepaymentAmount = obj.Value<decimal>("RepaymentAmount")
      };

      var response = Request.CreateResponse(HttpStatusCode.NoContent);

      // Store slider result in a user cookie
      var cookie = new HttpCookie(LoanDto.CookieKey, result.ToJson());
      cookie.Expires = DateTime.Now.AddHours(24);
      HttpContext.Current.Response.Cookies.Add(cookie);

      if (User.Identity.IsAuthenticated)
      {
        if (!CurrentClient.OTPVerified)
        {
          response.Headers.Add(REDIRECT_HEADER, new Uri(Url.Link("Default", new { controller = "Account", action = "OTP" })).ToString());
        }
        else
        {
          var application = CurrentClient.CurrentApplication;
          bool canApply = (application == null || !application.IsFinal) && Services.WebServiceClient.APP_ApplyIn(CurrentClient.ClientId) <= 0;

          if (!canApply)
          {
            response.Headers.Add(REDIRECT_HEADER, new Uri(Url.Link("Default", new
            {
              controller = "Application",
              action = "Index"
            })).ToString());
          }
          else
          {
            response.Headers.Add(REDIRECT_HEADER, new Uri(Url.Link("Default", new { controller = "Application", action = "Index" })).ToString());
          }
        }
      }
      else
      {
        // User need to register or log in
        response.Headers.Add(REDIRECT_HEADER, new Uri(Url.Link("Default", new { controller = "Account", action = "Register" })).ToString());
      }

      return response;
    }

  }
}
