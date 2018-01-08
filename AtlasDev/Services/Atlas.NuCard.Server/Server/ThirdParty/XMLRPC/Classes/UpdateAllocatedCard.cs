/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     XML-RPC Class for UpdateCard method
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-01-30- Skeleton created
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

#region Using

using System;
using CookComputing.XmlRpc;

#endregion


namespace Atlas.ThirdParty.XMLRPC.Classes
{
  public struct UpdateAllocatedCard_Input
  {
    public const string MethodName = "UpdateAllocatedCard";

    public string terminalID;

    public string profileNumber;

    public string cardNumber;

    public string cellphoneNumber;

    public string transactionID;

    public DateTime transactionDate;

    public override string ToString()
    {
      return string.Format("{0}{1}{2}{3}{4}{5}{6:yyyyMMddTHH:mm:ss}",
          MethodName,
          terminalID,
          profileNumber,
          cardNumber,
          cellphoneNumber,
          transactionID,
          transactionDate);
    }
  }


  public struct UpdateAllocatedCard_Output
  {
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string terminalID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string profileNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string cardNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string cellPhoneNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string clientTransactionID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string serverTransactionID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? resultCode;

    public string resultText;
  }
}
