using Ninject.Modules;
using Ninject;
using System.Configuration;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Atlas.Online.Data.Models.Definitions;
using System;
using Atlas.Online.Node.FraudPrevention.EasyNetQ;

namespace Atlas.Online.Node.FraudPrevention
{
   public class FraudPreventionNodeRegistry : NinjectModule
  {
    public override void Load()
    {
      Bind<FraudPreventionServiceNode>().To<FraudPreventionServiceNode>().InSingletonScope();

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
