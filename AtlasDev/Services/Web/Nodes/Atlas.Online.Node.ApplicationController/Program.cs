using MassTransit.Log4NetIntegration.Logging;
using Ninject;
using System;
using System.IO;
using System.Reflection;
using Topshelf;

namespace Atlas.Online.Node.ApplicationController
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

      Log4NetLogger.Use(checkFile);

      HostFactory.Run(c =>
      {
        c.SetServiceName("ApplicationControllerNode");
        c.SetDisplayName("ApplicationController Node");
        c.SetDescription("A nodule that takes care of updating the web database from core");

        c.RunAsLocalSystem();
        var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
        var module = new ApplicationControllerNodeRegistry();
        kernel.Load(module);

        c.Service<ApplicationControllerNode>(s =>
        {
          s.ConstructUsing(builder => kernel.Get<ApplicationControllerNode>());
          s.WhenStarted(o => o.Start());
          s.WhenStopped(o => o.Stop());
        });
      });
    }
  }
}