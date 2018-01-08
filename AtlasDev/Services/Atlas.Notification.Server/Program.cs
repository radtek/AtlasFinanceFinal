using System;
using System.IO;
using System.Reflection;
using Ninject;
using Topshelf;

namespace Atlas.Notification.Server
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

      HostFactory.Run(c =>
      {
        c.SetServiceName("NotificationServer");
        c.SetDisplayName("Notification Server");
        c.SetDescription("A server used to shoveling notification messages back to the web ui.");

        c.RunAsLocalSystem();
        var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
        var module = new NotificationServerRegistry();
        kernel.Load(module);

        c.Service<NotificationServer>(s =>
        {
          s.ConstructUsing(builder => kernel.Get<NotificationServer>());
          s.WhenStarted(o => o.Start());
          s.WhenStopped(o => o.Stop());
        });
      });
    }
  }
}