using Atlas.Online.Web.Service.EasyNetQ;
using MassTransit;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.RabbitMQ.Messages.Online;

namespace Atlas.Online.Web.Service
{
  public class OnlineRegistry : NinjectModule
  {
    public override void Load()
    {
      Bind<Engine>().To<Engine>().InSingletonScope();
      var avsServiceBus = new AVSServiceBus();
      avsServiceBus.Start();
      Bind<AVSServiceBus>().ToConstant(avsServiceBus);
      var atlasOnlineServiceBus = new AtlasOnlineServiceBus();
      atlasOnlineServiceBus.Start();
      Bind<AtlasOnlineServiceBus>().ToConstant(atlasOnlineServiceBus);

      Bind<TaskSpooler>().ToSelf();
    }

    protected virtual INinjectSettings CreateSettings()
    {
      var settings = new NinjectSettings();
      settings.LoadExtensions = false;
      return settings;
    }
  }
}
