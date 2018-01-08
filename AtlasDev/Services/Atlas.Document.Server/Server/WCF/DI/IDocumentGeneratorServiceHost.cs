using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using Atlas.Servers.Common.WCF;


namespace Atlas.DocServer.WCF.DI
{
  public interface IDocumentGeneratorServiceHost : IBaseServiceHost
  {
  }

  public class DocumentGeneratorServiceHost : BaseServiceHost, IDocumentGeneratorServiceHost
  {
    public DocumentGeneratorServiceHost(Container container)
    {
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(Atlas.DocServer.WCF.Implementation.Generator.DocumentGeneratorServer));
    }
  }
}
