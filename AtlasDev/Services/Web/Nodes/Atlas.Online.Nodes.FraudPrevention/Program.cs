using Ninject;
using System;
using System.IO;
using System.Reflection;
using Ninject.Extensions.Logging.Log4net.Infrastructure;
using Topshelf;

namespace Atlas.Online.Node.FraudPrevention
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
        c.SetServiceName("FraudPreventionServiceNode");
        c.SetDisplayName("FraudPreventionService Node");
        c.SetDescription("A nodule that performs required fraud preventative checks.");

        c.RunAsLocalSystem();
        var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
        var module = new FraudPreventionNodeRegistry();
        kernel.Load(module);

        c.Service<FraudPreventionServiceNode>(s =>
        {
          s.ConstructUsing(builder => kernel.Get<FraudPreventionServiceNode>());
          s.WhenStarted(o => o.Start());
          s.WhenStopped(o => o.Stop());
        });
      });
    }
  }
}