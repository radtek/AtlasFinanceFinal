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

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Net;

using Atlas.NuCard.WCF.Interface;

#endregion


namespace Atlas.NuCard.WCF.Client
{
  /// <summary>
  /// Static routines to create SourceRequest's for WCF calls
  /// </summary>
  public static class SourceRequestUtils
  {
     public static SourceRequest CreateRequest(string branchNum, string userIDOrPassport)
    {
      return new SourceRequest()
       {
         AppName = GetThisSoftwareDesc(),
         AppVer = GetThisSoftwareVer(),
         MachineIPAddresses = GetLocalIPAddresses(),
         MachineName = GetLocalMachineName().ToUpper(), // 
         MachineUniqueID = MD5Hash(string.Format("{0}{1}", GetCDriveSerial(), GetLocalMachineName())),
         BranchCode = branchNum,         
         MachineDateTime = DateTime.Now,
         UserIDOrPassport = userIDOrPassport 
       };
    }


    #region Private methods

    /// <summary>
    /// Returns name of local machine
    /// </summary>
    /// <returns>Name of local machine</returns>
    private static string GetLocalMachineName()
    {
      return Environment.MachineName
              ?? Dns.GetHostName()
              ?? Environment.GetEnvironmentVariable("COMPUTERNAME");
    }


    /// <summary>
    /// Get comma-delimited, sorted IP address list for local machine
    /// </summary>
    /// <returns>String of comma-delimited, sorted IP addresses for the local machine</returns>
    private static string GetLocalIPAddresses()
    {
      // Use MachineName?
      var hostName = Dns.GetHostName().Trim().ToUpper();
      var ipHostEntry = Dns.GetHostEntry(hostName);
      if (ipHostEntry.AddressList.Length == 0)
      {
        ipHostEntry = Dns.GetHostEntry(IPAddress.Parse("127.0.0.1"));
      }
      // Get IPv4 addresses as CSV
      return string.Join(",", ipHostEntry.AddressList.Where(s => s.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        .Select(s => s.ToString())
        .Distinct()
        .OrderBy(s => s).ToList());
    }


    /// <summary>
    /// Returns the version number of main assembly
    /// </summary>
    /// <returns>String with file version</returns>
    private static string GetThisSoftwareVer()
    {
      return System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location).FileVersion;
    }


    /// <summary>
    /// Gets file description of main assembly
    /// </summary>
    /// <returns>String with file description</returns>
    private static string GetThisSoftwareDesc()
    {
      return System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location).FileDescription;
    }


    /// <summary>
    /// Gets the C drive serial number
    /// </summary>
    /// <returns>C drive serial number</returns>
    private static string GetCDriveSerial()
    {
      var searcher = new ManagementObject("Win32_Logicaldisk=\"C:\"");
      searcher.Get();
      return searcher["VolumeSerialNumber"].ToString().Trim();
    }


    /// <summary>
    /// MD5 hash a string and then hex the hash
    /// </summary>
    /// <param name="data">String to be hashed</param>
    /// <returns>Hex string containing MD5 hash of 'data'</returns>
    private static string MD5Hash(string data)
    {
      var result = new StringBuilder();
      using (var md5 = System.Security.Cryptography.MD5.Create())
      {
        // Convert the input string to a byte array and compute the hash. 
        byte[] inputBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
        for (int i = 0; i < inputBytes.Length; i++)
        {
          result.Append(inputBytes[i].ToString("x2"));
        }
      }

      return result.ToString();
    }

    #endregion

  }
}