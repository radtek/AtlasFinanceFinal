using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.PayoutEngine.Business;
using DevExpress.Xpo;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atlas.Payout.Engine
{
  public class Service
  {
    public string ServiceName;

    private static ILogger _logger = null;
    private PYT_ServiceDTO _service;
    private Timer _exportTimer; // Used for Exporting Queued Payouts
    private Timer _monitorTimer; // Monitor how long Payouts are taking and to send off warnings if taking longer than usual
    private int _thresholdPeriod; // Max Time a payout should take to come back with a response
    private bool _canExport; // Used for locking the Export if for whatever reason it takes too long
    private bool _canMonitor; // Used for locking the Monitor if for whatever reason it takes too long
    private List<PYT_ServiceScheduleBankDTO> _scheduledBanks;
    private List<DateTime> _publicHolidays;

    public Service(PYT_ServiceDTO service, ILogger ilogger)
    {
      _service = service;
      _logger = ilogger;

      ServiceName = service.ReferenceName;
    }

    /// <summary>
    /// Starts Up the service
    /// </summary>
    public void Start()
    {
      var exportPeriod = 1;
      var monitorPeriod = 5;

      using (var uow = new UnitOfWork())
      {
        var configExportPeriod = new XPQuery<Config>(uow).FirstOrDefault(c => c.DataType == (int)Atlas.Enumerators.Config.Payout.RTCExportPeriod);
        if (configExportPeriod != null)
          exportPeriod = int.Parse(configExportPeriod.DataValue);

        var configMonitorPeriod = new XPQuery<Config>(uow).FirstOrDefault(c => c.DataType == (int)Atlas.Enumerators.Config.Payout.RTCMonitorPeriod);
        if (configMonitorPeriod != null)
          monitorPeriod = int.Parse(configMonitorPeriod.DataValue);

        var scheduledBanks = new XPQuery<PYT_ServiceScheduleBank>(uow).Where(s => s.ServiceSchedule.Service.ServiceId == _service.ServiceId).ToList();
        _scheduledBanks = AutoMapper.Mapper.Map<List<PYT_ServiceScheduleBank>, List<PYT_ServiceScheduleBankDTO>>(scheduledBanks);

        // TODO
        // Get public holidays from db and assign to private var
        _publicHolidays = new List<DateTime>();
      }

      _monitorTimer = new Timer(MonitorExecute, null, 0, monitorPeriod * 60 * 1000); // convert seconds to milliseconds
      _exportTimer = new Timer(ExportExecute, null, 0, exportPeriod * 30 * 1000); // convert seconds to milliseconds

      _canExport = true;
    }

    private void MonitorExecute(object state)
    {
      if (_canMonitor)
      {
        throw new NotImplementedException();
      }
    }

    private void ExportExecute(object state)
    {
      if (_canExport)
      {
        try
        {
          _canExport = false;

          _logger.Info("Importing New Payouts");

          var utility = new Utility(_service.ServiceId, _publicHolidays);

          utility.ImportAndValidateNewPayouts(_scheduledBanks);

          _logger.Info("Completed Importing Payouts");

          var openBanks = GetOpenBanks();
          if (openBanks.Count > 0)
          {
            _logger.Info(string.Format("There are {0} banks open", openBanks.Count));

            _logger.Info("Batching and Exporting Payouts");

            utility.PayNewPayouts(openBanks);

            _logger.Info("Completed Batching and Exporting Payouts");
          }

          utility = null;
        }
        catch (Exception exception)
        {
          _logger.Error(string.Format("Service {0}: ExportExecute - Encountered an error, Message: {1}, Inner Exception: {2}, Stack Trace: {3}", ServiceName, exception.Message, exception.InnerException, exception.StackTrace));
        }
        finally
        {
          _canExport = true;
        }
      }
      else
      {
        _logger.Warn(string.Format("Service {0}: ExportExecute - Still busy on previous Thread", ServiceName));
      }
    }

    private List<long> GetOpenBanks()
    {
      _logger.Info(string.Format("Service {0}: Export - Checking if Banks are open..", ServiceName));

      List<long> banks = new List<long>();
      foreach (var scheduleBank in _scheduledBanks)
      {
        if (scheduleBank.ServiceSchedule.CloseTime.HasValue && scheduleBank.ServiceSchedule.OpenTime.HasValue)
        {
          if (scheduleBank.ServiceSchedule.OpenTime.Value.TimeOfDay <= DateTime.Now.TimeOfDay
            && scheduleBank.ServiceSchedule.CloseTime.Value.TimeOfDay > DateTime.Now.AddMinutes(1).TimeOfDay)
          {
            banks.Add(scheduleBank.Bank.BankId);
          }
        }
      }

      return banks;
    }
  }
}