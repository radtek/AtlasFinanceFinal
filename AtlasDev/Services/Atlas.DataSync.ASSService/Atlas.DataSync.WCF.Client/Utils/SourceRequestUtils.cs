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
 *     2013-09-20   Created
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;

using Atlas.DataSync.WCF.Interface;
using Atlas.Client.SysInfo;
using Atlas.Client.Windows;


namespace Atlas.DataSync.WCF.Client.Utils
{
  /// <summary>
  /// Static routines to create SourceRequest's for WCF calls
  /// </summary>
  public static class SourceRequestUtils
  {
     public static SourceRequest CreateRequest(string branchNum, long userPersonId)
    {
      return new SourceRequest()
       {
         AppName = SysInfoUtils.GetThisSoftwareDesc(),
         AppVer = SysInfoUtils.GetThisSoftwareVer(),
         MachineIPAddresses = SysInfoUtils.GetLocalIPAddresses(),
         MachineName = SysInfoUtils.GetLocalMachineName().ToUpper(), // 
         MachineUniqueID = SysInfoUtils.MD5Hash(string.Format("{0}{1}", WindowsAPI.GetCDriveSerial(), SysInfoUtils.GetLocalMachineName())),
         BranchCode = branchNum,         
         MachineDateTime = DateTime.Now,         
         PersonId = userPersonId         
       };
    }


    #region Private methods

    
    #endregion

  }
}