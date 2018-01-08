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
  public struct Statement_Input
  {
    public const string MethodName = "Statement";

    public string terminalID;

    public string profileNumber;

    public string cardNumber;

    public string transactionID;

    public DateTime transactionDate;

    public override string ToString()
    {
      return string.Format("{0}{1}{2}{3}{4}{5:yyyyMMddTHH:mm:ss}",
          MethodName,
          terminalID,
          profileNumber,
          cardNumber,
          transactionID,
          transactionDate);
    }
  }


  public struct Statement_Output
  {
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string terminalID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string profileNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string cardNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? balanceAmount;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public DateTime? expiryDate;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public Statement_Output_LineItem[] statement;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string clientTransactionID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string serverTransactionID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? resultCode;

    public string resultText;
  }


  public struct Statement_Output_LineItem
  {
    public DateTime transactionDate;

    public int transactionAmount;

    public string transactionDescription;

    public int transactionType;
  }

}