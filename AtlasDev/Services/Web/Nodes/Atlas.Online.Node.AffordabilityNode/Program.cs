using Ninject;
using System;
using System.IO;
using System.Reflection;
using Topshelf;

namespace Atlas.Online.Node.AffordabilityNode
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
        c.SetServiceName("AffordabilityServiceNode");
        c.SetDisplayName("AffordabilityService Node");
        c.SetDescription("A nodule enabling the calculation of affordability for the client.");

        c.RunAsLocalSystem();
        var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
        var module = new AffordabilityNodeRegistry();
        kernel.Load(module);

        c.Service<AffordabilityServiceNode>(s =>
        {
          s.ConstructUsing(builder => kernel.Get<AffordabilityServiceNode>());
          s.WhenStarted(o => o.Start());
          s.WhenStopped(o => o.Stop());
        });
      });
    }
  }
}