using Atlas.Domain.Model;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Ninject;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Topshelf;

namespace Atlas.ThirdParty.Service
{
  class Program
  {
    [STAThread]
    static void Main()
    {
      var path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

      var checkFile = Path.Combine(path, string.Format("{0}{1}", Assembly.GetExecutingAssembly().ManifestModule.Name, ".Config"));
      if (!File.Exists(checkFile))
      {
        throw new FileNotFoundException("Log configuration file was not found");
      }
      log4net.Config.XmlConfigurator.Configure();

      try
      {
        #region Start XPO- Create a thread-safe data layer

        // Create thread-safe- load and build the domain!
        var connStr = ConfigurationManager.ConnectionStrings["AtlasMain"].ConnectionString;
        var dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.None);
        using (var dataLayer = new SimpleDataLayer(dataStore))
        {
          using (var session = new Session(dataLayer))
          {
            //session.UpdateSchema();
            //session.UpdateSchema(typeof(PER_Person));
            //session.CreateObjectTypeRecords();
            XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
          }
        }
        XpoDefault.Session = null;

        Domain.DomainMapper.Map();

        #endregion

        HostFactory.Run(c =>
        {
          c.SetServiceName("ThirdPartyService");
          c.SetDisplayName("Atlas Third Party Service");
          c.SetDescription("Used to allow third parties to leverage atlas services");

          c.RunAsLocalSystem();
          var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
          var module = new ThirdPartyServiceRegistry();
          kernel.Load(module);

          c.Service<Engine>(s =>
          {
            s.ConstructUsing(builder => kernel.Get<Engine>());
            s.WhenStarted(o => o.Start());
            s.WhenStopped(o => o.Stop());
          });
        });
      }
      catch (Exception err)
      {
        throw new Exception("Error ", err);
      }
    }
  }
}