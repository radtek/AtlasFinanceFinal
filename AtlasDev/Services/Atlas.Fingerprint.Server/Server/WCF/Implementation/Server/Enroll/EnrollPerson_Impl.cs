using System;
using System.Linq;

using Atlas.Enumerators;
using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.Common;
using Atlas.Common.Interface;

namespace Atlas.WCF.FPServer.WCF.Implementation.Server.Enroll
{
  public static class EnrollPerson_Impl
  {
    public static int EnrollPerson(ILogging log, SourceRequest sourceRequest, FPScannerInfoDTO scanner, FPScannerOptionDTO scannerOptions, FPRawBufferDTO[] fpBitmaps,
      bool isStaff,
      out string errorMessage)
    {
      errorMessage = string.Empty;
      var methodName = "EnrollPerson";

      try
      {
        log.Information("{MethodName} starting: {@Request}", methodName, new { sourceRequest, scannerOptions });

        #region Check request parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }

        if (fpBitmaps.Length < 6) // 3 scans per finger
        {
          errorMessage = "At least two fingers must be scanned!";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }

        var personId = fpBitmaps[0].PersonId;
        #endregion

        #region Start enolment
        Int64 startEnrollRef;
        var startEnroll = StartEnrollPerson_Impl.StartEnrollPerson(log, sourceRequest, scanner, scannerOptions, personId, out startEnrollRef, out errorMessage);
        if (startEnroll != (int)General.WCFCallResult.OK)
        {
          return startEnroll;
        }
        #endregion

        #region Enroll each finger
        for (var finger = 1; finger <= 10; finger++)
        {
          if (fpBitmaps.Any(s => s.FingerId == finger))
          {
            var fingers = fpBitmaps.Where(s => s.FingerId == finger).ToList();
            var enrollFinger = EnrollFingerprint_Impl.EnrollFingerprint(log, sourceRequest, startEnrollRef, fingers, out errorMessage);
            if (enrollFinger != (int)General.WCFCallResult.OK)
            {
              // Cancel the enrolment
              string error;
              CancelEnrollPerson_Impl.CancelEnrollPerson(log, sourceRequest, startEnrollRef, out error);

              return enrollFinger;
            }
          }
        }
        #endregion

        #region End enrolment
        var finalEnroll = EndEnrollPerson_Impl.EndEnrollPerson(log, sourceRequest, startEnrollRef, out errorMessage);
        if (finalEnroll != (int)General.WCFCallResult.OK)
        {
          // Cancel the enrolment
          string error;
          CancelEnrollPerson_Impl.CancelEnrollPerson(log, sourceRequest, startEnrollRef, out error);

          return finalEnroll;
        }
        #endregion

        log.Information("{MethodName} completed successfully", methodName);
        return (int)General.WCFCallResult.OK;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = FPActivation.SERVER_ERR_UNEXPECTED;
        return (int)General.WCFCallResult.ServerError;
      }
    }

  }
}
