using System.Configuration;
using Atlas.Notification.Server.EasyNetQ;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Ninject;
using Ninject.Modules;

namespace Atlas.Notification.Server
{
  public class NotificationServerRegistry : NinjectModule
  {
    public override void Load()
    {
      Bind<NotificationServer>().To<NotificationServer>().InSingletonScope();

      // Create thread-safe- load and build the domain!
      var connStr = ConfigurationManager.ConnectionStrings["Atlas"].ConnectionString;
      var dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.None);
      using (var dataLayer = new SimpleDataLayer(dataStore))
      {
        using (var session = new Session(dataLayer))
        {
          XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
        }
      }
      XpoDefault.Session = null;

      var atlasOnlineServiceBus = new AtlasOnlineServiceBus();
      atlasOnlineServiceBus.Start();
      Bind<AtlasOnlineServiceBus>().ToConstant(atlasOnlineServiceBus);
    }

    protected virtual INinjectSettings CreateSettings()
    {
      return new NinjectSettings() { LoadExtensions = false };       
    }
  }
}
