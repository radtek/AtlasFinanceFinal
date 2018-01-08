using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Domain.Model.Biometric;
using Atlas.Enumerators;
using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.WCF.Implementation.Utils;
using Atlas.WCF.FPServer.Common;
using Atlas.Common.Interface;

namespace Atlas.WCF.FPServer.WCF.Implementation.Server.Options
{
  public static class GetPersonVerifyOptions_Impl
  {
    public static int GetPersonVerifyOptions(ILogging log, SourceRequest sourceRequest, FPScannerInfoDTO scanner, Int64 personId,
      out FPVerifyOptionsDTO verifyOptions, out string errorMessage)
    {
      var methodName = "GetPersonVerifyOptions";
      errorMessage = null;
      verifyOptions = null;

      try
      {
        //log.Information("{MethodName} starting: {@Request}, {@Scanner}, {PersonId}", methodName, sourceRequest, scanner, personId);

        #region Check request parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }

        PER_PersonDTO person = null;

        if (personId > 0)
        {
          using (var unitOfWork = new UnitOfWork())
          {
            var personDb = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == personId);
            if (personDb == null)
            {
              errorMessage = "Person could not be located";
              log.Warning(new Exception(errorMessage), methodName);
              return (int)General.WCFCallResult.BadParams;
            }

            person = AutoMapper.Mapper.Map<PER_Person, PER_PersonDTO>(personDb);
          }
        }
        #endregion

        #region Get settings
        List<BIO_SettingDTO> companySettings;
        List<BIO_SettingDTO> personTypeSettings;
        List<BIO_SettingDTO> personSettings;

        using (var unitOfWork = new UnitOfWork())
        {
          var settings = unitOfWork.Query<BIO_Setting>().Where(s => s.AppliesTo == Biometric.AppliesTo.Company);
          companySettings = AutoMapper.Mapper.Map<List<BIO_SettingDTO>>(settings);

          settings = unitOfWork.Query<BIO_Setting>().Where(s => s.AppliesTo == Biometric.AppliesTo.PersonType);
          personTypeSettings = AutoMapper.Mapper.Map<List<BIO_SettingDTO>>(settings);

          settings = unitOfWork.Query<BIO_Setting>().Where(s => s.AppliesTo == Biometric.AppliesTo.PersonSpecific &&
            s.Entity == personId);
          personSettings = AutoMapper.Mapper.Map<List<BIO_SettingDTO>>(settings);
        }
        if (companySettings == null && personTypeSettings == null && companySettings.Count == 0 && personTypeSettings.Count == 0)
        {
          errorMessage = "All biometric settings missing from configuration";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.ServerError;
        }
        #endregion

        #region Get settings, prioritise on 'person specific' settings then revert to 'person type' general settings
        var personType = person.PersonType != null ? person.PersonType.Type : General.PersonType.Client;
        var minSecurityLevel = int.Parse(ServerUtils.GetFPConfigSetting(Biometric.SettingType.MinSecurityLevel, personType,
          personSettings, personTypeSettings, companySettings));

        if (minSecurityLevel == 0)
        {
          errorMessage = "Biometric settings missing from configuration";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.ServerError;
        }
        #endregion

        verifyOptions = new FPVerifyOptionsDTO()
        {
          MinSecurityLevel = minSecurityLevel
        };

        //log.Information("{MethodName} completed with result: {@Result}", methodName, verifyOptions);
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
