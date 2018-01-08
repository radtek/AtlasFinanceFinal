using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Falcon.Gyrkin.Library.Extensions
{
  public static class MvcStringExtensions
  {
    public static MvcHtmlString If(this MvcHtmlString value, bool evaluation)
    {
      return evaluation ? value : MvcHtmlString.Empty;
    }
  }
}