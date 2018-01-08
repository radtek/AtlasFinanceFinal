/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Creates WCF requests: SourceRequest
 *     
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-02- Created
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;

using Atlas.WCF.FPServer.Security.Interface;
using Atlas.Client.SysInfo;
using Atlas.Client.Windows;


namespace Atlas.Fingerprint.WCF.Client
{
  public static class SourceRequestUtils
  {
    /// <summary>
    /// Creates necessary Source Request, with requisite fields
    /// </summary>
    /// <returns>New, populated WSFPAdmin.SourceRequest for use with server</returns>
    public static SourceRequest CreateSourceRequest(Int64 personId = 0, Int64 userPersonId = 0, Int64 adminPersonId = 0, string legacyBranchCode = null)
    {
      return new SourceRequest()
        {
          AppName = SysInfoUtils.GetThisSoftwareDesc(),
          AppVer = SysInfoUtils.GetThisSoftwareVer(),
          MachineIPAddresses = SysInfoUtils.GetLocalIPAddresses(),
          MachineName = SysInfoUtils.GetLocalMachineName().ToUpper(),
          MachineUniqueID = SysInfoUtils.MD5Hash(string.Format("{0}{1}", WindowsAPI.GetCDriveSerial(), SysInfoUtils.GetLocalMachineName())),
          PersonId = personId,
          UserPersonId = userPersonId,
          AdminPersonId = adminPersonId,
          MachineDateTime = DateTime.Now,
          BranchCode = legacyBranchCode
        };       
    }
    
  }
}