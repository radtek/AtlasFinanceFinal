using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using Atlas.Servers.Common.WCF;
using Atlas.DocServer.WCF.Implementation.Admin;


namespace Atlas.DocServer.WCF.DI
{
  public interface IDocumentAdminServiceHost : IBaseServiceHost
  {
  }

  public class DocumentAdminServiceHost : BaseServiceHost, IDocumentAdminServiceHost
  {
    public DocumentAdminServiceHost(Container container)
    {
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(DocumentAdminServer));
    }
  }
}
