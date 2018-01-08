using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Caching;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Falcon.Gyrkin.Library.Security.Attributes
{
  public class ThrottleAttribute : ActionFilterAttribute
  {
    public string Name { get; set; }
    public int Seconds { get; set; }
    public string Message { get; set; }

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      //var key = string.Concat(Name, "-", IP.GetClient(actionContext.Request));
      //var allowExecute = false;

      //if (HttpRuntime.Cache[key] == null)
      //{
      //  HttpRuntime.Cache.Add(key,
      //      true, // is this the smallest data we can have?
      //      null, // no dependencies
      //      DateTime.Now.AddSeconds(Seconds), // absolute expiration
      //      Cache.NoSlidingExpiration,
      //      CacheItemPriority.Low,
      //      null); // no callback

      //  allowExecute = true;
      //}

      //if (!allowExecute)
      //{
      //  if (string.IsNullOrEmpty(Message))
      //  {
      //    Message = "You may only perform this action every {0} seconds.";
      //  }
      //  actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Conflict,
      //   string.Format(Message, Seconds.ToString()));
      //}
      return;
    }
  }
}