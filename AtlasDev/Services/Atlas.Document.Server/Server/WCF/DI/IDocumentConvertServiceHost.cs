using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using Atlas.Servers.Common.WCF;


namespace Atlas.DocServer.WCF.DI
{
  public interface IDocumentConvertServiceHost : IBaseServiceHost
  {
  }

  public class DocumentConvertServiceHost : BaseServiceHost, IDocumentConvertServiceHost
  {
    public DocumentConvertServiceHost(Container container)
    {
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(Implementation.Convert.DocumentConvertServer));
    }
  }
}
