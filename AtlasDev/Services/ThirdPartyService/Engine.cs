using Atlas.Online.Node.Core;
using Atlas.ThirdParty.Service.Implementation;
using MassTransit;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.ServiceModel;

namespace Atlas.ThirdParty.Service
{
  public sealed class Engine : IService
  {
    private static ILogger _logger = null;
    private static IKernel _kernal = null;
    private ServiceHost _webService = null;
    public Engine(ILogger ilogger, IKernel ikernal)
    {
      _logger = ilogger;
      _kernal = ikernal;
    }

    public void Start()
    {
      _logger.Info("[ThirdPartyService] - Starting Engine");

      try
      {
        _webService = new ServiceHost(typeof(ThirdPartyService));
        _webService.Open();

        _logger.Info("[ThirdPartyService] - Engine Started");

      }
      catch (Exception exception)
      {
        _logger.Error(string.Format("[ThirdPartyService] - Engine out of fuel /r/n Message: {0} Inner Exception: {1} Stack: {2}",
          exception.Message + Environment.NewLine, exception.InnerException + Environment.NewLine, exception.StackTrace + Environment.NewLine));
      }
    }

    public void Stop()
    {
      _logger.Info("[ThirdPartyService] - Shutting Down.");
    }
  }
}
