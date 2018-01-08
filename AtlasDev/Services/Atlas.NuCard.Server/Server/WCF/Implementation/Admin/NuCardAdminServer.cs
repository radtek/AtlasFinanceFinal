/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2013 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Implementation of the NuCard
* 
* 
*  Author:
*  ------------------
*     Keith Blows
* 
*  Notes:
*  -------------------
*    This has been implemented while old cards/old stock in use- card checks have been relaxed
* 
*  Revision history: 
*  ------------------ 
*     2012-04-12 - Skeleton created
*     
*     2012-06-20 - Implementation started
*     
*     2012-06-25 - Implementation fleshed out    
*                  Added some more 'all-in-one' routines: 
*                  
*     2012-08-06-  Converted to XPO
*     
*     2012-08-07 - Started logging - new DB structures
*     
*     2012-08-10 - Added logging
*     
*     2012-08-14 - Completed logging and basics 
*     
*     2012-08-17 - All routines working and tested    
*     
*     2013-02-05 - Started overhaul/standardization of logging mechanism
*                 Started adding new XML-RPC methods
*     
*     2013-02-07 - Completed logging / fixes
*     
*     2013-10-23 - Added 'ResetPin', implemented 'CancelStopCard'
*     
*     2013-11-12 - Moved database routines to Atlas.Data library
* 
*     2013-11-13 - Clean-up- DRY: CheckCard() added, JsonSerialize all params for debugging, log4net standardized
*     
*     2013-12-11 - AllocateCard- if already allocated to person, return success
*                  LinkCard- if already linked to profile, return success
*                 
*     2013-12-13 - Split each WCF method into own class for manageability
*        
* 
*  All functions result:
*  -----------------------
*     -1 - Parameters bad
*      0 - No operation performed
*      1 - Successful
*      2 - Can't communicate with the service provider
*      3 - unexpected server error (database, logic error)
*   
* 
*   TODO: How to handle error in middle of multi-step routines?
*         Remove 'out' parameters and use a return class for each method
*         
* ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.ServiceModel;

using Atlas.NuCard.WCF.Interface;
using AtlasServer.WCF.Admin.Implementation;


namespace AtlasServer.WCF.Implementation
{
  /// <summary>
  /// WCF implementation of the Tutuka NuCard XML RPC API- adds intelligence/logging/etc.
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class NuCardAdminServer : INuCardAdminServer
  {    
    public int IsBranchConfigured(SourceRequest sourceRequest, out string errorMessage)
    {
      return IsBranchConfigured_Impl.IsBranchConfigured(sourceRequest, out errorMessage);
    }

        
    public int TryPingNuCard(SourceRequest sourceRequest, out string errorMessage)
    {
      return TryPingNuCard_Impl.TryPingNuCard(sourceRequest, out errorMessage);
    }

        
    public int LinkCard(SourceRequest sourceRequest, string cardNumber, string transactionID,
      out string serverTransactionID, out string errorMessage)
    {
      return LinkCard_Impl.LinkCard(sourceRequest, cardNumber, transactionID, out serverTransactionID, out errorMessage);
    }

    public int DeLinkCard(SourceRequest sourceRequest, string cardNumber, string transactionID,
      out string serverTransactionID, out string errorMessage)
    {
      return DeLinkCard_Impl.DeLinkCard(sourceRequest, cardNumber, transactionID, out serverTransactionID, out errorMessage);
    }

            
    public int AllocateCard(SourceRequest sourceRequest,
        string cardNumber, string firstName, string lastName, string idOrPassportNumber,
        string cellPhoneNumber, string transactionID,
        out string serverTransactionID, out string errorMessage)
    {
      return AllocateCard_Impl.AllocateCard(sourceRequest, cardNumber, firstName, lastName, idOrPassportNumber,
        cellPhoneNumber, transactionID, out serverTransactionID, out errorMessage);
    }

        
    public int CardBalance(SourceRequest sourceRequest,
        string cardNumber, string transactionID,
    out BalanceResult balanceResult,
    out string serverTransactionID, out string errorMessage)
    {
      return CardBalance_Impl.CardBalance(sourceRequest, cardNumber, transactionID, out balanceResult, out serverTransactionID, out errorMessage);
    }

        
    public int DeductFromProfileLoadCard(SourceRequest sourceRequest,
        string cardNumber,
        int amountInCents, string transactionID,
    out string serverTransactionID, out string errorMessage)
    {
      return DeductFromProfileLoadCard_Impl.DeductFromProfileLoadCard(sourceRequest, cardNumber, amountInCents, transactionID, out serverTransactionID, out errorMessage);
    }

        
    public int DeductFromCardLoadProfile(SourceRequest sourceRequest,
        string cardNumber, int amountInCents, string transactionID,
    out int transferredAmountInCents,
    out string serverTransactionID, out string errorMessage)
    {
      return DeductFromCardLoadProfile_Impl.DeductFromCardLoadProfile(sourceRequest, cardNumber, amountInCents, transactionID, out transferredAmountInCents,
        out  serverTransactionID, out errorMessage);
    }

        
    public int StopCard(SourceRequest sourceRequest,
        string cardNumber, int stopReasonCodeID, string transactionID,
    out int transferredAmountInCents,
    out string serverTransactionID, out string errorMessage)
    {
      return StopCard_Impl.StopCard(sourceRequest, cardNumber, stopReasonCodeID, transactionID, out transferredAmountInCents,
            out serverTransactionID, out errorMessage);
    }

        
    public int TransferFundsBetweenCards(SourceRequest sourceRequest,
        string cardNumberFrom, string cardNumberTo, int amountInCents, string transactionID,
        bool stopTheFromCard, int stopReasonCodeID, // optional- StopCardReason
        out int transferredAmountInCents,
        out string serverTransactionID, out string errorMessage)
    {
      return TransferFundsBetweenCards_Impl.TransferFundsBetweenCards(sourceRequest,
        cardNumberFrom, cardNumberTo, amountInCents, transactionID, stopTheFromCard, stopReasonCodeID, // optional- StopCardReason
        out transferredAmountInCents,
        out serverTransactionID, out errorMessage);
    }

        
    public int CardStatement(SourceRequest sourceRequest, string cardNumber,
      out StatementResult statementResult,
      out string serverTransactionID, out string errorMessage)
    {
      return CardStatement_Impl.CardStatement(sourceRequest, cardNumber, out statementResult, out serverTransactionID, out errorMessage);
    }

        
    public int UpdateAllocatedCard(SourceRequest sourceRequest,
      string cardNumber, string cellphoneNumber, string transactionID,
       out string serverTransactionID, out string errorMessage)
    {
      return UpdateAllocatedCard_Impl.UpdateAllocatedCard(sourceRequest, cardNumber, cellphoneNumber, transactionID, out serverTransactionID, out errorMessage);
    }

        
    public int GetCardStatus(SourceRequest sourceRequest,
      string cardNumber, string transactionID,
      out CardStatus cardStatus,
       out string serverTransactionID, out string errorMessage)
    {
      return GetCardStatus_Impl.GetCardStatus(sourceRequest, cardNumber, transactionID, out cardStatus, out serverTransactionID, out errorMessage);      
    }

        
    public int CancelStopCard(SourceRequest sourceRequest, string cardNumber, string transactionID,
      out string serverTransactionID, out string errorMessage)
    {
      return CancelStopCard_Impl.CancelStopCard(sourceRequest, cardNumber, transactionID, out serverTransactionID, out errorMessage);
    }

        
    public int AllocateAndDeductFromProfileLoadCard(SourceRequest sourceRequest,
        string cardNumber, string firstName, string lastName, string idOrPassportNumber, string cellPhoneNumber,
        int amountInCents, string transactionID,
    out string serverTransactionID, out string errorMessage)
    {
      return AllocateAndDeductFromProfileLoadCard_Impl.AllocateAndDeductFromProfileLoadCard(sourceRequest, cardNumber, firstName, lastName, idOrPassportNumber, cellPhoneNumber,
        amountInCents, transactionID, out serverTransactionID, out errorMessage);      
    }

        
    public int TransferFundsToNewCard(SourceRequest sourceRequest,
        string cardNumberFrom, string cardNumberTo, int stopReasonCodeID,
        string firstName, string lastName, string idOrPassportNumber, string cellPhoneNumber, string transactionID,
    out int transferredAmountInCents,
    out string serverTransactionID, out string errorMessage)
    {
      return TransferFundsToNewCard_Impl.TransferFundsToNewCard(sourceRequest, cardNumberFrom, cardNumberTo, stopReasonCodeID, firstName,
        lastName, idOrPassportNumber, cellPhoneNumber, transactionID, out transferredAmountInCents, out serverTransactionID, out errorMessage);      
    }

       
    public int ResetPin(SourceRequest sourceRequest, string cardNumber,
      out string serverTransactionID, out string errorMessage)
    {
      return ResetPin_Impl.ResetPin(sourceRequest, cardNumber, out serverTransactionID, out errorMessage);
    }
            
  }
}