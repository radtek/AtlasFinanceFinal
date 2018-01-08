using System;
using Autofac;
using Falcon.Common.Interfaces.Jobs;
using Falcon.DuckHawk.Jobs.Attributes;
using Quartz;
using Serilog;
using Stream.Framework.Services;

namespace Falcon.DuckHawk.Jobs.QuartzTasks.ASS
{
  //[DisableJob]
  [DisallowConcurrentExecution]
  [JobName("AssStreamAccountImport")]
  [TriggerName("AssStreamAccountImport")]
  [CronExpression("0 10 8,20 1/1 * ? *")] // run every day @ 20:00 AND 07:00
  //[CronExpression("0 0/1 * 1/1 * ? *")] //run every minute for testing purposes
  public class AssStreamAccountImport : IAssStreamAccountImportJob
  {
    private readonly ILogger _logger;
    private readonly IStreamService _streamService;
    private readonly ILifetimeScope _scope;
    public AssStreamAccountImport(ILifetimeScope scope, ILogger logger, IStreamService streamService)
    {
      _scope = scope;
      _logger = logger;
      _streamService = streamService;
    }

    public void Execute(IJobExecutionContext context)
    {
      using (_scope.BeginLifetimeScope())
      {
        try
        {
          _logger.Information("Started Job [AssStreamAccountImport]");

          try
          {
            _logger.Information("Working Job [AssStreamAccountImport] - Importing New Accounts - Collections");
            _streamService.ImportNewCollectionAccounts();
          }
          catch (Exception ex)
          {
            _logger.Error(
              string.Format("Error in Job [AssStreamAccountImport] - Importing New Accounts - Collections: {0} - {1}",
                ex.Message, ex.StackTrace));
          }

          try
          {
            _logger.Information("Working Job [AssStreamAccountImport] - Importing New Accounts - Sales");
            _streamService.ImportNewSaleAccounts();
          }
          catch (Exception ex)
          {
            _logger.Error(
              string.Format("Error in Job [AssStreamAccountImport] - Importing New Accounts - Sales: {0} - {1}",
                ex.Message, ex.StackTrace));
          }

          try
          {
            _logger.Information("Working Job [AssStreamAccountImport] - Removing Deceased Clients");
            _streamService.RemoveAssDeceaseClients();
          }
          catch (Exception ex)
          {
            _logger.Error(
              string.Format("Error in Job [AssStreamAccountImport] - Removing Deceased Clients: {0} - {1}",
                ex.Message, ex.StackTrace));
          }

          try
          {
            _logger.Information("Working Job [AssStreamAccountImport] - Checking PTP Payments");
            _streamService.CheckAllCollectionPtps();
          }
          catch (Exception ex)
          {
            _logger.Error(string.Format("Error in Job [AssStreamAccountImport] - Checking PTP Payments: {0} - {1}",
              ex.Message, ex.StackTrace));
          }

          try
          {
            _logger.Information("Working Job [AssStreamAccountImport] - Checking Handovers and Paid-ups");
            _streamService.ClosePaidupAccounts();
          }
          catch (Exception ex)
          {
            _logger.Error(
              string.Format("Error in Job [AssStreamAccountImport] - Checking Handovers and Paid-ups: {0} - {1}",
                ex.Message, ex.StackTrace));
          }

          try
          {
            _logger.Information("Working Job [AssStreamAccountImport] - Checking Handover client for Sales");
            _streamService.RemoveAssArrearClientsFromSalesStream();
          }
          catch (Exception ex)
          {
            _logger.Error(
              string.Format(
                "Error in Job [AssStreamAccountImport] - Checking Handover client for Sales: {0} - {1}", ex.Message,
                ex.StackTrace));
          }

          try
          {
            _logger.Information("Working Job [AssStreamAccountImport] - Checking Client  for Newer loans for Sales");
            _streamService.RemoveAssClientsWithNewLoansFromSalesStream();
          }
          catch (Exception ex)
          {
            _logger.Error(
              string.Format(
                "Error in Job [AssStreamAccountImport] - Checking Client  for Newer loans for Sales: {0} - {1}",
                ex.Message, ex.StackTrace));
          }

          _logger.Information("Finished Job [AssStreamAccountImport]");
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("Error in Job [AssStreamAccountImport]: {0} - {1}", ex.Message, ex.StackTrace));
        }
      }
    }
  }
}