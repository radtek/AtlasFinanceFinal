using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Falcon.Gyrkin.Library.Security.Role
{
  public static class RoleType
  {
    public const string ADMINISTRATOR = "Administrator";
   
    #region Naedo Roles
    
    public const string NAEDO_VIEW = "Naedo View";
    public const string NAEDO_REMOVE = "Naedo Remove";

    #endregion

    #region Avs Roles

    public const string AVS_VIEW = "Avs View";
    public const string AVS_ADMIN = "Avs Admin";

    #endregion

    #region Report Roles

    public const string REPORT = "Report";
    public const string CI_REPORT = "CI Report";
    public const string STREAM_REPORT = "Stream Report";
    public const string POSSIBLE_HANDOVERS = "Possible Handovers";
    

    #endregion

    #region Campaign Manager

    public const string CAMPAIGN_MANAGER = "Campaign Manager";
    public const string CAMPAIGN_MANAGER_SMS = "Campaign Manager - SMS";
    public const string CAMPAIGN_MANAGER_EMAIL = "Campaign Manager - Email";

    #endregion

    #region User Tracking
    public const string USER_TRACKING = "User Tracking";
    public const string USER_TRACKING_PIN = "User Tracking - Pin";
    public const string USER_TRACKING_PINNED = "User Tracking - Pinned";
    #endregion

    #region Stream

    public const string STREAM = "Stream Manager";
    public const string STREAM_COLLECTIONS = "Stream Manager - Collections";
    public const string STREAM_SALES = "Stream Manager - Sales";
    public const string STREAM_MANAGEMENT = "Stream Manager - Management";

    #endregion

    #region Target

    public const string TARGET = "Target Manager";

#endregion

    public static List<Tuple<string, string>> GetClaims()
    {
      List<Tuple<string, string>> claims = new List<Tuple<string, string>>();
      claims.Add(new Tuple<string, string>(ADMINISTRATOR, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(NAEDO_VIEW, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(NAEDO_REMOVE, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(AVS_VIEW, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(REPORT, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(CI_REPORT, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(STREAM_REPORT, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(POSSIBLE_HANDOVERS, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(CAMPAIGN_MANAGER, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(CAMPAIGN_MANAGER_SMS, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(CAMPAIGN_MANAGER_EMAIL, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(USER_TRACKING, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(USER_TRACKING_PIN, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(USER_TRACKING_PINNED, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(STREAM, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(STREAM_COLLECTIONS, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(STREAM_SALES, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(STREAM_MANAGEMENT, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
      claims.Add(new Tuple<string, string>(TARGET, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));

      return claims; 
    }
  }
}
