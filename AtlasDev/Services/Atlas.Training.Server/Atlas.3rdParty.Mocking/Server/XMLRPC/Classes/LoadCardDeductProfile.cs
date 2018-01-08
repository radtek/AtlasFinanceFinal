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

#endregion


namespace Atlas.ThirdParty.XMLRPC.Classes
{
  public struct LoadCardDeductProfile_Input
  {
    public const string MethodName = "LoadCardDeductProfile";

    public string terminalID;

    public string profileNumber;

    public string cardNumber;

    public int requestAmount;

    public string transactionID;

    public DateTime transactionDate;

    public override string ToString()
    {
      return string.Format("{0}{1}{2}{3}{4}{5}{6:yyyyMMddTHH:mm:ss}",
          MethodName,
          terminalID,
          profileNumber,
          cardNumber,
          requestAmount,
          transactionID,
          transactionDate);
    }
  }


  public struct LoadCardDeductProfile_Output
  {
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string terminalID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string profileNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string cardNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? requestAmount;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string clientTransactionID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string serverTransactionID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? balanceAmount;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string authNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public DateTime? expiryDate;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? transactionFee;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? resultCode;

    public string resultText;
  }
}


