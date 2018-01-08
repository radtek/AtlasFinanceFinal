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
  public static class GetPersonScanOptions_Impl
  {
    public static int GetPersonScanOptions(ILogging log, SourceRequest sourceRequest, FPScannerInfoDTO scanner, Int64 personId,
      out FPScannerOptionDTO scannerOptions, out string errorMessage)
    {
      var methodName = "GetPersonScanOptions";
      errorMessage = null;
      scannerOptions = null;

      try
      {
       // log.Information("{MethodName} starting: {@Request}, {@Scanner}, {PersonId}", methodName, sourceRequest, scanner, personId);

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
        var personType = person != null && person.PersonType != null ? person.PersonType.Type : General.PersonType.Client;
        var minFingers = int.Parse(ServerUtils.GetFPConfigSetting(Biometric.SettingType.MinFingers, personType, personSettings, personTypeSettings, companySettings));
        var minQuality = int.Parse(ServerUtils.GetFPConfigSetting(Biometric.SettingType.MinQuality, personType, personSettings, personTypeSettings, companySettings));
        var acceptQuality = int.Parse(ServerUtils.GetFPConfigSetting(Biometric.SettingType.AcceptQuality, personType, personSettings, personTypeSettings, companySettings));
        var nfiq = int.Parse(ServerUtils.GetFPConfigSetting(Biometric.SettingType.NFIQHighest, personType, personSettings, personTypeSettings, companySettings));
              
        var detectCoreVal = ServerUtils.GetFPConfigSetting(Biometric.SettingType.DetectCore, personType, personSettings, personTypeSettings, companySettings);
        var detectCore = !string.IsNullOrEmpty(detectCoreVal) ? detectCoreVal == "T" || detectCoreVal == "Y" : true;
       
        if (minFingers == 0 || minQuality == 0 || acceptQuality == 0 || nfiq == 0)
        {
          errorMessage = "Biometric settings missing from configuration";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.ServerError;
        }
        #endregion

        scannerOptions = new FPScannerOptionDTO()
        {
          MinFingers = minFingers,
          AcceptedQualityMin = acceptQuality,
          NFIQMin = nfiq,
          MinQuality = minQuality,
          DetectCore = detectCore
        };

        //log.Information("{MethodName} completed with result: {@Result}", methodName, scannerOptions);
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