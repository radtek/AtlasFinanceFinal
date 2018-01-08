using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Falcon.Gyrkin.ControllerDiscovery;

namespace Falcon.Gyrkin.ServiceDiscovery
{
  public class ControllerDiscoverySelector : DefaultHttpControllerSelector
  {
    private readonly HttpConfiguration _configuration;

    public ControllerDiscoverySelector(HttpConfiguration configuration)
      : base(configuration)
    {
      _configuration = configuration;
    }

    public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
    {
      var controllerName = request.RequestUri.Segments[3].Replace("/","").ToLower();
      var controller = ControllerCacher.Get(controllerName);
      return new HttpControllerDescriptor(_configuration, controllerName, controller);
    }
  }
}