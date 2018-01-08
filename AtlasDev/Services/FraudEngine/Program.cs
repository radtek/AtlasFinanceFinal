using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Ninject;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Topshelf;

namespace Atlas.Fraud.Engine
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

      #region Start XPO- Create a thread-safe data layer
      try
      {
        // Create thread-safe- load and build the domain!
        var connStr = ConfigurationManager.ConnectionStrings["AtlasMain"].ConnectionString;
        var dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.None);
        using (var dataLayer = new SimpleDataLayer(dataStore))
        {
          using (var session = new Session(dataLayer))
          {
            XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
          }
        }
        XpoDefault.Session = null;

        Domain.DomainMapper.Map();

      }
      catch (Exception err)
      {
        throw new Exception("Error with XPO domain", err);
      }
      #endregion


      HostFactory.Run(c =>
      {
        c.SetServiceName("FraudEngine");
        c.SetDisplayName("Atlas Fraud Engine");
        c.SetDescription("Used to communicate with various fraud bureaus in order to do fraud scoring.");

        c.RunAsLocalSystem();
        var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
        var module = new FraudEngineRegistry();
        kernel.Load(module);
				Kernel.SetKernel(kernel);

        c.Service<Engine>(s =>
        {
          s.ConstructUsing(builder => kernel.Get<Engine>());
          s.WhenStarted(o => o.Start());
          s.WhenStopped(o => o.Stop());
        });
      });
    }
  }
}