/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Task to disable mUSB memory sticls
 *     
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2015-12 Created
 *     
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Principal;
using System.Security.AccessControl;
using Microsoft.Win32;

using Quartz;
using Serilog;


namespace AClientSvc.QuartzTasks
{
  internal class USBStickProtect: IJob
  {
    public void Execute(IJobExecutionContext context)
    {
      try
      {
        var machineName = Environment.MachineName
                 ?? Dns.GetHostName()
                 ?? Environment.GetEnvironmentVariable("COMPUTERNAME");

        if (Regex.IsMatch(machineName, @"^[0-9,A-Z]{2,3}\-00\-[0-9]{2}$"))
        {
          // Deny everybody access to file Usbstor.pnf/Usbstor.inf
          var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Inf", "Usbstor.pnf");
          RemoveRights(file);
          file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Inf", "Usbstor.inf");
          RemoveRights(file);

          // Set usb storage service to 'disabled'
          var key = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Services\USBSTOR", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
          var x = key.GetValue("Start");
          if (x != null && (int)x != 4)
          {
            key.SetValue("Start", 4);
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "USBStick");
      }
    }


    private static void RemoveRights(string fileName)
    {
      try
      {
        if (File.Exists(fileName))
        {
          // SYSTEM account
          var sid = new SecurityIdentifier("S-1-5-18");
          var acct = (NTAccount)sid.Translate(typeof(NTAccount));
          SetFileDenyAccess(fileName, acct.Value);          
        }
      }
      catch  {  }
    }


    private static void SetFileDenyAccess(String filePath, string identity)
    {
      try
      {
        var fi = new FileInfo(filePath); // We get exception here, because we are system and have no rights anymore....

        //get security access
        var fs = fi.GetAccessControl();

        //remove any inherited access
        fs.SetAccessRuleProtection(true, false);

        //get any special user access
        var rules = fs.GetAccessRules(true, true, typeof(NTAccount));

        //remove any special access
        foreach (FileSystemAccessRule rule in rules)
          fs.RemoveAccessRule(rule);

        // deny user
        fs.AddAccessRule(new FileSystemAccessRule(identity, FileSystemRights.ReadData, AccessControlType.Deny));

        //deny others
        fs.AddAccessRule(new FileSystemAccessRule("Authenticated Users", FileSystemRights.ReadData, AccessControlType.Deny));

        //flush security access.
        File.SetAccessControl(filePath, fs);
      }
      catch { }
    }
    

    #region Private vars

    // Logging
    private static readonly ILogger _log = Log.ForContext<USBStickProtect>();

    #endregion

  }
}
