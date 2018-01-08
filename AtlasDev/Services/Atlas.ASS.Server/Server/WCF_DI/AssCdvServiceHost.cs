using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using Atlas.Servers.Common.WCF;


namespace Atlas.Server.WCF_DI
{
  public interface IAssCdvServiceHost : IBaseServiceHost
  {
  }


  public class AssCdvServiceHost : BaseServiceHost, IAssCdvServiceHost
  {
    public AssCdvServiceHost(Container container)
    {     
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(Atlas.Server.WCF.Implementation.CDV.AVS));
    }
  }

}
