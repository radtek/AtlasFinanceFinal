using Ninject;
using System;
using System.IO;
using System.Reflection;
using Topshelf;

namespace Atlas.Online.Node.CreditNode
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
        c.SetServiceName("CreditServiceNode");
        c.SetDisplayName("CreditService Node");
        c.SetDescription("A nodule that takes care of performing credit enquiries on the application.");

        c.RunAsLocalSystem();
        var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
        var module = new CreditNodeRegistry();
        kernel.Load(module);

        c.Service<CreditServiceNode>(s =>
        {
          s.ConstructUsing(builder => kernel.Get<CreditServiceNode>());
          s.WhenStarted(o => o.Start());
          s.WhenStopped(o => o.Stop());
        });
      });
    }
  }
}