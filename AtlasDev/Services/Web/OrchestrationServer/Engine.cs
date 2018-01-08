using Atlas.Online.Web.Service.Interface.Atlas.Online.Node.Core;
using MassTransit;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Orchestration.Server
{
  public sealed class Engine : IService
  {

    private static ILogger _logger = null;
    private static IKernel _kernal = null;
    private ServiceHost _orchestration;


    public Engine(ILogger ilogger, IKernel ikernal)
    {
      _logger = ilogger;
      _kernal = ikernal;
    }

    public void Start()
    {
      try
      {
        _orchestration = new ServiceHost(typeof(Atlas.Orchestration.Server.WCF.Implementation.OrchestrationService));
        _orchestration.Open();
        // 2 is default
        ServicePointManager.DefaultConnectionLimit = 100;

      }
      catch (Exception exception)
      {
        _logger.FatalException("Engine()", exception);
      }
    }

    public void Stop()
    {
      _kernal.Dispose();
    }
  }
}