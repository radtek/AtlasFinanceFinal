using Atlas.Servers.Common.WCF;
using SimpleInjector;
using SimpleInjector.Integration.Wcf;


namespace Atlas.WCF.FPServer.WCF.DI
{
  public interface IFPCoreServiceHost : IBaseServiceHost
  {
  }


  public class FPCoreServiceHost : BaseServiceHost, IFPCoreServiceHost
  {
    public FPCoreServiceHost(Container container)
    {
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(Atlas.WCF.FPServer.WCF.Implementation.FPServer));
    }
  }

}
