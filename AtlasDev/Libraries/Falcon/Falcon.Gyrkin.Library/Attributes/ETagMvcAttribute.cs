using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Falcon.Gyrkin.Library.Attributes
{
  public class ETagMvcAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      filterContext.HttpContext.Response.Filter = new ETagFilter(filterContext.HttpContext.Response, filterContext.RequestContext.HttpContext.Request);
    }
  }
}