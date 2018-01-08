using MassTransit;
using Ninject.Modules;
using Ninject;
using Atlas.Online.Node.Core;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Atlas.Online.Data.Models.Definitions;
using System.Configuration;
using System;
using Atlas.Online.Node.AffordabilityNode.EasyNetQ;

namespace Atlas.Online.Node.AffordabilityNode
{   
  public class AffordabilityNodeRegistry : NinjectModule
  {
    public override void Load()
    {
      Bind<AffordabilityServiceNode>().To<AffordabilityServiceNode>().InSingletonScope();
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
