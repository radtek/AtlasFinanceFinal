using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.Common;
using Atlas.WCF.FPServer.Comms;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Comms
{
  public static class GetPersonEnrolledFingers_Impl
  {
    public static int Execute(ILogging log, SourceRequest sourceRequest, Int64 personId,
      out List<int> enrolledFingers, out string errorMessage)
    {
      var methodName = "GetPersonEnrolledFingers";
      //log.Information("{MethodName} starting: {@Request}, {PersonId}", methodName, sourceRequest, personId);

      enrolledFingers = new List<int>();
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

      if (personId <= 0)
      {
        errorMessage = string.Format("Invalid personId parameter: {0}", personId);
        log.Warning(new Exception(errorMessage), methodName);
        return (int)Enumerators.General.WCFCallResult.BadParams;
      }

      using (var unitOfWork = new UnitOfWork())
      {
        if (!unitOfWork.Query<PER_Person>().Any(s => s.PersonId == personId))
        {
          errorMessage = "PersonId does not exist";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }
      }
      #endregion

      try
      {
        var enrolledInDB = DistCommUtils.GetFingersEnrolled(personId);
        enrolledFingers = enrolledInDB != null ? enrolledInDB.ToList() : new List<int>();
        //log.Information("{MethodName} completed successfully with {@EnrolledFingers}", methodName, enrolledFingers);
        return (int)Enumerators.General.WCFCallResult.OK;
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
