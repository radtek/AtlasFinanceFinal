using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using Atlas.Servers.Common.WCF;


namespace ASSServer.WCF.DI
{
  public interface IDataFileServiceHost : IBaseServiceHost
  {
  }


  public class DataFileServiceHost : BaseServiceHost, IDataFileServiceHost
  {
    public DataFileServiceHost(Container container)
    {
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(ASSServer.WCF.Implementation.DataFileChunk.DataFileServer));
    }
  }

}
