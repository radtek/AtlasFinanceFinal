using Ninject;
using System;
using System.IO;
using System.Reflection;
using Topshelf;

namespace Atlas.Online.Node.AccountCreation
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
        c.SetServiceName("AccountCreationServiceNode");
        c.SetDisplayName("AccountCreationService Node");
        c.SetDescription("A nodule that takes care of creating the initation of the account(loan).");

        c.RunAsLocalSystem();
        var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
        var module = new AccountCreationNodeRegistry();
        kernel.Load(module);

        c.Service<AccountCreationServiceNode>(s =>
        {
          s.ConstructUsing(builder => kernel.Get<AccountCreationServiceNode>());
          s.WhenStarted(o => o.Start());
          s.WhenStopped(o => o.Stop());
        });
      });
    }
  }
}