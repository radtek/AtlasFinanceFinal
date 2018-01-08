using Atlas.Servers.Common.WCF;
using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using AtlasServer.WCF.Implementation;


namespace Atlas.Server.NuCard.WCF.DI
{
  public interface INuCardStockServiceHost : IBaseServiceHost
  {
  }
  

  public class NuCardStockServiceHost : BaseServiceHost, INuCardStockServiceHost
  {
    public NuCardStockServiceHost(Container container)
    {
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(NuCardStockServer));
    }
  }

}