using System;

using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.WCF.Implementation.Comms.Utils;
using Atlas.WCF.FPServer.Common;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Comms
{
  class GetPersonViaPersonId_Impl
  {
    public static int Execute(ILogging log, SourceRequest sourceRequest, Int64 personId,
      out BasicPersonDetailsDTO personDetails, out string errorMessage)
    {
      var methodName = "GetPersonViaPersonId";
      log.Information("{MethodName} starting: {@Request}", methodName, new { sourceRequest, personId });

      personDetails = null;
      errorMessage = string.Empty;

      #region Check request parameters
      Machine machine;
      User user;
      Int64 branchId;
      if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
      {
        log.Warning(new Exception(errorMessage), methodName);
        personDetails = null;
        return (int)Enumerators.General.WCFCallResult.BadParams;
      }

      if (personId <= 0)
      {
        errorMessage = "personId parameter empty";
        log.Warning(new Exception(errorMessage), methodName);
        return (int)Enumerators.General.WCFCallResult.BadParams;
      }
      #endregion

      try
      {
        if (!WCFCommsUtils.PersonToBasicPersonDetailsDTO(Atlas.WCF.FPServer.WCF.Implementation.Comms.Utils.WCFCommsUtils.FindPersonByField.ByPersonId, personId.ToString(), out personDetails))
        {
          errorMessage = string.Format("Unable to locate person with personId: {0}", personId);
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }

        log.Information("{MethodName} completed successfully with result: {@PersonDetails}", methodName, personDetails);
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
