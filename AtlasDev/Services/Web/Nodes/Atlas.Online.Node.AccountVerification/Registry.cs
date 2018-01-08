using Ninject.Modules;
using Ninject;
using System.Configuration;
using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using Atlas.Online.Node.AccountVerificationNode.EasyNetQ;

namespace Atlas.Online.Node.AccountVerificationNode
{
  public class VerificationNodeRegistry : NinjectModule
  {
    public override void Load()
    {
      Bind<AccountVerificationServiceNode>().To<AccountVerificationServiceNode>().InSingletonScope();

      // Create thread-safe- load and build the domain!
      var connStr = ConfigurationManager.ConnectionStrings["AtlasOnline"].ConnectionString;
      var dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.None);
      using (var dataLayer = new SimpleDataLayer(dataStore))
      {
        using (var session = new Session(dataLayer))
        {
          XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
        }
      }
      XpoDefault.Session = null;
      var avsServiceBus = new AVSServiceBus();
      avsServiceBus.Start();
      Bind<AVSServiceBus>().ToConstant(avsServiceBus);
      var atlasOnlineServiceBus = new AtlasOnlineServiceBus();
      atlasOnlineServiceBus.Start();
      Bind<AtlasOnlineServiceBus>().ToConstant(atlasOnlineServiceBus);
    }
    protected virtual INinjectSettings CreateSettings()
    {
      var settings = new NinjectSettings();
      settings.LoadExtensions = false;
      return settings;
    }
  }
}
