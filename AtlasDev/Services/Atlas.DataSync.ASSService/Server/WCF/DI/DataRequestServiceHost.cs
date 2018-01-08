using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using Atlas.Servers.Common.WCF;


namespace ASSServer.WCF.DI
{
  public interface IDataRequestServiceHost : IBaseServiceHost
  {
  }


  public class DataRequestServiceHost : BaseServiceHost, IDataRequestServiceHost
  {
    public DataRequestServiceHost(Container container)
    {
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(ASSServer.WCF.Implementation.DataFileRequest.DataRequestServer));
    }
  }
}
