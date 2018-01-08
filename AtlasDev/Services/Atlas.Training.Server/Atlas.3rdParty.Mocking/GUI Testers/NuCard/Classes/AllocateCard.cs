/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     XML-RPC Class for AllocateCard method
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
  public struct AllocateCard_Input
  {
    public const string MethodName = "AllocateCard";

    public string terminalID;

    public string profileNumber;

    public string cardNumber;

    public string firstName;

    public string lastName;

    public string idOrPassportNumber;

    public string cellPhoneNumber;

    public string transactionID;

    public DateTime transactionDate;

    public override string ToString()
    {
      return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9:yyyyMMddTHH:mm:ss}",
          MethodName,
          terminalID,
          profileNumber,
          cardNumber,
          firstName,
          lastName,
          idOrPassportNumber,
          cellPhoneNumber,
          transactionID,
          transactionDate);
    }
  }


  public struct AllocateCard_Output
  {
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string terminalID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string profileNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string cardNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string firstName;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string lastName;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string idOrPassportNumber;

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
