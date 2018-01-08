using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using Atlas.Domain.DTO;
using Atlas.Enumerators;
using AvsEngineLight.DB;
using AvsEngineLight.Handlers.NuCard.Xml;
using Atlas.Common.Utils;
using Atlas.Common.Interface;
using System.Diagnostics;

namespace AvsEngineLight.Handlers.NuCard
{
  internal class NuCardHandler
  {
    public NuCardHandler(ILogging log)
    {
      _log = log;
    }


    internal void Start()
    {
      try
      {
        #region Get config
        int scanIntervalMs;
        string userName;
        string userKey;

        string merchantNum = "000000000013413"; // The merchant number(Card Acceptor) requires padded 0’s
        AvsDbRepository.GetAvsSettings(AVS.Service.NuCard, out userName, out userKey, out scanIntervalMs);
        #endregion

        #region Get operating schedule
        // Schedules are unlikely to change, so hard-coding is fine.
        var mon2FriStartMinute = (6 * 60) + 30; // 06:30 - NuCard operates 24x7
        var mon2FriEndMinute = (19 * 60) + 30;  // 18:30 - NuCard operates operate s24x7, but not much point unless we have online...

        var satStartMinute = (6 * 60) + 30; // 06:30 - NuCard operates 24x7, but not much point unless we have online...
        var satEndMinute = (15 * 60) + 30;  // 15:30 - NuCard operates 24x7, but not much point unless we have online...
        #endregion

        var upTime = Stopwatch.StartNew();
        while (!_terminate.Wait(scanIntervalMs))
        {
          try
          {
            //var currMinute = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
            //var startTime = DateTime.Now.DayOfWeek == DayOfWeek.Saturday ? satStartMinute : mon2FriStartMinute;
            //var endTime = DateTime.Now.DayOfWeek == DayOfWeek.Saturday ? satEndMinute : mon2FriEndMinute;

            //if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday && currMinute >= startTime && currMinute <= endTime) // Only operate during AVS service hours...
            {
              // New AVS requests logged and not yet submitted
              var queuedTrans = AvsDbRepository.GetQueuedAvs(AVS.Service.NuCard);
              HandleQueued(userName, userKey, merchantNum, queuedTrans);

              // Handle stuff we are waiting on feedback- Transactions submitted with service provider
              var pendingTransIds = AvsDbRepository.GetPendingAvs(AVS.Service.NuCard);
              HandlePending(userName, userKey, merchantNum, pendingTransIds);

              if (upTime.Elapsed > TimeSpan.FromMinutes(10)) // Don't expire anything for first 10 minutes of AVS being active            
              {
                AvsDbRepository.ExpireOldAVS(AVS.Service.NuCard, 2, _log);
              }
            }
          }
          catch (Exception err)
          {
            _log.Error(err, "Start()");
          }
        }
      }
      finally
      {
        _log.Information("Handler stopped");
      }
    }


    /// <summary>
    /// Handle new NuCard requests- send to NuCard
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="userKey"></param>
    /// <param name="queuedTrans"></param>
    private void HandleQueued(string userName, string userKey, string merchantNum, List<AVS_TransactionDTO> queuedTrans)
    {
      if (queuedTrans == null || !queuedTrans.Any())
      {
        return;
      }

      #region Async task Submit
      var submitted = new ConcurrentBag<Tuple<Int64, XmlNode, string>>();
      var tasks = new Task[queuedTrans.Count];
      for (int i = 0; i < queuedTrans.Count; i++)
      {
        var queue = queuedTrans[i];
        tasks[i] = Task.Run(async () =>
        {
          try
          {
            using (var client = new wsAVSRSoapClient())
            {
              var idNumber = (IdValidator2.IsValidSouthAfricanId(queue.IdNumber)) ?
                queue.IdNumber : StringUtils.RemoveNonNumeric(queue.IdNumber);
              
              var enquiryResult = await client.accountValidateAsync(merchantNum, userName, userKey, "",
                GetBankString(queue.Bank.Type), queue.BranchCode, queue.AccountNo, "00", idNumber, queue.Initials, queue.LastName, "Y", "Y", "Y");
              submitted.Add(new Tuple<long, XmlNode, string>(queue.TransactionId, enquiryResult, null));
            }
          }
          catch (Exception err)
          {
            _log.Error(err, "wsAVSRSoapClient()");
            submitted.Add(new Tuple<long, XmlNode, string>(queue.TransactionId, null, err.Message));
          }
        });
      }

      Task.WaitAll(tasks);
      #endregion

      #region Process responses
      Tuple<Int64, XmlNode, string> item;
      while (submitted.TryTake(out item))
      {
        if (item.Item2?.OuterXml != null && string.IsNullOrEmpty(item.Item3)) // Result and no WCF error
        {
          var result = DeserializeXmlResult(item.Item2.OuterXml);
          if (result != null && result.ReportError != null && !string.IsNullOrEmpty(result.ReportError.error_code))
          {
            AvsDbRepository.UpdateWithError(item.Item1, string.Format("{0}-{1}", result.ReportError.error_code, result.ReportError.error_detail),
              AVS.Status.Complete, AVS.Result.NoResult);
          }
          else if (result != null && result.accountDetailResult != null && !string.IsNullOrWhiteSpace(result.accountDetailResult.request_id))
          {
            var transactionId = result.accountDetailResult.request_id;
            AvsDbRepository.UpdateThirdParty(item.Item1, transactionId);
          }
          else
          {
            _log.Error("Invalid response- {@Response}", item.Item2);
            AvsDbRepository.UpdateWithError(item.Item1, string.Format("Invalid response- {0}", item.Item2));
          }
        }
        else
        {
          if (!string.IsNullOrEmpty(item.Item3))
          {
            AvsDbRepository.UpdateWithError(item.Item1, item.Item3);
          }
          else
          {
            _log.Error("null response");
            AvsDbRepository.UpdateWithError(item.Item1, "null response");
          }
        }
      }
      #endregion
    }


    /// <summary>
    /// Handles transactions in the 'Pending' state
    /// </summary>  
    /// <param name="pendingTransIds">List of Tuple: AVS_Transaction.TransactionId - NuCard ABSA Reference </param>
    /// <returns>List of Tuple: AVS_Transaction.TransactionId - NuCard Reference - error</returns>
    private void HandlePending(string userName, string userKey, string merchantNum, List<Tuple<Int64, string>> pendingTransIds)
    {
      if (pendingTransIds == null || !pendingTransIds.Any())
      {
        return;
      }

      #region Async task enquiry
      var processed = new ConcurrentBag<Tuple<Int64, XmlNode, string>>();      // AVS transId,result XML, error
      var tasks = new Task[pendingTransIds.Count];
      for (var i = 0; i < pendingTransIds.Count; i++)
      {
        var enquiry = pendingTransIds[i];
        tasks[i] = Task.Run(async () =>
        {
          using (var client = new wsAVSRSoapClient())
          {
            try
            {
              var addResult = await client.accountValidateAsync(merchantNum, userName, userKey, enquiry.Item2, "", "", "", "", "", "", "", "", "", "");
              processed.Add(new Tuple<Int64, XmlNode, string>(enquiry.Item1, addResult, null));
            }
            catch (Exception err)
            {
              _log.Error(err, "wsAVSRSoapClient()");
              processed.Add(new Tuple<Int64, XmlNode, string>(enquiry.Item1, null, err.Message));
            }
          }
        });
      }
      Task.WaitAll(tasks);
      #endregion

      #region Process responses
      Tuple<Int64, XmlNode, string> item;
      while (processed.TryTake(out item))
      {
        if (item.Item2?.OuterXml != null && string.IsNullOrEmpty(item.Item3))
        {
          var result = DeserializeXmlResult(item.Item2.OuterXml);

          if (result != null)
          {
            if (result.accountDetailResult != null)
            {
              var accResult = result.accountDetailResult;
              switch (accResult.respCode)
              {
                case 0:// success
                  AvsDbRepository.UpdateWithResponse(item.Item1, GetResponseResult(accResult.accCredits), GetResponseResult(accResult.accDebits),
                    GetResponseResult(accResult.accFound), GetResponseResult(accResult.accOpen), GetResponseResult(accResult.idMatch),
                    GetResponseResult(accResult.initMatch), GetResponseResult(accResult.nameMatch), GetResponseResult(accResult.accLenghtmatch));

                  break;

                case 1: // pending
                  break;

                case 33: // tech error
                  AvsDbRepository.UpdateWithError(item.Item1, "33- SP technical error", AVS.Status.Complete, AVS.Result.NoResult);

                  break;

                case 98: // duplicate.. urk, what to do?
                  AvsDbRepository.UpdateWithError(item.Item1, "98- Duplicate", AVS.Status.Complete, AVS.Result.NoResult);

                  break;

                case 99: // time-out
                  AvsDbRepository.UpdateWithError(item.Item1, "99- Time-out", AVS.Status.Complete, AVS.Result.NoResult);

                  break;
              }
            }
          }
          else
          {
            _log.Error("Invalid response- {@Response}", item.Item2);
            AvsDbRepository.UpdateWithError(item.Item1, string.Format("Invalid response- {0}", item.Item2));
          }
        }
        else
        {
          // error with submission
          AvsDbRepository.UpdateWithError(item.Item1, "Invalid response", AVS.Status.Complete, AVS.Result.NoResult);
        }
      }
      #endregion
    }


    public void Stop()
    {
      _terminate.Set();
    }



    private report DeserializeXmlResult(string result)
    {
      try
      {
        return Deserialize<report>(result);
      }
      catch (Exception err)
      {
        _log.Error(err, "AccountVerificationResponse- {@result}", result);
        return null;
      }
    }


    private static T Deserialize<T>(string xmlText) where T : class, new()
    {
      if (string.IsNullOrEmpty(xmlText))
      {
        return null;
      }

      var serializer = new XmlSerializer(typeof(T));
      using (var sr = new StringReader(xmlText))
      {
        return (T)serializer.Deserialize(sr);
      }
    }


    private static string GetBankString(General.BankName bank)
    {
      switch (bank)
      {
        case General.BankName.ABS:
          return "000006";

        case General.BankName.CAP:
          return "000034";

        case General.BankName.FNB:
          return "000005";

        case General.BankName.NED:
          return "000021";

        case General.BankName.STD:
          return "000016";

        case General.BankName.AFR:
          return "000036";

        default:
          throw new NotSupportedException("Bank not supported");
      }
    }


    private static AVS.ResponseResult GetResponseResult(string result)
    {
      switch (result)
      {
        case "Y":
          return AVS.ResponseResult.Passed;

        case "N":
          return AVS.ResponseResult.Failed;

        default:
          return AVS.ResponseResult.NoResponse;
      }
    }


    #region Private fields

    /// <summary>
    /// Flag to indicate termination
    /// </summary>
    private ManualResetEventSlim _terminate = new ManualResetEventSlim();


    /// <summary>
    /// Logging
    /// </summary>
    private readonly ILogging _log;

    #endregion
  }
}
