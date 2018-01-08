using SimpleInjector;
using SimpleInjector.Integration.Wcf;

using Atlas.Servers.Common.WCF;


namespace ASSServer.WCF.DI
{
  public interface IDataSyncServiceHost : IBaseServiceHost
  {
  }

  public class DataSyncServiceHost : BaseServiceHost, IDataSyncServiceHost
  {
    public DataSyncServiceHost(Container container)
    {
      _serviceHost = new SimpleInjectorServiceHost(container, typeof(ASSServer.WCF.Implementation.DataSync.DataSyncServer));
    }
  }
}
