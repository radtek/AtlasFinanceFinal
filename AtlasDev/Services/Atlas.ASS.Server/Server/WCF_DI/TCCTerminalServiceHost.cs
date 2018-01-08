using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using AtlasServer.WCF.Implementation;
using Atlas.Servers.Common.WCF;


namespace Atlas.Server.WCF_DI
{
  public interface ITCCTerminalServiceHost : IBaseServiceHost
  {
  }


  public class TCCTerminalServiceHost : BaseServiceHost, ITCCTerminalServiceHost
  {   
    public TCCTerminalServiceHost(Container container)
    {     
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(NPTerminalRC));
    }     
  }

}
