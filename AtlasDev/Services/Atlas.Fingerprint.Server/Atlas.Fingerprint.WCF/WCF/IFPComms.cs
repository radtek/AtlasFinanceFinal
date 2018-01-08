/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Interface for Fingerprint communications functionality between LMS and Fingerprint GUI client- services here
 *     are exposed via *HTTP* and NET.TCP, so only use for LMS functions.
 *     
 *
 * Notes:
 *     FPC_ calls are only for WinForms GUI. Kept here for manageability, as they don't fit into FPServer anyway.
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
 *     2013-11-11- Removed RabbitMQ/Protocolbuffers- way overkill for this simple requirement
 *                 Simplified the interface
 *
 * -----------------------------------------------------------------------------------------------------------------  */

#region Using

using System;
using System.ServiceModel;
using System.Collections.Generic;


using Atlas.WCF.FPServer.Security.Interface;

#endregion


namespace Atlas.WCF.FPServer.Interface
{
  // Loan system to server / local FP client GUI, message-based
  [ServiceContract(Namespace = "Atlas.FP.Comms")]
  public interface IFPComms
  {
    [OperationContract]
    int LMS_MachineToUseFP(SourceRequest sourceRequest, out bool isEnabled, out string errorMessage);

    [OperationContract]
    int LMS_MachineToUseFPForClients(SourceRequest sourceRequest, out bool isEnabled, out string errorMessage);
    
    [OperationContract]
    int LMS_IsMachineFPReady(SourceRequest sourceRequest, out bool isReady, out string errorMessage);

    [OperationContract]
    int CreatePerson(SourceRequest sourceRequest, BasicPersonDetailsDTO personDetails,
      out Int64 personId, out string errorMessage);

    [OperationContract]
    int GetPersonViaIdOrPassport(SourceRequest sourceRequest, string idOrPassport,
      out BasicPersonDetailsDTO personDetails, out string errorMessage);

    [OperationContract]
    int GetPersonViaOperatorId(SourceRequest sourceRequest, string personOperatorId,
      out BasicPersonDetailsDTO personDetails, out string errorMessage);

    [OperationContract]
    int GetPersonEnrolledFingers(SourceRequest sourceRequest, Int64 personId,
      out List<int> enrolledFingers, out string errorMessage);

    [OperationContract]
    int GetPersonViaPersonId(SourceRequest sourceRequest, Int64 personId,
      out BasicPersonDetailsDTO personDetails, out string errorMessage);


    [OperationContract]
    int FPC_AddRequest(SourceRequest sourceRequest, FPRequestType requestType,
      string trackingId, Int64 personId, Int64 userPersonId, Int64 adminPersonId,
      string message1, string message2, string message3, int timeoutSecs);

    [OperationContract]
    void FPC_UploadClientHWSWStatus(SourceRequest sourceRequest, byte[] machineInfo, out string errorMessage);

    [OperationContract]
    void FPC_UploadClientFPStatus(SourceRequest sourceRequest, byte[] fpInfo, out string errorMessage);

    [OperationContract]
    DateTime GetServerDateTime();

    [OperationContract]
    RecoveryInfoDTO FPC_CheckForOpenSession(SourceRequest sourceRequest);


    #region Request-Response

    [OperationContract]
    int LMS_AddRequest(SourceRequest sourceRequest, FPRequestType requestType,
      Int64 personId, Int64 userPersonId, Int64 adminPersonId,
      string message1, string message2, string message3, int timeoutSecs,
      out string requestId, out string errorMessage);
       
    [OperationContract]
    int FPC_SetRequestDone(SourceRequest sourceRequest, string requestId, FPRequestStatus status, byte[] wsq,
      Int64 personId, Int64 userPersonId, Int64 adminPersonId, int fingerId,
      out string errorMessage);

    [OperationContract]
    int LMS_CheckRequestStatus(SourceRequest sourceRequest, string requestId,
      out FPRequestStatus status, out BasicPersonDetailsDTO person,
      out string errorMessage);

    [OperationContract]
    int LMS_UserCancelled(SourceRequest sourceRequest, string requestId, out string errorMessage);

    #endregion

  }
}