using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using Atlas.WCF.Implementation;
using Atlas.Servers.Common.WCF;


namespace Atlas.Server.WCF_DI
{
  public interface IAssAvsServiceHost : IBaseServiceHost
  {
  }


  public class AssAvsServiceHost : BaseServiceHost, IAssAvsServiceHost
  {
    public AssAvsServiceHost(Container container)
    {    
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(VerificationServer));
    }
  }

}
