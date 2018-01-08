using Atlas.Online.Data.Models.Definitions;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Ninject;
using Ninject.Extensions.Logging.Log4net;
using Ninject.Extensions.Logging.Log4net.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Atlas.Online.Web.Service
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
        var connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        var dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.None);
        using (var dataLayer = new SimpleDataLayer(dataStore))
        {
          using (var session = new Session(dataLayer))
          {
            XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
          }
        }
        XpoDefault.Session = null;
      }
      catch (Exception err)
      {
        throw new Exception("Error with XPO domain", err);
      }


      #endregion

      HostFactory.Run(c =>
      {
        c.SetServiceName("OnlineService");
        c.SetDisplayName("Atlas Online Web Service");
        c.SetDescription("Used to ferry calls to various backend components");

        c.RunAsLocalSystem();
        var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
        var module = new OnlineRegistry();
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