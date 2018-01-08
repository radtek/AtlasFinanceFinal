using MassTransit;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Orchestration.Server
{
  public class OrchestrationRegistry : NinjectModule
  {
    public override void Load()
    {
      Bind<Engine>().To<Engine>().InSingletonScope();
    }

    protected virtual INinjectSettings CreateSettings()
    {
      var settings = new NinjectSettings();
      settings.LoadExtensions = false;
      return settings;
    }
  }
}
