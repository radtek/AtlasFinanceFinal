/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2016 Atlas Finance (Pty) Ltd.
*
*  Description:
*  ------------------
*    Main entry- Atlas EasyNetQ Notification server
*
*
*  Author:
*  ------------------
*     Keith Blows
*
*
*  Revision history:
*  ------------------
*     2016-03-24  Created
*
*
* ----------------------------------------------------------------------------------------------------------------- */

using SimpleInjector;
using Topshelf;
using Topshelf.SimpleInjector;

using Atlas.Common.Interface;
using Atlas.Servers.Common.Config;
using Atlas.Servers.Common.Logging;
using Atlas.Servers.Common.Xpo;
using NotificationServerENQ.Db;
using NotificationServerENQ.Senders;
using NotificationServerENQ.MessageHandler;


namespace NotificationServerENQ
{
  class Program
  {
    static void Main()
    {     
      //// DI
      RegisterDependencies();
      //      
      // XPO
      XpoUtils.CreateXpoDomain(_config, _log, new []{ typeof(Atlas.Domain.Model.NTF_NotificationNew) });

      #region Topshelf service hosting
      HostFactory.Run(hc =>
      {
        // DI config
        hc.UseSerilog();
        hc.UseSimpleInjector(_container);     

        hc.RunAsLocalSystem();
        hc.StartAutomaticallyDelayed();

        hc.SetServiceName("Atlas_Notification_ENQ");
        hc.SetDisplayName("Atlas Notification Server (EasyNetQ)");
        hc.SetDescription("Distributed delivery implementation of sending SMS and e-mail messages");

        hc.Service<MainService>(sc =>
        {
          sc.ConstructUsingSimpleInjector();

          sc.WhenStarted((service, control) => service.Start());
          sc.WhenStopped((service, control) => service.Stop());       
        });
      });
      #endregion
    }


    private static void RegisterDependencies()
    {     
      // Infrastructure 
      // ---------------------------------------
      _container.RegisterSingleton(_log);     
      _container.RegisterSingleton(_config);
           
      // Message bus- we want singleton to avoid Bus create/tear-down...means all others also need to be singleton
      _container.Register<IMessageBusHandler, EasyNetQMessageBus>(Lifestyle.Singleton);

      // Message currently being sent
      _container.Register<IMessageInProgressTracker, MessageInProgressTracker>(Lifestyle.Singleton);

      // Implementation of SMS sender
      _container.Register<ISmsSender, SmsPortalSender>(Lifestyle.Singleton);

      // Implementation of e-mail sender
      _container.Register<IEmailSender, SimpleSmtpSender>(Lifestyle.Singleton);  
      
      // Database repository    
      _container.Register<IDbRepository, DbRepository>(Lifestyle.Singleton);
    }


    //// *Cross-cutting concerns*  we need instances upfront, so create here and register as singletons
    private static readonly ILogging _log = new SerilogLogging("Atlas.Notification.Server", true, typeof(Program));
    private static readonly IConfigSettings _config = new ConfigFileSettings();

    //// DI
    private static readonly Container _container = new Container();

  }
}
