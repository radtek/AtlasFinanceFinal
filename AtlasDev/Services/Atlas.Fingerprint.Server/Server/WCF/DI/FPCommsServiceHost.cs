using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using Atlas.Servers.Common.WCF;


namespace Atlas.WCF.FPServer.WCF.DI
{
  public interface IFPCommsServiceHost : IBaseServiceHost
  {

  }


  public class FPCommsServiceHost : BaseServiceHost, IFPCommsServiceHost
  {
    public FPCommsServiceHost(Container container)
    {
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(Atlas.WCF.FPServer.WCF.Implementation.FPComms));
    }
  }

}
