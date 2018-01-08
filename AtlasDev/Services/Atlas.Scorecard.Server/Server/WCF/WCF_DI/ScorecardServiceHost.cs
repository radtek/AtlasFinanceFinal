using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using Atlas.Servers.Common.WCF;
using Atlas.ThirdParty.CS.WCF.Implementation;


namespace Atlas.ThirdParty.CS.Bureau.WCF.WCF_DI
{

  public interface IScorecardServiceHost : IBaseServiceHost
  {
  }


  public class ScorecardServiceHost : BaseServiceHost, IScorecardServiceHost
  {
    public ScorecardServiceHost(Container container)
    {
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(ScorecardServer));
    }
  }

}
