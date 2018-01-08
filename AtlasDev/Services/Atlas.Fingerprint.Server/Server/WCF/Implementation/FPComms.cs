/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013-2015 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Implementation of Fingerprint communications interface- 'disconnected' comms between LMS and GUI client.
 *     The communications are facilitated via simple polling- comms are non-persistent, via Dictionary 
 *     and ReaderWriterLockSlim (see FPThreadSafe.cs).
 *     
 * 
 *     Definitions:
 *     ------------
 *        LMS- Core Loan Management System, i.e. ASS (current) or website (future)
 *        
 *        GUI Client- Local software permanently installed on all Atlas Desktops- provides for local WebCam/Fingerprint/etc.
 *        hardware access.
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-01-08- Basic functionality started
 *     
 *     2013-01-14- Replaced RabbitMQ/Protocol buffers with simple Dictionary/List and ReaderWriterLockSlim
 *     
 *     2013-01-16- Added 'identification'
 *     
 *     2013-01-17- Added 3 x messages, which can be used to display custom messages to the end-user
 *     
 * -----------------------------------------------------------------------------------------------------------------  */

using System;
using System.Collections.Generic;
using System.ServiceModel;

using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.WCF.Implementation.Comms;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;


namespace Atlas.WCF.FPServer.WCF.Implementation
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class FPComms : IFPComms
  {
    public FPComms(ILogging log, ICacheServer cache)
    {
      _log = log;
      _cache = cache;
    }

    /// <summary>
    /// Determines if machine has been enabled for fingerprinting in BIO_Config table
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="isEnabled">Is this branch enabled for fingerprinting (requires hardware and Atlas GUI software)</param>
    /// <param name="errorMessage"></param>
    /// <returns>Enumerators.WCFCallResult</returns>
    public int LMS_MachineToUseFP(SourceRequest sourceRequest, out bool isEnabled, out string errorMessage)
    {
      return LMS_MachineToUseFP_Impl.Execute(_log, sourceRequest, out isEnabled, out errorMessage);
    }


    /// <summary>
    /// Determines if machine must capture clients
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="isEnabled">Must clients be enrolled?</param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public int LMS_MachineToUseFPForClients(SourceRequest sourceRequest, out bool isEnabled, out string errorMessage)
    {
      return LMS_MachineToUseFPForClients_Impl.Execute(_log, sourceRequest, out isEnabled, out errorMessage);
    }


    /// <summary>
    /// Determines if this machine's 'Atlas GUI' is ready and has a FP device connected
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="isReady"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public int LMS_IsMachineFPReady(SourceRequest sourceRequest, out bool isReady, out string errorMessage)
    {
      return LMS_IsMachineFPReady_Impl.Execute(_log, sourceRequest, out isReady, out errorMessage);
    }


    /// <summary>
    /// Gets persons details via their PersonId
    /// </summary>
    /// <param name="sourceRequest">Source client/user details<</param>
    /// <param name="personId">The PER_Person.PersonId</param>
    /// <param name="personDetails">The found person</param>
    /// <param name="errorMessage">server error message</param>
    /// <returns>Enumerators.WCFCallResult</returns>
    public int GetPersonViaPersonId(SourceRequest sourceRequest, Int64 personId,
      out BasicPersonDetailsDTO personDetails, out string errorMessage)
    {
      return GetPersonViaPersonId_Impl.Execute(_log, sourceRequest, personId, out personDetails, out errorMessage);
    }


    /// <summary>
    /// Gets Person for a person, via their SA ID or passport
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="idOrPassport">ID number or passport</param>
    /// <param name="personId">Resultant Person.PersonId</param>
    /// <param name="errorMessage">Any server error message</param>
    /// <returns>Enumerators.WCFCallResult</returns>
    public int GetPersonViaIdOrPassport(SourceRequest sourceRequest, string idOrPassport,
      out BasicPersonDetailsDTO personDetails, out string errorMessage)
    {
      return GetPersonViaIdOrPassport_Impl.Execute(_log, sourceRequest, idOrPassport, out personDetails, out errorMessage);
    }


    /// <summary>
    /// Gets PersonId via ASS Operator ID
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="operatorId">ASS Operator ID</param>
    /// <param name="personId">Resultant Person.PersonId</param>
    /// <param name="errorMessage">server error message</param>
    /// <returns>Enumerators.WCFCallResult</returns>
    public int GetPersonViaOperatorId(SourceRequest sourceRequest, string personOperatorId,
      out BasicPersonDetailsDTO personDetails, out string errorMessage)
    {
      return GetPersonViaOperatorId_Impl.Execute(_log, sourceRequest, personOperatorId, out personDetails, out errorMessage);
    }


    /// <summary>
    /// Creates a new person in the database
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="dateOfBirth">Date of borth</param>
    /// <param name="firstName">First name</param>
    /// <param name="otherNames">Other names</param>
    /// <param name="lastName">Last name</param>
    /// <param name="idOrPassport">SA ID or passport</param>
    /// <param name="cellPhoneNumber">Cellular number</param>
    /// <param name="eMailAddress">e-mail address</param>
    /// <param name="gender">Gender- 'M' or 'F'</param>
    /// <param name="personId">Resultant PersonId</param>
    /// <param name="errorMessage">Server error message</param>
    /// <returns>Enumerators.WCFCallResult</returns>
    public int CreatePerson(SourceRequest sourceRequest, BasicPersonDetailsDTO personDetails,
      out Int64 personId, out string errorMessage)
    {
      return CreatePerson_Impl.Execute(_log, sourceRequest, personDetails, out personId, out errorMessage);
    }


    /// <summary>
    /// Determines which fingers have been enrolled into MongoDB for this person
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="personId">Person.PersonId</param>
    /// <param name="fingerEnrolledCount">Number of fingers enrolled for this person</param>
    /// <param name="errorMessage">Any errors</param>
    /// <returns>Enumerators.WCFCallResult</returns>
    public int GetPersonEnrolledFingers(SourceRequest sourceRequest, Int64 personId,
      out List<int> enrolledFingers, out string errorMessage)
    {
      return GetPersonEnrolledFingers_Impl.Execute(_log, sourceRequest, personId, out enrolledFingers, out errorMessage);
    }


    /// <summary>
    /// Loan management system- accept a new request
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="personId">PersonId for fingerprint request (not required for identification)</param>
    /// <param name="requestType">Fingeprint request type</param>
    /// <param name="requestId">Returns new request (GUID)</param>
    /// <param name="errorMessage"></param>
    /// <returns>Enumerators.General.WCFCallResult</returns>
    public int LMS_AddRequest(SourceRequest sourceRequest, FPRequestType requestType,
      Int64 personId, Int64 userPersonId, Int64 adminPersonId,
      string message1, string message2, string message3, int timeoutSecs,
      out string requestId, out string errorMessage)
    {
      return LMS_AddRequest_Impl.Execute(_log, sourceRequest, requestType, personId, userPersonId, adminPersonId,
        message1, message2, message3, timeoutSecs, out requestId, out errorMessage);
    }

    /// <summary>
    /// Loan management system- accept a new request
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="personId">PersonId for fingerprint request (not required for identification)</param>
    /// <param name="requestType">Fingeprint request type</param>
    /// <param name="requestId">Returns new request (GUID)</param>
    /// <param name="errorMessage"></param>
    /// <returns>Enumerators.General.WCFCallResult</returns>
    public int FPC_AddRequest(SourceRequest sourceRequest, FPRequestType requestType,
      string trackingId, Int64 personId, Int64 userPersonId, Int64 adminPersonId,
      string message1, string message2, string message3, int timeoutSecs)
    {
      string errorMessage;
      return FPC_AddRequest_Impl.Execute(_log, sourceRequest, requestType, trackingId, personId, userPersonId, adminPersonId,
        message1, message2, message3, timeoutSecs, out errorMessage);
    }


    /// <summary>
    /// LMS user has cancelled a pending operation
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="requestId"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public int LMS_UserCancelled(SourceRequest sourceRequest, string requestId, out string errorMessage)
    {
      return LMS_UserCancelled_Impl.Execute(_log, sourceRequest, requestId, out errorMessage);
    }


    /// <summary>
    /// Fingerprint client- request completed
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="requestId"></param>
    /// <param name="status"></param>
    /// <param name="errorMessage"></param>
    /// <param name="capturedImage">Raw 8-bit image</param>
    /// <returns></returns>
    public int FPC_SetRequestDone(SourceRequest sourceRequest, string requestId, FPRequestStatus status, byte[] wsq,
      Int64 personId, Int64 userPersonId, Int64 adminPersonId, int fingerId,
      out string errorMessage)
    {
      return FPC_SetRequestDone_Impl.Execute(_log, sourceRequest,
        requestId, status, wsq, personId, userPersonId, adminPersonId, fingerId, out errorMessage);
    }


    // TODO: !!! Remove once Alberton upgraded  !!!!
    /// <summary>
    /// Local Fingerprint GUI Client- check for new requests from other system(s) 
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="requestId">Request ID (GUID)</param>
    /// <param name="requestType">Returns the request type</param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public int FPC_CheckForNewRequest(SourceRequest sourceRequest, out string requestId,
      out FPRequestType requestType, out Int64 personId, out Int64 userPersonId, out Int64 adminPersonId,
      out string message1, out string message2, out string message3, out int timeoutSecs, out string errorMessage)
    {
      return FPC_CheckForNewRequest_Impl.Execute(_log, sourceRequest, out requestId,
      out requestType, out personId, out userPersonId, out adminPersonId,
      out message1, out message2, out message3, out timeoutSecs, out errorMessage);
    }


    /// <summary>
    /// Loan management system- check for status of request
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="requestId"></param>
    /// <param name="status"></param>
    /// <param name="errorMessage"></param>    
    /// <returns></returns>
    public int LMS_CheckRequestStatus(SourceRequest sourceRequest, string requestId,
      out FPRequestStatus status, out BasicPersonDetailsDTO person,
      out string errorMessage)
    {
      return LMS_CheckRequestStatus_Impl.Execute(_log, sourceRequest, requestId, out status, out person, out errorMessage);
    }


    /// <summary>
    /// Upload hardware/software client information - core hardware (disk size/space, CPU), other hardware (webcams, fingerprint), 
    /// core software (Windows info, MS office licenses), 
    /// fingerprint device status, etc.
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="machineInfo">Deflated JSON</param>
    /// <param name="errorMessage">Any error information</param>
    /// <returns>Enumerators.General.WCFCallResult</returns>
    public void FPC_UploadClientHWSWStatus(SourceRequest sourceRequest, byte[] machineInfo, out string errorMessage)
    {
      FPC_UploadClientHWSWStatus_Impl.Execute(_log, sourceRequest, machineInfo, out errorMessage);
    }


    /// <summary>
    /// Upload fingerprint unit status
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="fpInfo"></param>
    /// <param name="errorMessage"></param>
    public void FPC_UploadClientFPStatus(SourceRequest sourceRequest, byte[] fpInfo, out string errorMessage)
    {
      FPC_UploadClientFPStatus_Impl.Execute(_log, _cache, sourceRequest, fpInfo, out errorMessage);
    }


    /// <summary>
    /// Gets the current server date/time
    /// </summary>
    /// <returns>The current server date/time</returns>
    public DateTime GetServerDateTime()
    {
      return DateTime.Now;
    }


    public RecoveryInfoDTO FPC_CheckForOpenSession(SourceRequest sourceRequest)
    {
      return FPC_CheckForOpenSession_Impl.Execute(_log, sourceRequest);
    }


    #region Private vars

    // Logging
    private readonly ILogging _log;
    private readonly ICacheServer _cache;

    #endregion

  }
}
