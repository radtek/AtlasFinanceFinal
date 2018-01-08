using Ninject;
using System;
using System.IO;
using Topshelf;
using Serilog;

namespace Atlas.Online.Transaction.Processor
{
  class Program
  {
    [STAThread]
    static void Main()
    {
      Log.Logger = new LoggerConfiguration()
         .WriteTo.ColoredConsole()
         .WriteTo.RollingFile(string.Format("{0}/{1}", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"), "Processor.txt"))
         .CreateLogger();

      HostFactory.Run(c =>
     {
       c.UseAssemblyInfoForServiceInfo();
       
       var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
       var module = new TransactionProcessorRegistry();
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