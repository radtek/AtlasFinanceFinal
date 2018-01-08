using System.Collections.Generic;
using Autofac;
using MassTransit;

namespace Falcon.Gyrkin.ESB
{
  public static class EsbBootstrapConfigurator
  {
    public static void Start(IContainer container)
    {
      foreach (var service in container.Resolve<IEnumerable<IBusService>>())
      {
        service.Start(container.Resolve<IServiceBus>());
      }
    }
  }
}
