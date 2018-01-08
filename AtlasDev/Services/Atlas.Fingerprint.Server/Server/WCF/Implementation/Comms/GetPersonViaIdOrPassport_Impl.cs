using System;

using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.WCF.Implementation.Comms.Utils;
using Atlas.WCF.FPServer.Common;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Comms
{
  public static class GetPersonViaIdOrPassport_Impl
  {
    public static int Execute(ILogging log, SourceRequest sourceRequest, string idOrPassport,
      out BasicPersonDetailsDTO personDetails, out string errorMessage)
    {
      var methodName = "GetPersonViaIdOrPassport";
      log.Information("{MethodName} starting: {@Request}, {IdOrPassport}", methodName, sourceRequest, idOrPassport);
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

      if (string.IsNullOrEmpty(idOrPassport))
      {
        errorMessage = "idOrPassport parameter empty";
        log.Warning(new Exception(errorMessage), methodName);
        personDetails = null;
        return (int)Enumerators.General.WCFCallResult.BadParams;
      }

      if (idOrPassport.Length < 5)
      {
        errorMessage = "idOrPassport parameter is too small- passport or ID should be at least 5 characters. ";
        log.Warning(new Exception(errorMessage), methodName);
        personDetails = null;
        return (int)Enumerators.General.WCFCallResult.BadParams;
      }
      #endregion

      try
      {
        if (!WCFCommsUtils.PersonToBasicPersonDetailsDTO(Atlas.WCF.FPServer.WCF.Implementation.Comms.Utils.WCFCommsUtils.FindPersonByField.ByIdOrPassport, idOrPassport, out personDetails))
        {
          errorMessage = string.Format("Unable to locate person with SA ID/Passport: {0}", idOrPassport);
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
