/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Thread-safe, centralised storage for all fingerprint templates using the new MS Immutable collections
 *    & provides repository for accessing all fingerprints from MongoDB.
 *    
 *    Use to check new fingerprints, to ensure no duplicates.
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-11-20- Created
 *     
 *     2013-01-04- Converted from lock(object) to ReaderWriterLockSlim class 
 *                 Is this really going to be faster? Need testing once sufficient data populated...
 *                 
 *     2014-04-02- Converted staff template from traditional ReaderWriterLockSlim/Dictionary to new the new 
 *                 Immutable MS collections (1.0.8 BETA). These are ideally suited to this kind of requirement- 
 *                 it is far more memory efficient and they are inherently thread-safe.
 *                 
 *                 Handle rotated templates by extending the in-memory Dictionary key to include 'orientation'
 *                 
 *     2014-06-05  Back to ReaderWriterLockSlim/Dictionary- thought there was a problem with Immutable, but was actually
 *                 DTO and AutoMapper...
 *                 
 *     2014-11-11  Back to MS Immutable Collections... it rocks!
 * 
 *     2015-10-07  Moved all IB SDK/template functionality to distributed servers via RabbitMQ/Redis- now in DistCommUtils
 *     
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using DevExpress.Xpo;

using Atlas.Domain.Model.Biometric;
using Atlas.Domain.Security;
using Atlas.Enumerators;
using Atlas.Domain.Model;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.ClientState
{
  public sealed class LMSGuiState
  {
    public LMSGuiState(ILogging log)
    {
      _log = log;
    }


    #region Request - response - LMS FP client

    /// <summary>
    /// Adds a new request to the thread-safe list
    /// </summary>
    /// <param name="machineId">The machine id</param>
    /// <param name="requestId">The request id</param>
    /// <param name="status">Status of the request</param>
    public static void LmsGuiAddRequest(Int64 machineId, string requestId,
      Biometric.RequestStatus status, Int64 personId, Int64 userPersonId, Int64 adminPersonId,
      string message1, string message2, string message3, int timeoutSecs, bool webRequest = false)
    {
      _lmsGuiRequestLock.EnterWriteLock();
      try
      {
        List<FPGuiRequest> machineRequests;
        if (_lmsGuiRequestTracking.TryGetValue(machineId, out machineRequests))
        {
          var request = machineRequests.FirstOrDefault(s => s.RequestId == requestId);
          if (request != null)
          {
            // Update updateable fields
            request.Status = status;
            request.PersonId = personId;
            request.UserPersonId = userPersonId;
          }
          else
          {
            machineRequests.Add(new FPGuiRequest(requestId, status, DateTime.Now, personId, userPersonId, adminPersonId, message1, message2, message3, null, timeoutSecs, null, webRequest));
          }
        }
        else
        {
          machineRequests = new List<FPGuiRequest>();
          machineRequests.Add(new FPGuiRequest(requestId, status, DateTime.Now, personId, userPersonId, adminPersonId, message1, message2, message3, null, timeoutSecs, null, webRequest));
          _lmsGuiRequestTracking.Add(machineId, machineRequests);
        }
      }
      finally
      {
        _lmsGuiRequestLock.ExitWriteLock();
      }
    }


    /// <summary>
    /// Get pending requests for machine
    /// </summary>
    /// <param name="machineId">Machine id</param>
    /// <param name="maxAgeSeconds">Maximum age of request</param>
    /// <returns>List of requests, empty list none</returns>
    public static List<FPGuiRequest> LmsGuiGetPendingRequests(Int64 machineId, int maxAgeSeconds = 60)
    {
      var result = new List<FPGuiRequest>();
      _lmsGuiRequestLock.EnterReadLock();
      try
      {
        List<FPGuiRequest> machineRequests;
        if (_lmsGuiRequestTracking.TryGetValue(machineId, out machineRequests) && machineRequests.Count > 0)
        {
          result = machineRequests
            .Where(s => (
              s.Status == Biometric.RequestStatus.EnrollmentRequested ||
              s.Status == Biometric.RequestStatus.EnrollmentPending ||
              s.Status == Biometric.RequestStatus.IdentificationRequested ||
              s.Status == Biometric.RequestStatus.IdentificationPending ||
              s.Status == Biometric.RequestStatus.VerifyRequested ||
              s.Status == Biometric.RequestStatus.VerifyPending) &&
              DateTime.Now.Subtract(s.Started).TotalSeconds < maxAgeSeconds)
            .Select(s => s.DeepCopy()).ToList();
        }
      }
      finally
      {
        _lmsGuiRequestLock.ExitReadLock();
      }

      return result;
    }


    /// <summary>
    /// Update status of a request
    /// </summary>
    /// <param name="machineId">Machine id</param>
    /// <param name="requestId">Request id</param>
    /// <param name="status">Status to set the request to</param>
    public static void LmsGuiSetCommStatus(Int64 machineId, string requestId, Biometric.RequestStatus status,
      Int64 personId = 0, Int64 userPersonId = 0, Int64 adminPersonId = 0, byte[] compressedImage = null, Int64 otherPersonId = 0)
    {
      var preStatus = Biometric.RequestStatus.None;

      FPGuiRequest requestCopy = null;
      _lmsGuiRequestLock.EnterWriteLock();
      try
      {
        List<FPGuiRequest> machineRequests;
        if (_lmsGuiRequestTracking.TryGetValue(machineId, out machineRequests))
        {
          var request = machineRequests.FirstOrDefault(s => s.RequestId == requestId);
          if (request != null)
          {
            preStatus = request.Status;

            #region Update
            request.Status = status;
            if (personId > 0)
            {
              request.PersonId = personId;
            }

            if (userPersonId > 0)
            {
              request.UserPersonId = personId;
            }

            if (adminPersonId > 0)
            {
              request.AdminPersonId = adminPersonId;
            }

            if (compressedImage != null)
            {
              request.CompressedImage = new byte[compressedImage.Length];
              Array.Copy(compressedImage, request.CompressedImage, compressedImage.Length);
            }
            else
            {
              request.CompressedImage = null;
            }
            #endregion

            requestCopy = request.DeepCopy();
          }
        }
      }
      finally
      {
        _lmsGuiRequestLock.ExitWriteLock();
      }

      #region If just completed- log the result
      if ((requestCopy != null && preStatus != status) &&
          (status == Biometric.RequestStatus.EnrollmentCancelled ||
           status == Biometric.RequestStatus.EnrollmentFailed ||
           status == Biometric.RequestStatus.EnrollmentSuccessful ||

           status == Biometric.RequestStatus.IdentificationCancelled ||
           status == Biometric.RequestStatus.IdentificationFailed ||
           status == Biometric.RequestStatus.IdentificationSuccessful ||

           status == Biometric.RequestStatus.VerifyFailed ||
           status == Biometric.RequestStatus.VerifySuccessful ||
           status == Biometric.RequestStatus.VerifyCancelled ||
           status == Biometric.RequestStatus.EnrollmentDuplicated))
      {
        //// If a Falcon request, notify of the result
        //if (requestCopy.WebRequest)
        //{
        //  FPActivation.PublishToFalcon(requestCopy.RequestId, status == Biometric.RequestStatus.VerifySuccessful,
        //    !string.IsNullOrEmpty(requestCopy.ErrorMessage), requestCopy.ErrorMessage ?? string.Empty);
        //}

        using (var unitOfWork = new UnitOfWork())
        {
          new BIO_LogRequest(unitOfWork)
          {
            BiometricClass = Biometric.BiometricClass.Fingerprint,
            Machine = unitOfWork.Query<COR_Machine>().FirstOrDefault(s => s.MachineId == machineId),
            Person = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == requestCopy.PersonId),
            UserPerson = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == requestCopy.UserPersonId),
            StartDT = requestCopy.Started,
            EndDT = DateTime.Now,
            Error = otherPersonId > 0 ? otherPersonId.ToString() : requestCopy.ErrorMessage,
            RequestId = requestCopy.RequestId,
            TimeoutSecs = requestCopy.TimeoutSecs,
            FPResult = status,            
          };

          unitOfWork.CommitChanges();
        }
      }
      #endregion
    }

    #endregion


    #region Hardware monitoring

    /// <summary>
    /// Update in-memory hardware/software status for an endpoint
    /// </summary>
    /// <param name="machineId">Machine key</param>
    /// <param name="fpDeviceCount">Number of active fingerprint scanners</param>
    /// <param name="lastDBEntry">Last time we wrote the details to the DB</param>
    public static void LmsGuiUpdateHWStatus(string machineId, int fpDeviceCount, DateTime? lastDBEntry)
    {
      _lmsGuiHardwareLock.EnterWriteLock();
      try
      {
        FPHardware fpHardware;
        if (_lmsGuiHardware.TryGetValue(machineId, out fpHardware))
        {
          fpHardware.FPDeviceCount = fpDeviceCount;
          if (lastDBEntry != null)
          {
            fpHardware.LastDBEntry = (DateTime)lastDBEntry.Value;
          }
        }
        else
        {
          fpHardware = new FPHardware()
          {
            CreatedTS = DateTime.Now,
            FPDeviceCount = fpDeviceCount
          };
          if (lastDBEntry != null)
          {
            fpHardware.LastDBEntry = (DateTime)lastDBEntry.Value;
          }
          _lmsGuiHardware.Add(machineId, fpHardware);
        }
      }
      finally
      {
        _lmsGuiHardwareLock.ExitWriteLock();
      }
    }


    /// <summary>
    /// Returns in-memory hardware/software information for an endpoint
    /// </summary>
    /// <param name="machineId">The machine key</param>
    /// <returns>Hardware/software information</returns>
    public static FPHardware LmsGuiGetHWStatus(string machineId)
    {
      _lmsGuiHardwareLock.EnterReadLock();
      try
      {
        FPHardware fpHardware;
        if (_lmsGuiHardware.TryGetValue(machineId, out fpHardware))
        {
          return fpHardware.DeepCopy();
        }

        return null;
      }
      finally
      {
        _lmsGuiHardwareLock.ExitReadLock();
      }
    }


    /// <summary>
    /// Find a request by machine and request
    /// </summary>
    /// <param name="machineId">Machine fingerprint</param>
    /// <param name="requestId">Request GUID</param>    
    /// <returns>null if not found, else matching FPGuiRequest</returns>
    public static FPGuiRequest GetGUICommStatus(Int64 machineId, string requestId)
    {
      _lmsGuiRequestLock.EnterReadLock();
      try
      {
        List<FPGuiRequest> machineRequests;
        if (_lmsGuiRequestTracking.TryGetValue(machineId, out machineRequests))
        {
          var request = machineRequests.FirstOrDefault(s => s.RequestId == requestId);
          if (request != null)
          {
            return request.DeepCopy();
          }
        }
      }
      finally
      {
        _lmsGuiRequestLock.ExitReadLock();
      }

      return null;
    }

    #endregion

   
    #region Private methods
    
    /// <summary>
    /// Cleans out GUI/LMS queue items older than 60 minutes
    /// </summary>
    public static void LmsGuiSessionDeleteOld()
    {
      var logRequests = new List<FPGuiRequest>();

      #region Remove expired requests
      _lmsGuiRequestLock.EnterWriteLock();
      try
      {
        foreach (var machineQueue in _lmsGuiRequestTracking)
        {
          for (var i = machineQueue.Value.Count - 1; i >= 0; i--)
          {
            if (DateTime.Now.Subtract(machineQueue.Value[i].Started).TotalMinutes > 60)
            {
              Biometric.RequestStatus result;
              switch (machineQueue.Value[i].Status)
              {
                case Biometric.RequestStatus.EnrollmentPending:
                case Biometric.RequestStatus.EnrollmentRequested:
                  result = Biometric.RequestStatus.EnrollmentTimedOut;
                  break;

                case Biometric.RequestStatus.IdentificationPending:
                case Biometric.RequestStatus.IdentificationRequested:
                  result = Biometric.RequestStatus.IdentificationTimedOut;
                  break;

                case Biometric.RequestStatus.VerifyPending:
                case Biometric.RequestStatus.VerifyRequested:
                  result = Biometric.RequestStatus.VerifyTimedOut;
                  break;

                default:
                  result = Biometric.RequestStatus.TimedOut;
                  break;
              }

              var requestCopy = machineQueue.Value[i].DeepCopy();
              requestCopy.Status = result;
              logRequests.Add(requestCopy);

              machineQueue.Value.RemoveAt(i);
            }
          }
        }
      }
      finally
      {
        _lmsGuiRequestLock.ExitWriteLock();
      }
      #endregion

      #region Log any expired requests
      if (logRequests.Count > 0)
      {
        using (var unitOfWork = new UnitOfWork())
        {
          foreach (var logRequest in logRequests)
          {

          }
        }
      }
      #endregion
    }

    #endregion


    #region Private vars
    
    /// <summary>
    /// Thread-safe access to _lmsGuiRequestTracking data
    /// </summary>
    private static readonly ReaderWriterLockSlim _lmsGuiRequestLock = new ReaderWriterLockSlim();

    /// <summary>
    /// Store LMS/GUI interaction state- use MachineId as dictionary key value.
    /// </summary>
    private static readonly Dictionary<long, List<FPGuiRequest>> _lmsGuiRequestTracking = new Dictionary<Int64, List<FPGuiRequest>>();

    // Store the hardware status of each machine
    /// <summary>
    /// Thread-safe access to _lmsGuiHardware
    /// </summary>
    private static readonly ReaderWriterLockSlim _lmsGuiHardwareLock = new ReaderWriterLockSlim();

    /// <summary>
    /// In-memory, key hardware/software information about an endpoint (uploaded by endpoint itself)
    /// </summary>
    private static readonly Dictionary<string, FPHardware> _lmsGuiHardware = new Dictionary<string, FPHardware>();

    // Log4net
    private readonly ILogging _log;

    #endregion

  }
}