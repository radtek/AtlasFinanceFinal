/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Altech NuCard handling interface    
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-04-12 - Skeleton created
 *     2012-06-20 - fleshed out
 *     
 * 
 * 
 *  Notes:
 *  ------------------
 *     All Rand amounts are passed as their cent values.
 *     
 *     ResultCode: 0- Error, 1- OK
 *     
 * ----------------------------------------------------------------------------------------------------------------- */

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Runtime.Serialization;

#endregion


namespace Atlas.NuCard.WCF.Interface
{
  [ServiceContract(Namespace = "Atlas.ThirdParty.Altech.NuCard")]
  public interface INuCardAdminServer
  {
    // -------------------------------------------------------------------------------------------------------------------------------
    // Standard 1-to-1 functionality- these map directly to Tutuka operations- no added intelligence
    // -------------------------------------------------------------------------------------------------------------------------------

    // Link card to a profile
    [OperationContract]
    int LinkCard(SourceRequest sourceRequest, string cardNumber, string transactionID,
      out string serverTransactionID, out string errorMessage);
    
    
    // De-link card from profile   
    [OperationContract]
    int DeLinkCard(SourceRequest sourceRequest, string cardNumber, string transactionID,
      out string serverTransactionID, out string errorMessage);


    // Allocates a card to a person
    [OperationContract]
    int AllocateCard(SourceRequest sourceRequest,
      string cardNumber, string firstName, string lastName, string idOrPassportNumber,
      string cellPhoneNumber, string transactionID,
      out string serverTransactionID, out string errorMessage);

    // Returns a card's balance/info
    [OperationContract]
    int CardBalance(SourceRequest sourceRequest,
      string cardNumber, string transactionID,
      out BalanceResult balanceResult,
      out string serverTransactionID, out string errorMessage);

    // Loads funds onto a card
    [OperationContract]
    int DeductFromProfileLoadCard(SourceRequest sourceRequest,
      string cardNumber, int amountInCents, string transactionID,
      out string serverTransactionID, out string errorMessage);

    // Pass 0 or less for amountInCents, to transfer all funds from the card
    [OperationContract]
    int DeductFromCardLoadProfile(SourceRequest sourceRequest,
      string cardNumber, int amountInCents, string transactionID,
      out int transferredAmountInCents,
      out string serverTransactionID, out string errorMessage);

    // Transfers all the funds from one card to another
    // Pass 0 or less for amountInCents, to transfer all funds from the card
    [OperationContract]
    int TransferFundsBetweenCards(SourceRequest sourceRequest,
      string cardNumberFrom, string cardNumberTo, int amountInCents, string transactionID,
      bool stopTheFromCard, int stopReasonCodeID, // optional- StopCardReason
      out int transferredAmountInCents,
      out string serverTransactionID, out string errorMessage);

    // Gets statement for a card
    [OperationContract]
    int CardStatement(SourceRequest sourceRequest,
      string cardNumber,
      out StatementResult statementResult,
      out string serverTransactionID, out string errorMessage);

    // Stops a card
    [OperationContract]
    int StopCard(SourceRequest sourceRequest,
      string cardNumber, int stopReasonCodeID, string transactionID,
      out int transferredAmountInCents,
      out string serverTransactionID, out string errorMessage);

    // Get the status of a card
    [OperationContract]
    int GetCardStatus(SourceRequest sourceRequest,
      string cardNumber, string transactionID,
      out CardStatus cardStatus,
       out string serverTransactionID, out string errorMessage);

    // Unstop a cancelled card
    [OperationContract]
    int CancelStopCard(SourceRequest sourceRequest, string cardNumber, string transactionID,
      out string serverTransactionID, out string errorMessage);

    // Reset PIN for a card
    [OperationContract]
    int ResetPin(SourceRequest sourceRequest, string cardNumber,
      out string serverTransactionID, out string errorMessage);

    // Update cell number linked to a card
    [OperationContract]
    int UpdateAllocatedCard(SourceRequest sourceRequest,
      string cardNumber, string cellphoneNumber, string transactionID,
       out string serverTransactionID, out string errorMessage);   

    // -------------------------------------------------------------------------------------------------------------------------------
    // Custom operations- adds some intelligence
    // -------------------------------------------------------------------------------------------------------------------------------

    // Determines if this server can contact the Tutuka server
    [OperationContract]
    int TryPingNuCard(SourceRequest sourceRequest,
      out string errorMessage);

    // Determines if this branch has been configured for use with NuCard (branch has a profile)
    [OperationContract]
    int IsBranchConfigured(SourceRequest sourceRequest,
      out string errorMessage);

    // This will link the card to a profile and then load the funds from the profile
    [OperationContract]
    int AllocateAndDeductFromProfileLoadCard(SourceRequest sourceRequest,
      string cardNumber, string firstName, string lastName, string idOrPassportNumber, string cellPhoneNumber,
      int amountInCents, string transactionID,
      out string serverTransactionID, out string errorMessage);

    // Links new card to profile, transfers funds to new card and stops the old card 
    [OperationContract]
    int TransferFundsToNewCard(SourceRequest sourceRequest,
      string cardNumberFrom, string cardNumberTo, int stopReasonCodeID,
      string firstName, string lastName, string idOrPassportNumber, string cellPhoneNumber, string transactionID,
      out int transferredAmountInCents,
      out string serverTransactionID, out string errorMessage);    
    
  }
}
