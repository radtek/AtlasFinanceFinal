using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using Atlas.Servers.Common.WCF;


namespace ASSServer.WCF.DI
{
  public interface IAdminServiceHost : IBaseServiceHost
  {

  }


  public class AdminServiceHost : BaseServiceHost, IAdminServiceHost
  {
    public AdminServiceHost(Container container)
    {
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(ASSServer.WCF.Implementation.Admin.AdminServer));
    }

  }
}
