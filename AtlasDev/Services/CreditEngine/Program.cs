using System.IO;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Ninject;
using System;
using System.Configuration;
using Atlas.Domain;
using Topshelf;
using Serilog;

namespace Atlas.Credit.Engine
{
  class Program
  {
    [STAThread]
    static void Main()
    {
      try
      {
        Log.Logger = new LoggerConfiguration()
      .WriteTo.ColoredConsole()
      .WriteTo.RollingFile(string.Format("{0}/{1}", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"), "CreditEngine.txt"))
      .CreateLogger();

        #region Start XPO- Create a thread-safe data layer

        // Create thread-safe- load and build the domain!
        var connStr = ConfigurationManager.ConnectionStrings["AtlasMain"].ConnectionString;
        var dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.None);
        using (var dataLayer = new SimpleDataLayer(dataStore))
        {
          using (var session = new Session(dataLayer))
          {
            XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
          }
        }
        XpoDefault.Session = null;

        AutoMapper.Mapper.CreateMap<Atlas.ThirdParty.CompuScan.Enquiry.Reason, Atlas.RabbitMQ.Messages.Credit.Reason>();
        AutoMapper.Mapper.CreateMap<Atlas.ThirdParty.CompuScan.Enquiry.Product, Atlas.RabbitMQ.Messages.Credit.Product>();
        AutoMapper.Mapper.CreateMap<Atlas.ThirdParty.CompuScan.Enquiry.Account, Atlas.RabbitMQ.Messages.Credit.NLRCPAAccount>();
        AutoMapper.Mapper.CreateMap<Atlas.RabbitMQ.Messages.Credit.CreditResponse, Atlas.RabbitMQ.Messages.Credit.CreditStreamResponse>();
        DomainMapper.Map();
        
        #endregion

        HostFactory.Run(c =>
        {
          c.SetServiceName("CreditEngine");
          c.SetDisplayName("Atlas Credit Engine");
          c.SetDescription("Used to communicate with various credit bureaus in order to do credit scoring.");
          c.UseSerilog();
          c.RunAsLocalSystem();
          var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
          var module = new CreditEngineRegistry();
          kernel.Load(module);

          c.Service<Engine>(s =>
          {
            s.ConstructUsing(builder => kernel.Get<Engine>());
            s.WhenStarted(o => o.Start());
            s.WhenStopped(o => o.Stop());
          });
        });
      }
      catch (Exception err)
      {
        throw new Exception("Error ", err);
      }
    }
  }
}