using Atlas.Bureau.Server.Cache;
using Atlas.Bureau.Service.WCF.Implemenation;
using Atlas.Online.Node.Core;
using MassTransit;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Bureau.Service
{
  public sealed class Engine : IService
  {
    private static ILogger _logger = null;
	  private ServiceHost BatchService;
    private ServiceHost CreditService;
    private ServiceHost FraudService;
    private ServiceHost AuthenticationService;

    public Engine(ILogger ilogger, IKernel kernel)
    {
	    _logger = ilogger;
			ServiceLocator.SetServiceLocator(kernel);
    }

	  public void Start()
    {

      try
      {
        #region Start WCF
        try
        {
          BatchService = new ServiceHost(typeof(BatchServer));
          BatchService.Open();
        }
        catch (Exception err)
        {
          BatchService = null;
          throw err;
        }

        try
        {
          FraudService = new ServiceHost(typeof(FraudServer));
          FraudService.Open();
        }
        catch (Exception err)
        {
          FraudService = null;
          throw err;
        }
        
        try
        {
          CreditService = new ServiceHost(typeof(CreditServer));
          CreditService.Open();
        }
        catch (Exception err)
        {
          CreditService = null;
          throw err;
        }

        try
        {
          AuthenticationService = new ServiceHost(typeof(AuthenticationServer));
          AuthenticationService.Open();
        }
        catch (Exception err)
        {
          AuthenticationService = null;
          throw err;
        }

        #endregion
      }
      catch (Exception exception)
      {
        throw exception;
      }
    }

    public static void AddMsg(dynamic msg)
    {
      ResponseMessageCache.Add(msg);
    }

    public void Stop()
    {
      #region Close WCF services

      _logger.Info("Closing WCF services");

      if (BatchService != null)
      {
        BatchService.Close();
        BatchService = null;
        _logger.Info("Batch Service Closed");
      }

      if (FraudService != null)
      {
        FraudService.Close();
        FraudService = null;
        _logger.Info("Fraud Service Closed");
      }

      if (CreditService != null)
      {
        CreditService.Close();
        CreditService = null;
        _logger.Info("Credit Service Closed");
      }

      if (AuthenticationService != null)
      {
        AuthenticationService.Close();
        AuthenticationService = null;
        _logger.Info("Authentication Service Closed");
      }


      _logger.Info("WCF services stopped");

      #endregion
    }
  }
}