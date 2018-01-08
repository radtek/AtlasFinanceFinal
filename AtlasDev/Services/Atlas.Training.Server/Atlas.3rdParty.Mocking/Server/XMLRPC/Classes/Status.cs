/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     XML-RPC Class for Status method
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
  public struct Status_Input
  {
    public const string MethodName = "Status";

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


  public struct Status_Output
  {
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string expired;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string retired;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string stopped;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string stolen;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string valid;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string cancelled;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string loaded;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string lost;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string activated;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string redeemed;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string empty;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string pinBlocked;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string serverTransactionID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? resultCode;

    public string resultText;
  }
}
