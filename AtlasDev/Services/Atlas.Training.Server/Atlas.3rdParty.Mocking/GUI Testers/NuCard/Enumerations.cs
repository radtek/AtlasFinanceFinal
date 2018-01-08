/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
* 
*  Description:
*  ------------------
*     XML-RPC Enumerations
* 
* 
*  Author:
*  ------------------
*     Keith Blows
* 
* 
*  Revision history: 
*  ------------------ 
*     
* 
* 
* ----------------------------------------------------------------------------------------------------------------- */

#region Using

using System;

#endregion


namespace Atlas.WCF.XMLRPC.Cashless
{
  public enum StatementTransactionTypes
  {
    Load = 1,
    Deducation = 2,
    Authorisation = 3
  };


  public enum StopReasonCodes
  {
    CardLost = 1,
    CardStolen = 2,
    CardPendingQuery = 3,
    CardConsolidation = 4,
    CardNoLongerActive = 5,
    CardPINRetriesExceeded = 6,
    CardSuspectFraud = 7,
    CardEmergencyReplacement = 8
  };
}
