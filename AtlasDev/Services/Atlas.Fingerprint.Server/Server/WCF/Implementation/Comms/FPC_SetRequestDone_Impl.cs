using System;
using System.Threading.Tasks;

using Atlas.MongoDB.Entities;
using Atlas.Enumerators;
using Atlas.WCF.FPServer.Common;
using Atlas.WCF.FPServer.MongoDB;
using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.ClientState;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Comms
{
  class FPC_SetRequestDone_Impl
  {
    public static int Execute(ILogging log, SourceRequest sourceRequest, string requestId, FPRequestStatus status, byte[] wsq,
      Int64 personId, Int64 userPersonId, Int64 adminPersonId, int fingerId,
      out string errorMessage)
    {
      var methodName = "FPC_SetRequestDone";
      //log.Information("{MethodName} starting: {@Request}", methodName, new { sourceRequest, requestId, status, personId, userPersonId, adminPersonId });

      errorMessage = string.Empty;
      try
      {
        #region check parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }

        if (string.IsNullOrEmpty(requestId))
        {
          errorMessage = "requestId missing";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }

        if (status == FPRequestStatus.NotSet)
        {
          errorMessage = "status missing";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }
        #endregion

        var request = LMSGuiState.GetGUICommStatus(machine.Id, requestId);
        if (request != null)
        {
          switch (request.Status)
          {
            case Biometric.RequestStatus.EnrollmentPending:
            case Biometric.RequestStatus.EnrollmentRequested:
              switch (status)
              {
                case FPRequestStatus.Successful:
                  LMSGuiState.LmsGuiSetCommStatus(machine.Id, requestId, Biometric.RequestStatus.EnrollmentSuccessful, personId, userPersonId, adminPersonId);
                  break;

                case FPRequestStatus.Failed:
                  LMSGuiState.LmsGuiSetCommStatus(machine.Id, requestId, Biometric.RequestStatus.EnrollmentFailed, personId, userPersonId, adminPersonId);
                  break;

                case FPRequestStatus.Cancelled:
                  LMSGuiState.LmsGuiSetCommStatus(machine.Id, requestId, Biometric.RequestStatus.EnrollmentCancelled, personId, userPersonId, adminPersonId);
                  break;
              }
              break;

            case Biometric.RequestStatus.VerifyPending:
            case Biometric.RequestStatus.VerifyRequested:
              switch (status)
              {
                case FPRequestStatus.Successful:
                  LMSGuiState.LmsGuiSetCommStatus(machine.Id, requestId, Biometric.RequestStatus.VerifySuccessful, personId, userPersonId, adminPersonId, wsq);
                  break;

                case FPRequestStatus.Failed:
                  LMSGuiState.LmsGuiSetCommStatus(machine.Id, requestId, Biometric.RequestStatus.VerifyFailed, personId, userPersonId, adminPersonId, wsq);
                  break;

                case FPRequestStatus.Cancelled:
                  LMSGuiState.LmsGuiSetCommStatus(machine.Id, requestId, Biometric.RequestStatus.VerifyCancelled, personId, userPersonId, adminPersonId, wsq);
                  break;
              }

              #region Save the wsq image to MongoDB
              if (wsq != null)
              {
                Task.Run(async () =>
                  {
                    var newBitmap = new FPCaptured()
                    {
                      CreatedDT = sourceRequest.MachineDateTime,
                      PersonId = personId,
                      FingerId = fingerId,
                      MachineId = machine.HardwareKey,
                      Width = 288,
                      Height = 352,
                      Bitmap = wsq,
                      SessionId = requestId,
                      CreatedPersonId = userPersonId
                    };

                    var client = MongoDbUtils.GetMongoClient();
                    var database = client.GetDatabase("fingerprint");
                    var mongoBitmaps = database.GetCollection<FPCaptured>("fpCaptured");
                    await mongoBitmaps.InsertOneAsync(newBitmap);
                  }).Wait();
              }
              #endregion

              break;

            case Biometric.RequestStatus.IdentificationPending:
            case Biometric.RequestStatus.IdentificationRequested:
              switch (status)
              {
                case FPRequestStatus.Successful:
                  LMSGuiState.LmsGuiSetCommStatus(machine.Id, requestId, Biometric.RequestStatus.IdentificationSuccessful, personId, userPersonId, adminPersonId, wsq);
                  break;

                case FPRequestStatus.Failed:
                  LMSGuiState.LmsGuiSetCommStatus(machine.Id, requestId, Biometric.RequestStatus.IdentificationFailed, personId, userPersonId, adminPersonId, wsq);
                  break;

                case FPRequestStatus.Cancelled:
                  LMSGuiState.LmsGuiSetCommStatus(machine.Id, requestId, Biometric.RequestStatus.IdentificationCancelled, personId, userPersonId, adminPersonId, wsq);
                  break;
              }

              break;
          }

          //log.Information("{MethodName} completed successfully", methodName);
          return (int)Enumerators.General.WCFCallResult.OK;
        }
        else
        {
          errorMessage = string.Format("Unable to find request with ID {0}", requestId);
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = FPActivation.SERVER_ERR_UNEXPECTED;
        return (int)Enumerators.General.WCFCallResult.ServerError;
      }
    }
  }
}
