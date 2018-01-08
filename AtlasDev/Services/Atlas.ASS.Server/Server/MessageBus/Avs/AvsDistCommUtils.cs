using System;
using System.Threading.Tasks;

using EasyNetQ;
using Atlas.Common.Interface;

using Atlas.WCF.Interface;
using BankVerification.EasyNetQ;


namespace Atlas.Server.MessageBus.Avs
{
  /// <summary>
  /// Distributed comms utils
  /// </summary>
  internal static class AvsDistCommUtils
  {
    /// <summary>
    /// Start messaging system
    /// </summary>
    internal static void Start(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;

      _log.Information("AVS comms: Starting Bus...");
      // Request/receive via RabbitMQ...
      var address = _config.GetCustomSetting("", "rabbitmq-avs-address", false);
      var virtualHost = _config.GetCustomSetting("", "rabbitmq-avs-vhost", false);
      var userName = _config.GetCustomSetting("", "rabbitmq-avs-username", false);
      var password = _config.GetCustomSetting("", "rabbitmq-avs-password", false);

      var connectionString = string.Format(
        "host={0};virtualHost={1};username={2};password={3};timeout=5;product=AtlasServer;requestedHeartbeat=120",
        address, virtualHost, userName, password);
      _bus = RabbitHutch.CreateBus(connectionString);
    }


    /// <summary>
    /// Stop messaging system
    /// </summary>
    internal static void Stop()
    {
      try
      {
        if (_bus != null)
        {
          _bus.Dispose();
          _bus = null;
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "Stop()");
      }
    }


    /// <summary>
    /// Add a new AVS request and return the message id
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static long AddAVS(AddAVSRequest request)
    {
      _log.Information("AddAVS: Starting {@Request}", request);

      try
      {
        var task = Task.Run<BankVerification.EasyNetQ.AVSResponse>(async () =>
          {
            return await _bus.RequestAsync<AddAVSRequest, BankVerification.EasyNetQ.AVSResponse>(request).ConfigureAwait(false);        
          });

        if (task.Wait(5500))
        {
          return task.Result?.TransactionId ?? 0;
        }
        else
        {
          _log.Error("AddAVS: Timed-out waiting for Task");
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "AddAVS()");
      }

      return -1;
    }


    /// <summary>
    /// Add a new AVS request and return the old style response for legacy systems
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static AVSReply AddAVSWithResponse(AddAVSRequest request)
    {
      _log.Information("AddAVSWithResponse starting: {@Request}", request);

      try
      {
        var task = Task.Run<BankVerification.EasyNetQ.AVSResponse>(async () =>
          {
            return await _bus.RequestAsync<AddAVSRequest, BankVerification.EasyNetQ.AVSResponse>(request).ConfigureAwait(false);           
          });

        if (task.Wait(5500))
        {
          return new AVSReply
          {
            AccountAcceptsCredits = task.Result.AccountAcceptsCredits,
            AccountAcceptsDebits = task.Result.AccountAcceptsDebits,
            AccountExists = task.Result.AccountExists,
            AccountOpen = task.Result.AccountOpen,
            AccountOpen90days = task.Result.AccountOpen90days,
            FinalResult = task.Result.FinalResult,
            IdNumberMatch = task.Result.IdNumberMatch,
            InitialsMatch = task.Result.InitialsMatch,
            LastNameMatch = task.Result.LastNameMatch,
            TransactionId = task.Result.TransactionId,
            WaitingReply = task.Result.WaitingReply,

            Bank = request.Bank,
            BankAccountNo = request.AccountNo,
            BranchCode = request.BranchCode,
            IdNumber = request.IdNumber,
            Initials = request.Initials,
            Lastname = request.LastName
          };
        }
        else
        {
          _log.Error("AddAVSWithResponse: Timed-out waiting for Task");
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "AddAVSWithResponse()");
      }

      return null;
    }


    /// <summary>
    /// Check on status of an AVS
    /// </summary>
    /// <param name="messageId"></param>
    /// <returns></returns>
    public static BankVerification.EasyNetQ.AVSResponse CheckAVS(Int64 messageId)
    {
      _log.Information("CheckAVS: Starting {MessageId}", messageId);

      try
      {
        var task = Task.Run<BankVerification.EasyNetQ.AVSResponse>(async () =>
          {
            return await _bus.RequestAsync<CheckAVSRequest, BankVerification.EasyNetQ.AVSResponse>(new CheckAVSRequest(messageId)).ConfigureAwait(false);           
          });

        if (task.Wait(5500))
        {
          return task.Result;
        }
        else
        {
          _log.Error("CheckAVS: Timed-out waiting for Task");
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "CheckAVS()");        
      }

      return null;
    }


    /// <summary>
    /// Message bus
    /// </summary>
    private static IBus _bus;

    private static ILogging _log;
    private static IConfigSettings _config;

  }
}
