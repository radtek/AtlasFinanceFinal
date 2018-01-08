using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using AtlasServer.WCF.Implementation;
using Atlas.Servers.Common.WCF;


namespace Atlas.Server.WCF_DI
{
  public interface IASSServiceHost : IBaseServiceHost
  {
  }


  public class ASSServiceHost : BaseServiceHost, IASSServiceHost
  {
    public ASSServiceHost(Container container)
    {    
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(ASSServer));
    }
  }

}
