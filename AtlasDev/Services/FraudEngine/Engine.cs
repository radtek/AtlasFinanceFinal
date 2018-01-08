namespace Atlas.Fraud.Engine
{
  using Atlas.Online.Node.Core;
  using Ninject;
  using Ninject.Extensions.Logging;
  using System;
  using Atlas.ThirdParty.Fraud;

  public sealed class Engine : IService
  {
    private static ILogger _logger = null;
    private static IKernel _kernal = null;

    public Engine(ILogger ilogger, IKernel ikernal)
    {
      _logger = ilogger;
      _kernal = ikernal;
    }

    public void Start()
    {
      _logger.Info("[FraudEngine] - Starting Engine");

      try
      {
        _logger.Info("[FraudEngine] - Engine Started");
        _logger.Info("Fraud Policies Caching..");

        ReasonCodeCache.BuildCache();

        _logger.Info("Fraud Policies Cached");
      }
      catch (Exception exception)
      {
        _logger.Error(string.Format("[FraudEngine] - Engine out of fuel /r/n Message: {0} Inner Exception: {1} Stack: {2}",
          exception.Message + Environment.NewLine, exception.InnerException + Environment.NewLine, exception.StackTrace + Environment.NewLine));
      }
    }

    public void Stop()
    {
      _logger.Info("[FraudEngine] - Shutting Down.");
    }
  }
}
