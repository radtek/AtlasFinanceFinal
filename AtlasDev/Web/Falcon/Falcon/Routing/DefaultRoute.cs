using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Falcon.Routing
{
  public class DefaultRoute : Route
  {
    public DefaultRoute()
      : base("{*path}", new DefaultRouteHandler())
    {
      this.RouteExistingFiles = false;
    }
  }
}