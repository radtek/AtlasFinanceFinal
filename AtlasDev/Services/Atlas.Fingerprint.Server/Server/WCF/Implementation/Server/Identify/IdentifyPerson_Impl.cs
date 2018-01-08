using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Enumerators;
using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.Domain.Model;
using Atlas.WCF.FPServer.Common;
using Atlas.WCF.FPServer.Comms;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Server.Identify
{
  public static class IdentifyPerson_Impl
  {
    public static int IdentifyPerson(ILogging log, SourceRequest sourceRequest, FPScannerInfoDTO scanner,
      FPScannerOptionDTO scannerOptions, FPRawBufferDTO[] compressedImages,
      out BasicPersonDetailsDTO person, out string errorMessage)
    {
      var methodName = "IdentifyPerson";
      errorMessage = string.Empty;
      person = new BasicPersonDetailsDTO();

      try
      {
        log.Information("{MethodName} starting: {@Request}, {@Scanner}, {@ScannerOptions}", methodName, sourceRequest, scanner, scannerOptions);

        #region Check request parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          return (int)General.WCFCallResult.BadParams;
        }

        if (compressedImages == null || compressedImages.Length == 0)
        {
          errorMessage = "compressedImages cannot be empty";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        var personId = -1L;
        if (!DistCommUtils.IdentifyFingerprint(compressedImages.Select(s => s.RawBuffer).ToList(), 5, out personId))
        {
          errorMessage = "Identify failed";
          return (int)General.WCFCallResult.ServerError;
        }

        if (personId > 0)
        {
          using (var unitOfWork = new UnitOfWork())
          {
            person = AutoMapper.Mapper.Map<BasicPersonDetailsDTO>(unitOfWork.Query<PER_Person>().First(s => s.PersonId == personId));
          }
        }

        log.Information("{MethodName} completed successfully: {@Person}", methodName, person);

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
