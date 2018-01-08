/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
* 
*  Description:
*  ------------------
*     XML-RPC Class for LinkCardBySequenceRange method
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
  public struct LinkCardBySequenceRange_Input
  {
    public const string MethodName = "LinkCardBySequenceRange";

    public string terminalID;

    public string profileNumber;

    public int startSequence;

    public int endSequence;

    public string transactionID;

    public DateTime transactionDate;

    public override string ToString()
    {
      return string.Format("{0}{1}{2}{3}{4}{5}{6:yyyyMMddTHH:mm:ss}",
          MethodName,
          terminalID,
          profileNumber,
          startSequence,
          endSequence,
          transactionID,
          transactionDate);
    }
  }


  public struct LinkCardBySequenceRange_Output
  {
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string terminalID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string profileNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? startSequence;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? endSequence;

    // List of cards already linked
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int[] cardsAlreadyLinked;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string clientTransactionID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string serverTransactionID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? resultCode;

    public string resultText;
  }
}
