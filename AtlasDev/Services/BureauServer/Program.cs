using System;
using System.IO;
using Topshelf;
using System.Reflection;
using System.Configuration;
using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using Ninject;

namespace Atlas.Bureau.Service
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
        c.SetServiceName("AtlasBureauService");
        c.SetDisplayName("Atlas Bureau Service");
        c.SetDescription("Bureau Service enables communication with third party providers.");

        c.RunAsLocalSystem();
        var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
        var module = new BureauServerRegistry();
        kernel.Load(module);

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