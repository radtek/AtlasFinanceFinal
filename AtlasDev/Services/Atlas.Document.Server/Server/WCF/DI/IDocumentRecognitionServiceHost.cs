using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using Atlas.Servers.Common.WCF;


namespace Atlas.DocServer.WCF.DI
{
  public interface IDocumentRecognitionServiceHost : IBaseServiceHost
  {
  }

  public class DocumentRecognitionServiceHost : BaseServiceHost, IDocumentRecognitionServiceHost
  {
    public DocumentRecognitionServiceHost(Container container)
    {
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(Atlas.DocServer.WCF.Implementation.Recognition.DocumentRecognition));
    }
  }
}
