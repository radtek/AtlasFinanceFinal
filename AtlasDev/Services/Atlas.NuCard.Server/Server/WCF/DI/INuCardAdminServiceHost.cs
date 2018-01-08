using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using Atlas.Servers.Common.WCF;
using AtlasServer.WCF.Implementation;


namespace Atlas.Server.NuCard.WCF.DI
{
  public interface INuCardAdminServiceHost: IBaseServiceHost
  {
  }


  public class NuCardAdminServiceHost: BaseServiceHost, INuCardAdminServiceHost
  {
    public NuCardAdminServiceHost(Container container)
    {
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(NuCardAdminServer));
    }
  }
}