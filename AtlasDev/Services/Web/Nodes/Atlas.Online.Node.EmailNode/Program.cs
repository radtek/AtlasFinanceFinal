using MassTransit.Log4NetIntegration.Logging;
using Ninject;
using System;
using System.IO;
using System.Reflection;
using Topshelf;

namespace Atlas.Online.Node.EmailNode
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
        c.SetServiceName("EmailServiceNode");
        c.SetDisplayName("EmailService Node");
        c.SetDescription("A nodule that takes care of sending emails.");

        c.RunAsLocalSystem();
        var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
        var module = new EmailNodeRegistry();
        kernel.Load(module);

        c.Service<EmailServiceNode>(s =>
        {
          s.ConstructUsing(builder => kernel.Get<EmailServiceNode>());
          s.WhenStarted(o => o.Start());
          s.WhenStopped(o => o.Stop());
        });
      });
    }
  }
}