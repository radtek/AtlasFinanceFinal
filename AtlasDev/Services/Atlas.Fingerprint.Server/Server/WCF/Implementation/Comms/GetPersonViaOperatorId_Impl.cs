using System;

using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.WCF.Implementation.Comms.Utils;
using Atlas.WCF.FPServer.Common;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Comms
{  public static class GetPersonViaOperatorId_Impl
  {
  public static int Execute(ILogging log, SourceRequest sourceRequest, string personOperatorId,
    out BasicPersonDetailsDTO personDetails, out string errorMessage)
  {
    var methodName = "GetPersonViaOperatorId";
    log.Information("{MethodName} starting: {@Request}, {PersonOperatorId}", methodName, sourceRequest, personOperatorId);

    personDetails = null;
    errorMessage = string.Empty;

    #region Check request parameters
    Machine machine;
    User user;
    Int64 branchId;
    if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
    {
      log.Warning(new Exception(errorMessage), methodName);
      return (int)Enumerators.General.WCFCallResult.BadParams;
    }

    if (string.IsNullOrEmpty(personOperatorId) || personOperatorId.Length < 4)
    {
      errorMessage = "Parameter operatorId cannot be blank";
      log.Warning(new Exception(errorMessage), methodName);
      return (int)Enumerators.General.WCFCallResult.BadParams;
    }
    #endregion

    try
    {
      if (!WCFCommsUtils.PersonToBasicPersonDetailsDTO(WCFCommsUtils.FindPersonByField.ByLegacyOperatorId, personOperatorId, out personDetails))
      {
        errorMessage = string.Format("Unable to locate operator: '{0}'", personOperatorId);
        log.Error(new Exception(errorMessage), methodName);
        personDetails = null;
        return (int)Enumerators.General.WCFCallResult.BadParams;
      }

      log.Information("{MethodName}] completed successfully with result: {@PersonDetails}", methodName, personDetails);
      return (int)Enumerators.General.WCFCallResult.OK;
    }
    catch (Exception err)
    {
      log.Error(err, methodName);
      errorMessage = FPActivation.SERVER_ERR_UNEXPECTED;
      personDetails = null;
      return (int)Enumerators.General.WCFCallResult.ServerError;
    }
  }
  }
}
