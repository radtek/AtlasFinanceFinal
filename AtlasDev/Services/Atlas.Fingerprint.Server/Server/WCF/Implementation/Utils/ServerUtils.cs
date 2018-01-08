using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.DTO;
using Atlas.Enumerators;
using Atlas.Domain.Model;


namespace Atlas.WCF.FPServer.WCF.Implementation.Utils
{
  public static class ServerUtils
  {
    /// <summary>
    /// Get first matching setting, going from most specific (person) to most general (company)
    /// </summary>
    /// <param name="setting">Setting required</param>
    /// <param name="personSettings">List of person's settings</param>
    /// <param name="personTypeSettings">List of person type settings</param>
    /// <param name="companySettings">List of company-wide settings</param>
    /// <returns>String with setting value, null if setting could not be found</returns>   
    public static string GetFPConfigSetting(Biometric.SettingType setting,
      General.PersonType entityId,
      List<BIO_SettingDTO> personSettings, List<BIO_SettingDTO> personTypeSettings, List<BIO_SettingDTO> companySettings)
    {
      BIO_SettingDTO settingVal = null;
      // Go from very specific to less specific and use first non-null

      #region Person specific
      if (personSettings != null && personSettings.Count > 0)
      {
        settingVal = personSettings.FirstOrDefault(s => s.SettingType == setting);
      }
      #endregion

      if (settingVal == null)
      {
        // Get setting for this person type
        settingVal = personTypeSettings.FirstOrDefault(s => s.SettingType == setting && s.Entity == (int)entityId);
      }

      if (settingVal == null)
      {
        settingVal = companySettings.FirstOrDefault(s => s.SettingType == setting && s.Entity == (int)entityId);
      }
      if (settingVal == null)
      {
        settingVal = companySettings.FirstOrDefault(s => s.SettingType == setting);
      }

      return (settingVal != null) ? settingVal.Value : null;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="userPersonId"></param>
    /// <param name="role"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public static bool CheckUserHasRole(Int64 userPersonId, General.RoleType role, out string errorMessage)
    {
      errorMessage = null;
      if (userPersonId == 0)
      {
        errorMessage = "userPersonId cannot be blank!";
        return false;
      }

      using (var unitofWork = new UnitOfWork())
      {
        var userPerson = unitofWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == userPersonId);
        if (userPerson == null)
        {
          errorMessage = "userPersonId unknown!";
          return false;
        }

        if (userPerson.PersonType.Type != General.PersonType.Employee || userPerson.Security == null)
        {
          errorMessage = "Not a system user!";
          return false;
        }

        if (!unitofWork.Query<PER_Role>().Any(s => s.Person == userPerson && s.RoleType.RoleTypeId == (int)role))
        {
          errorMessage = "User does not have right: " + role.ToString();
          return false;
        }

        return true;
      }

    }

  }
}
