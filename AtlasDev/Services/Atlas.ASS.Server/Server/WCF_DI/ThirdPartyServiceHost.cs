using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using Atlas.Servers.Common.WCF;


namespace Atlas.Server.WCF_DI
{
  public interface IThirdPartyServiceHost : IBaseServiceHost
  {
  }


  public class ThirdPartyServiceHost : BaseServiceHost, IThirdPartyServiceHost
  {
    public ThirdPartyServiceHost(Container container)
    {     
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(ThirdPartyServiceHost));
    }
  }

}
