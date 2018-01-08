using Ninject;
using System;
using System.IO;
using System.Reflection;
using Topshelf;

namespace Atlas.Online.Node.AccountVerificationNode
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
            c.SetServiceName("AccountVerificationNode");
            c.SetDisplayName("AccountVerification Node");
            c.SetDescription("A nodule enabling the processing of request messages to do account verification against clients.");

            c.RunAsLocalSystem();
            var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
            var module = new VerificationNodeRegistry();
            kernel.Load(module);

            c.Service<AccountVerificationServiceNode>(s =>
                {
                  s.ConstructUsing(builder => kernel.Get<AccountVerificationServiceNode>());
                  s.WhenStarted(o => o.Start());
                  s.WhenStopped(o => o.Stop());
                });
          });
    }
  }
}