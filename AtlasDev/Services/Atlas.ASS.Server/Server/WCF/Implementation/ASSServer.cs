/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Provide functions specific to ASS
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-09-18- Created
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.ServiceModel;

using AtlasServer.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Server.WCF.Implementation.ASS;


namespace AtlasServer.WCF.Implementation
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class ASSServer : IASSServer
  {
    public ASSServer(ILogging log, IConfigSettings config, ICacheServer cache)
    {
      _log = log;
      _config = config;
      _cache = cache;
    }


    /// <summary>
    /// Returns the SQLRDD connection information to use
    /// </summary>     
    public int GetDBSettings(string machineIP, string machineName, string machineFP,
      string env_username, string env_topware, string env_branch, string env_live,
      out string branchNum, out string rddType, out string connectionString, out string schemaName,
      out string errorMessage)
    {
      return GetDBSettings_Impl.Execute(_log, _config, _cache, machineIP, machineName, machineFP, env_branch, env_live,
        out branchNum, out rddType, out connectionString, out schemaName, out errorMessage);
    }


    /// <summary>
    /// Get application settings for xHarbour
    /// </summary>
    /// <param name="branchNum"></param>
    /// <param name="terminalNum"></param>
    /// <param name="settings"></param>
    /// <param name="errorMessage"></param>
    /// <returns>Serialized string for xHarbour use &stringVal macro in xHarbour to de-serialize 
    /// to a xHarbour 'Hash' object, or PrgExpToVal()</returns>
    public int GetAppSettings(string branchNum, string terminalNum,
      out string settings, out string errorMessage)
    {
      return GetAppSettings_Impl.Execute(_log, _config, _cache, branchNum, out settings, out errorMessage);
    }


    public int AEDOPCDFile(string branchNum, DateTime startDate, DateTime endDate,
      out string file, out string errorMessage)
    {
      return AEDOPCDFile_Impl.Execute(_log, _config, _cache, branchNum, startDate, endDate, out file, out errorMessage);
    }


    public int NAEDOPCDFile(string branchNum, DateTime startDate, DateTime endDate,
      out string file, out string errorMessage)
    {
      return NAEDOPCDFile_Impl.Execute(_log, branchNum, startDate, endDate, out file, out errorMessage);
    }


    public DateTime GetServerTime()
    {
      return DateTime.Now;
    }


    public int CheckSoftware(SourceRequest sourceRequest, out string serverAppVer, out string errorMessage)
    {
      return CheckSoftware_Impl.Execute(_log, _cache, sourceRequest, out serverAppVer, out errorMessage);
    }


    public int GetBranchServerIP(SourceRequest sourceRequest, out string serverIP, out string errorMessage)
    {
      return GetBranchServerIP_Impl.Execute(_log, _config, _cache, sourceRequest, out serverIP, out errorMessage);
    }


    public int SendSMS(SourceRequest sourceRequest, string[] cellNumbers, string[] messages, out string errorMessage)
    {
      return SendSMS_Impl.Execute(_log, sourceRequest, cellNumbers, messages, out errorMessage);
    }


    public int SendOTP(SourceRequest sourceRequest, string cellNumber, string otpPrefixText, string otpSuffixText, out string otpReference, out string errorMessage)
    {
      return SendOTP_Impl.Execute(_log, sourceRequest, cellNumber, otpPrefixText, otpSuffixText, out otpReference, out errorMessage);
    }


    public int VerifyOTP(SourceRequest sourceRequest, string otpReference, int otpEntered, out string errorMessage)
    {
      return VerifyOTP_Impl.Execute(_log, sourceRequest, otpReference, otpEntered, out errorMessage);
    }


    public int GetLastClientTransaction(SourceRequest sourceRequest, string idOrPassport, out string lastBranchNum, out DateTime lastTransactionDate, out string errorMessage)
    {
      return GetLastClientTransaction_Impl.Execute(_log, _config, sourceRequest, idOrPassport, out lastBranchNum, out lastTransactionDate, out errorMessage);
    }
    
    #region Private vars
     
    private readonly ILogging _log;
    private readonly ICacheServer _cache;
    private readonly IConfigSettings _config;

    #endregion

  }
}