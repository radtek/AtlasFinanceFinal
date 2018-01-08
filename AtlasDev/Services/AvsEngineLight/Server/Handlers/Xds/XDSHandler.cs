using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using Atlas.Domain.DTO;
using Atlas.Common.Extensions;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using AvsEngineLight.DB;
using Atlas.Common.Interface;
using AvsEngineLight.Handlers.Xds.Xml;
using System.Diagnostics;

namespace AvsEngineLight.AVSProviders.Handlers
{
  /// <summary>
  /// Class to handle pending XDS AVS
  /// </summary>
  internal class XDSHandler
  { 

    public XDSHandler(ILogging log)
    {
      _log = log;
    }


    #region Public methods

    public void Start()
    {
      try
      {
        #region Get config
        string userName;
        string userKey;
        int scanIntervalMs;
        AvsDbRepository.GetAvsSettings(AVS.Service.XDS, out userName, out userKey, out scanIntervalMs);
        #endregion

        //#region Get operating schedule
        //// Schedules are unlikely to change, so hard-coding is fine.
        //var mon2FriStartMinute = (6 * 60) + 30; // 06:30 - XDS operates from 03:00, but not much point unless we have online...
        //var mon2FriEndMinute = (19 * 60) + 30;  // 18:30 - XDS operates to 20:00, but not much point unless we have online...

        //var satStartMinute = (6 * 60) + 30; // 06:30 - XDS operates from 03:00, but not much point unless we have online...
        //var satEndMinute = (15 * 60) + 30;  // 15:30 - XDS operates to 20:00, but not much point unless we have online...
        //#endregion

        var upTime = Stopwatch.StartNew();

        while (!_terminate.Wait(scanIntervalMs))
        {
          try
          {
            //var currMinute = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
            //var startTime = DateTime.Now.DayOfWeek == DayOfWeek.Saturday ? satStartMinute : mon2FriStartMinute;
            //var endTime = DateTime.Now.DayOfWeek == DayOfWeek.Saturday ? satEndMinute : mon2FriEndMinute;

            //if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday && currMinute >= startTime && currMinute <= endTime) //Only operate during AVS service hours...
            {
              // New AVS requests logged and not yet submitted
              var queuedTrans = AvsDbRepository.GetQueuedAvs(AVS.Service.XDS);
              HandleQueued(userName, userKey, queuedTrans);

              // Handle stuff we are waiting on feedback- Transactions submitted with service provider
              var pendingTransIds = AvsDbRepository.GetPendingAvs(AVS.Service.XDS);
              HandlePending(userName, userKey, pendingTransIds);

              if (upTime.Elapsed > TimeSpan.FromMinutes(10)) // Don't expire anything for first 10 minutes of AVS being active
              {
                AvsDbRepository.ExpireOldAVS(AVS.Service.XDS, 5, _log);
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


    public void Stop()
    {
      _terminate.Set();
    }

    #endregion


    #region Private methods

    /// <summary>
    /// Handle new XDS requests- send to XDS
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="userKey"></param>
    /// <param name="queuedTrans"></param>
    private void HandleQueued(string userName, string userKey, List<AVS_TransactionDTO> queuedTrans)
    {
      if (queuedTrans == null || !queuedTrans.Any())
      {
        return;
      }

      #region Async task Submit
      var submitted = new ConcurrentBag<Tuple<Int64, string, string>>();
      var tasks = new Task[queuedTrans.Count];
      for (int i = 0; i < queuedTrans.Count; i++)
      {
        var queue = queuedTrans[i];
        tasks[i] = Task.Run(async () =>
        {
          try
          {
            using (var client = new XDSConnectWSSoapClient())
            {
              var token = await client.LoginAsync(userName, userKey);

              // Handle passports
              var idNumber = queue.IdNumber;
              var avsIdType = AVSIDType.SID;
              if (!new Atlas.Common.Utils.IDValidator(queue.IdNumber).isValid())
              {
                avsIdType = AVSIDType.FPP;
                // XDS passport numbers must just contain numerics
                idNumber = Atlas.Common.Utils.StringUtils.RemoveNonNumeric(queue.IdNumber).Trim();               
              }

              var enquiryResult = await client.ConnectAccountVerificationRealTimeAsync(
                token, TypeofVerificationenum.Individual, Entity.None,
                queue.Initials, queue.LastName.PadRight(3, '_'), idNumber, avsIdType, "", "", "", "", "",                         // LastName must be 3 chars or more for XDS
                queue.AccountNo, queue.BranchCode, "Savings Account", GetBank((General.BankName)queue.Bank.BankId), "", "");
              submitted.Add(new Tuple<long, string, string>(queue.TransactionId, enquiryResult, null));
            }
          }
          catch (Exception err)
          {
            _log.Error(err, "XDSConnectWSSoapClient()");
            submitted.Add(new Tuple<long, string, string>(queue.TransactionId, null, err.Message));
          }
        });
      }
      Task.WaitAll(tasks);
      #endregion

      #region Process responses
      Tuple<Int64, string, string> item;
      while (submitted.TryTake(out item))
      {
        if (!string.IsNullOrEmpty(item.Item2) && string.IsNullOrEmpty(item.Item3)) // Result and no error
        {
          var verification = AccountVerificationResponse(item.Item2);
          if (verification != null)
          {
            var transactionId = verification.ReferenceNo;
            AvsDbRepository.UpdateThirdParty(item.Item1, transactionId);
          }
          else
          {
            _log.Error("Invalid response- {Response}", item.Item2);
            AvsDbRepository.UpdateWithError(item.Item1, string.Format("Invalid response- {0}", item.Item2));
          }
        }
        else
        {
          _log.Warning("Response: {Response}", item.Item3 ?? "No response");
          AvsDbRepository.UpdateWithError(item.Item1, item.Item3 ?? "No response");
        }
      }
      #endregion
    }


    /// <summary>
    /// Handles transactions in the 'Pending' state
    /// </summary>
    /// <param name="xdsUser"></param>
    /// <param name="xdsKey"></param>
    /// <param name="pendingTransIds">List of Tuple: AVS_Transaction.TransactionId - XDS Reference </param>
    /// <returns>List of Tuple: AVS_Transaction.TransactionId - XDS Reference - error</returns>
    private void HandlePending(string xdsUser, string xdsKey, List<Tuple<Int64, string>> pendingTransIds)
    {
      if (pendingTransIds == null || !pendingTransIds.Any())
      {
        return;
      }

      #region Async task enquiry
      var processed = new ConcurrentBag<Tuple<Int64, string, string>>();      // AVS transId,result XML, error
      var tasks = new Task[pendingTransIds.Count];
      for (var i = 0; i < pendingTransIds.Count; i++)
      {
        var enquiry = pendingTransIds[i];
        tasks[i] = Task.Run(async () =>
          {
            using (var client = new XDSConnectWSSoapClient())
            {
              try
              {
                var token = await client.LoginAsync(xdsUser, xdsKey);
                var addResult = await client.ConnectGetAccountVerificationResultAsync(token, int.Parse(enquiry.Item2));
                processed.Add(new Tuple<Int64, string, string>(enquiry.Item1, addResult, null));
              }
              catch (Exception err)
              {
                _log.Error(err, "XDSConnectWSSoapClient()");
                processed.Add(new Tuple<Int64, string, string>(enquiry.Item1, null, err.Message));
              }
            }
          });
      }
      Task.WaitAll(tasks);
      #endregion

      #region Process responses
      Tuple<Int64, string, string> item;
      while (processed.TryTake(out item))
      {
        if (!string.IsNullOrEmpty(item.Item2) && string.IsNullOrEmpty(item.Item3))
        {
          var noResult = NoResultResponse(item.Item2);
          if (noResult == null)
          {
            var avsResult = AVSResultResponse(item.Item2);
            if (avsResult != null)
            {
              if (!string.IsNullOrEmpty(avsResult.ResultFile.ERRORCONDITIONNUMBER) &&
                string.Compare(avsResult.ResultFile.ERRORCONDITIONNUMBER, "Not Available", true) != 0)
              {
                AvsDbRepository.UpdateWithError(item.Item1, avsResult.ResultFile.ERRORCONDITIONNUMBER);
                _log.Warning("Response: {@Response}", avsResult.ResultFile);
              }
              else
              {
                AvsDbRepository.UpdateWithResponse(item.Item1, GetResponseResult(avsResult.ResultFile.ACCOUNTACCEPTSCREDITS),
                  GetResponseResult(avsResult.ResultFile.ACCOUNTACCEPTSDEBITS), GetResponseResult(avsResult.ResultFile.ACCOUNTFOUND),
                  GetResponseResult(avsResult.ResultFile.ACCOUNTOPEN), GetResponseResult(avsResult.ResultFile.IDNUMBERMATCH),
                  GetResponseResult(avsResult.ResultFile.INITIALSMATCH), GetResponseResult(avsResult.ResultFile.SURNAMEMATCH),
                  GetResponseResult(avsResult.ResultFile.ACCOUNTOPENFORATLEASTTHREEMONTHS));

                _log.Information("Response: {@Response}", avsResult.ResultFile);
              }
            }
            else
            {
              _log.Error("Invalid response- {Response}", item.Item2);
              AvsDbRepository.UpdateWithError(item.Item1, string.Format("Invalid response- {0}", item.Item2));
            }
          }
          else if (string.IsNullOrEmpty(noResult.Error))
          {
            // no error, just waiting for response from banks
            AvsDbRepository.UpdateWithError(item.Item1, null);
          }
          else
          {
            // error with submission
            AvsDbRepository.UpdateWithError(item.Item1, noResult.Error, AVS.Status.Complete, AVS.Result.NoResult);
          }
        }
        else
        {
          _log.Warning("Response: {Response}", item.Item3 ?? "No response");
          AvsDbRepository.UpdateWithError(item.Item1, item.Item3 ?? "No response");
        }
      }
      #endregion
    }


    /// <summary>
    /// Convert enum to string for Xds
    /// </summary>
    /// <param name="bank"></param>
    /// <returns></returns>
    private static string GetBank(General.BankName bank)
    {
      switch (bank)
      {
        case Atlas.Enumerators.General.BankName.ABS: return "ABSA";
        case Atlas.Enumerators.General.BankName.STD: return "STANDARD BANK";
        case Atlas.Enumerators.General.BankName.FNB: return "FNB";
        case Atlas.Enumerators.General.BankName.NED: return "NEDBANK";
        case Atlas.Enumerators.General.BankName.CAP: return "CAPITEC";
        case Atlas.Enumerators.General.BankName.AFR: return "AFRICAN BANK";
        case Atlas.Enumerators.General.BankName.MER: return "MERCANTILE BANK";
        default: return string.Empty;
      }
    }


    private static AVS.ResponseResult GetResponseResult(string description)
    {
      switch (description.Trim().ToLower())
      {
        case "yes": return AVS.ResponseResult.Passed;
        case "no": return AVS.ResponseResult.Failed;
        default: return AVS.ResponseResult.NoResponse;
      }
    }


    /// <summary>
    /// Deserialize 'result' XML to NoResult
    /// </summary>
    /// <param name="result">The XML string to seialize</param>
    /// <returns>Deserialized NoResult, else null if deserialization failed</returns>
    private static NoResult NoResultResponse(string result)
    {
      try
      {
        NoResult avsResult;
        using (var memStreamResult = new MemoryStream(Encoding.UTF8.GetBytes(result)))
        {
          var serializerResult = new XmlSerializer(typeof(NoResult));
          avsResult = (NoResult)serializerResult.Deserialize(memStreamResult);
        }
        return avsResult;
      }
      catch (InvalidOperationException)
      {
        return null;
      }
    }


    private AVSResult.AccountVerificationResult AVSResultResponse(string xmlText)
    {
      try
      {
        AVSResult.AccountVerificationResult avsResult;
        using (var memStreamResult = new MemoryStream(Encoding.UTF8.GetBytes(xmlText)))
        {
          var serializerResult = new XmlSerializer(typeof(AVSResult.AccountVerificationResult));
          avsResult = (AVSResult.AccountVerificationResult)serializerResult.Deserialize(memStreamResult);
        }
        return avsResult;
      }
      catch (Exception err)
      {
        _log.Error(err, "AccountVerificationResponse- {@AVSResultResponse}", xmlText);
        return null;
      }
    }


    private AccountVerification AccountVerificationResponse(string xmlText)
    {
      try
      {
        AccountVerification avsResult;
        using (var memStreamResult = new MemoryStream(Encoding.UTF8.GetBytes(xmlText)))
        {
          var serializerResult = new XmlSerializer(typeof(AccountVerification));
          avsResult = (AccountVerification)serializerResult.Deserialize(memStreamResult);
        }
        return avsResult;
      }
      catch (Exception err)
      {
        _log.Error(err, "AccountVerificationResponse- {@result}", xmlText);
        return null;
      }
    }

    #endregion


    #region Private fields

    /// <summary>
    /// Flag to indicate termination
    /// </summary>
    private ManualResetEventSlim _terminate = new ManualResetEventSlim();
    private readonly ILogging _log;


    #endregion

  }
}
