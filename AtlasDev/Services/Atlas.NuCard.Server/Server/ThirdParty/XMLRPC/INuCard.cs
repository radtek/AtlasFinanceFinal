/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
* 
*  Description:
*  ------------------
*     
* 
* 
*  Author:
*  ------------------
*     Keith Blows
* 
* 
*  Revision history: 
*  ------------------ 
*     2012-04-19- Skeleton created
* 
* 
* ----------------------------------------------------------------------------------------------------------------- */

#region Using

using System;
using CookComputing.XmlRpc;
using Atlas.ThirdParty.XMLRPC.Classes;

#endregion


namespace Atlas.ThirdParty.XMLRPC.Utils
{
  public interface INuCard : IXmlRpcProxy
  {
    [XmlRpcMethod("LinkCard")]
    LinkCardCard_Output LinkCard(string terminalID, string profileNumber, string cardNumber, string transactionID,
      DateTime transactionDate, string checksum);

    [XmlRpcMethod("AllocateCard")]
    AllocateCard_Output AllocateCard(string terminalID, string profileNumber, string cardNumber, string firstName,
      string lastName, string idOrPassportNumber, string cellPhoneNumber, string transactionID,
      DateTime transactionDate, string checksum);
    [XmlRpcMethod("LinkCardsBySequenceRange")]
    LinkCardBySequenceRange_Output LinkCardsBySequenceRange(string terminalID, string profileNumber, int startSequence, int endSequence,
      string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("Balance")]
    Balance_Output Balance(string terminalID, string profileNumber, string cardNumber, string transactionID,
      DateTime transactionDate, string checksum);

    [XmlRpcMethod("DeductCardLoadProfile")]
    DeductCardLoadProfile_Output DeductCardLoadProfile(string terminalID, string profileNumber, string cardNumber,
      int requestAmount, string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("LoadCardDeductProfile")]
    LoadCardDeductProfile_Output LoadCardDeductProfile(string terminalID, string profileNumber, string cardNumber,
      int requestAmount, string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("TransferFunds")]
    TransferFunds_Output TransferFunds(string terminalID, string profileNumber, string cardNumberFrom, string cardNumberTo,
      int requestAmount, string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("Statement")]
    Statement_Output Statement(string terminalID, string profileNumber, string cardNumber,
      string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("StopCard")]
    StopCard_Output StopCard(string terminalID, string profileNumber, string cardNumber, int stopReasonID,
      string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("UpdateAllocatedCard")]
    UpdateAllocatedCard_Output UpdateAllocatedCard(string terminalID, string profileNumber, string cardNumber, string cellphoneNumber,
      string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("Status")]
    Status_Output Status(string terminalID, string profileNumber, string cardNumber,
      string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("CancelStopCard")]
    CancelStopCard_Output CancelStopCard(string terminalID, string profileNumber, string cardNumber,
      string transactionID, DateTime transactionDate, string checksum);
    
    [XmlRpcMethod("CheckAuthorisation")]
    CheckAuthorisation_Output CheckAuthorisation(string terminalID, string profileNumber, string cardNumber, int requestAmount,
      string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("CheckLoad")]
    CheckLoad_Output CheckLoad(string terminalID, string profileNumber, string cardNumber, int requestAmount,
      string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("ResetPin")]
    ResetPin_Output ResetPin(string terminalID, string profileNumber, string cardNumber, 
      string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("Register")]
    Register_Output Register(string terminalID, string emailAddress, string password, string firstName, string lastName, string idNumber,
      string contactNumber, string cellPhoneNumber, bool isCompany, string vatNumber, string companyName, string companyCCNumber, 
      string addressLine1, string addressLine2, string city,
      string postalCode, string transactionID, DateTime transactionDate, string checksum);

    [XmlRpcMethod("DeLinkCard")]
    DeLinkCard_Output DeLinkCard(string terminalID, string profileNumber, string cardNumber, string transactionID,
      DateTime transactionDate, string checksum);
  }
}
