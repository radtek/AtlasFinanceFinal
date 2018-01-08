using System;
using System.Collections.Generic;

using Serilog;

using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;


namespace Atlas.Server.Training
{
  internal class FPComms : IFPComms
  {
    public int LMS_MachineToUseFP(SourceRequest sourceRequest, out bool isEnabled, out string errorMessage)
    {
      _log.Information("{MethodName}- {@SourceRequest}", "LMS_MachineToUseFP", sourceRequest);
      errorMessage = null;
      isEnabled = false;
      return 1;
    }


    public int LMS_MachineToUseFPForClients(SourceRequest sourceRequest, out bool isEnabled, out string errorMessage)
    {
      _log.Information("{MethodName}- {SourceRequest}", "LMS_MachineToUseFPForClients", sourceRequest);
      errorMessage = null;
      isEnabled = false;
      return 1;
    }


    public int LMS_IsMachineFPReady(SourceRequest sourceRequest, out bool isReady, out string errorMessage)
    {
      _log.Information("{MethodName}- {SourceRequest}", "LMS_IsMachineFPReady", sourceRequest);
      errorMessage = null;
      isReady = false;
      return 1;
    }


    public int CreatePerson(SourceRequest sourceRequest, BasicPersonDetailsDTO personDetails, out long personId, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public int GetPersonViaIdOrPassport(SourceRequest sourceRequest, string idOrPassport, out BasicPersonDetailsDTO personDetails, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public int GetPersonViaOperatorId(SourceRequest sourceRequest, string personOperatorId, out BasicPersonDetailsDTO personDetails, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public int GetPersonEnrolledFingers(SourceRequest sourceRequest, long personId, out List<int> enrolledFingers, out string errorMessage)
    {
      throw new NotImplementedException();
    }

    public int GetPersonViaPersonId(SourceRequest sourceRequest, long personId, out BasicPersonDetailsDTO personDetails, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public int FPC_AddRequest(SourceRequest sourceRequest, FPRequestType requestType, string trackingId, long personId, long userPersonId, long adminPersonId, string message1, string message2, string message3, int timeoutSecs)
    {
      throw new NotImplementedException();
    }

    public void FPC_UploadClientHWSWStatus(SourceRequest sourceRequest, byte[] machineInfo, out string errorMessage)
    {
      throw new NotImplementedException();
    }

    public void FPC_UploadClientFPStatus(SourceRequest sourceRequest, byte[] fpInfo, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public DateTime GetServerDateTime()
    {
      throw new NotImplementedException();
    }


    public RecoveryInfoDTO FPC_CheckForOpenSession(SourceRequest sourceRequest)
    {
      throw new NotImplementedException();
    }


    public int LMS_AddRequest(SourceRequest sourceRequest, FPRequestType requestType, long personId, long userPersonId, long adminPersonId, string message1, string message2, string message3, int timeoutSecs, out string requestId, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public int FPC_SetRequestDone(SourceRequest sourceRequest, string requestId, FPRequestStatus status, byte[] wsq, long personId, long userPersonId, long adminPersonId, int fingerId, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public int LMS_CheckRequestStatus(SourceRequest sourceRequest, string requestId, out FPRequestStatus status, out BasicPersonDetailsDTO person, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public int LMS_UserCancelled(SourceRequest sourceRequest, string requestId, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<FPComms>();

    #endregion
    
  }

}
