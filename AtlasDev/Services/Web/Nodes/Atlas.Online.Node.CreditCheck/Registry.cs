using MassTransit;
using Ninject.Modules;
using Ninject;
using Atlas.Online.Node.Core;
using System.Configuration;
using Atlas.Online.Node.CreditNode.CreditServer;
using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using Atlas.Online.Data.Models.Definitions;
using System;
using Atlas.Online.Node.CreditNode.EasyNetQ;

namespace Atlas.Online.Node.CreditNode
{
  public class CreditNodeRegistry : NinjectModule
  {
    public override void Load()
    {
      Bind<CreditServiceNode>().To<CreditServiceNode>().InSingletonScope();

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
