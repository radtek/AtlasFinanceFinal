/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Altech NAEDO interface 
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


namespace AtlasServer.WCF.Interface
{
  // TODO: ASS requires the defaut... have to sync with ASS...
  //[ServiceContract(Namespace = "Atlas.ASS.Utils")] 
  [ServiceContract]
  public interface IASSServer
  {
    [OperationContract]
    int GetDBSettings(string machineIP, string machineName, string machineFP,
      string env_username, string env_topware, string env_branch, string env_live,
      out string branchNum, out string rddType, out string connectionString, out string schemaName,
      out string errorMessage);

    [OperationContract]
    int GetAppSettings(string branchNum, string terminalNum,
      out string settings, out string errorMessage);

    [OperationContract]
    int AEDOPCDFile(string branchNum, DateTime startDate, DateTime endDate,
      out string file, out string errorMessage);

    [OperationContract]
    int NAEDOPCDFile(string branchNum, DateTime startDate, DateTime endDate,
      out string file, out string errorMessage);

    [OperationContract]
    DateTime GetServerTime();

    [OperationContract]
    int CheckSoftware(SourceRequest sourceRequest,
      out string serverAppVer, out string errorMessage);

    [OperationContract]
    int GetBranchServerIP(SourceRequest sourceRequest,
      out string serverIP, out string errorMessage);

    [OperationContract]
    int SendSMS(SourceRequest sourceRequest, string[] cellNumbers, string[] messages, out string errorMessage);

    [OperationContract]
    int SendOTP(SourceRequest sourceRequest, string cellNumber, string otpPrefixText, string otpSuffixText, out string otpReference, out string errorMessage);

    [OperationContract]
    int VerifyOTP(SourceRequest sourceRequest, string otpReference, int otpEntered, out string errorMessage);

    [OperationContract]
    int GetLastClientTransaction(SourceRequest sourceRequest, string idOrPassport, out string lastBranchNum, out DateTime lastTransactionDate, out string errorMessage);

  }
}
