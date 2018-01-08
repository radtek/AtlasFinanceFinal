/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
* 
*  Description:
*  ------------------
*     XML-RPC Class for 'TransferFunds' method
* 
* 
*  Author:
*  ------------------
*     Keith Blows
* 
* 
*  Revision history: 
*  ------------------ 
*     2012-06-21- Created
* 
* 
* ----------------------------------------------------------------------------------------------------------------- */

#region Using

using System;
using CookComputing.XmlRpc;

#endregion


namespace Atlas.ThirdParty.XMLRPC.Classes
{
  public struct TransferFunds_Input
  {
    public const string MethodName = "TransferFunds";

    public string terminalID;

    public string profileNumber;

    public string cardNumberFrom;

    public string cardNumberTo;

    public int requestAmount;

    public string transactionID;

    public DateTime transactionDate;

    public override string ToString()
    {
      return string.Format("{0}{1}{2}{3}{4}{5}{6}{7:yyyyMMddTHH:mm:ss}",
          MethodName,
          terminalID,
          profileNumber,
          cardNumberFrom,
          cardNumberTo,
          requestAmount,
          transactionID,
          transactionDate);
    }
  }


  public struct TransferFunds_Output
  {
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string terminalID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string profileNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string cardNumberFrom;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string cardNumberTo;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? requestAmount;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string clientTransactionID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string serverTransactionID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? balanceAmount;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? authNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public DateTime? expiryDate;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? transactionFee;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? resultCode;

    public string resultText;
  }
}
