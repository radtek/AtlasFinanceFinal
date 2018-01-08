/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
* 
*  Description:
*  ------------------
*    Altech NuPay Terminal Cloud Control (TCC) interface 
* 
* 
*  Author:
*  ------------------
*     Keith Blows
* 
* 
*  Revision history: 
*  ------------------ 
*     2012-04-12- Skeleton created
* 
* 
* ----------------------------------------------------------------------------------------------------------------- */
using System;
using System.ServiceModel;


namespace AtlasServer.WCF.Interface
{
  //[ServiceContract(Namespace = "Atlas.ThirdParty.Altech.TCC")]
  [ServiceContract]
  public interface INPTerminalRC
  {
    // List all terminals in the branch- comma-separated
    [OperationContract]
    string FindAllTerminals(string branchNumber);

    // List all free terminals in the branch- comma-separated
    [OperationContract]
    string FindFreeTerminals(string branchNumber);

    // Gets a specific terminal's description
    [OperationContract]
    string GetTerminalDescription(int terminalID);

    // Checks whether card can be used for AEDO
    [OperationContract]
    bool UpfrontBinCheck(int terminalID, int timeoutSeconds, out string errorMessage);

    [OperationContract]
    bool AEDOTerminalAuthorise2(int terminalID,
        string contractNo, decimal installAmount, decimal contractAmount, int totInstalments, string frequency,
        DateTime startDate, string employerCode, string clientIDNumber,
        string bankShortName, string branchNumber, string accountNumber, int accountType, string panNumber,
        string clientName, // Optionals on TCC 
        int timeoutSeconds,
        out string errorMessage,
        out string responseCodeOut, out string PANNumberOut, out string transactionIDOut, out decimal contractAmountOut, out string accountNumberOut,
        out string accountTypeOut, out string contractTrackingOut, out string adjustmentRuleOut, out string frequencyOut, out string contractTypeOut);


    [OperationContract]
    bool CheckCard(int terminalID,
      string contractNo, string bankShortName, string branchNumber, string accountNumber, int accountType, string panNumber,
      string clientName, string clientIDNumber, int timeoutSeconds,
      out string errorMessage,
      out string responseCodeOut, out string PANNumberOut, out string transactionIDOut, out string accountNumberOut,
      out string accountTypeOut, out string contractTypeOut);

    // Get data from terminal
    // inputType: 1- Alphanumeric, 2- Alpha input, 3- Numeric input
    [OperationContract]
    bool CaptureData(int terminalID, int displaySeconds, int timeoutSeconds, int inputType, string prompTextline1, string prompTextline2, string prompTextline3, string prompTextline4,
      out string errorMessage, out string capturedData);


    [OperationContract]
    bool CancelContract(string contractNo, string edoType, string edoReference, out string errorMessage);


    // edoType: 'A' or 'N'
    [OperationContract]
    bool EDOAlterInstalmentDate(string atlasBranchNumber, int transactionId, string edoType, int installment, DateTime newDate,
      string frequency, out string errorMessage);


    [OperationContract]
    bool EDOAddContract(string atlasBranchNumber, string contractNo, decimal installAmount, int totInstalments, string frequency,
        DateTime startDate, string bankShortName, string branchNumber, string accountNumber, string accountName, int accountType, string clientIdentityNumber,
         out int transactionId, out string errorMessage);

    [OperationContract]
    bool EDOGetFutureTrans(string contractRef, string edoType,
      out DateTime nextInstalmentDate, out decimal nextInstalmentVal, out string errorMessage);

  }
}
