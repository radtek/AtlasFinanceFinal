using System;
using System.ComponentModel;

namespace Atlas.Enumerators
{
  public class NuCard
  {
    public enum AdminRequestResult
    {
      [Description("No operation was performed")]
      NoOperation = 0,

      [Description("OK")]
      Successful = 1,

      [Description("Invalid merchant number")]
      InvalidMerchant = -1,

      [Description("Invalid card number")]
      InvalidCard = -2,

      [Description("Invalid merchant for card")]
      InvalidMerchantForCard = -3,

      [Description("Card already authorised")]
      CardAlreadyAuth = -4,

      [Description("The card has no value / Cancellation already requested")]
      CardHasNoValueOrCancelled = -5,

      [Description("The card has expired")]
      CardExpired = -6,

      [Description("Invalid amount- less than minimum, exceeds balance, less than minimum load or will exceed maximum balance")]
      InvalidAmount = -7,

      [Description("TerminalID not valid or checksum incorrect")]
      TerminalIdOrChecksumBad = -8,

      [Description("A general error occurred")]
      GeneralError = -9,

      [Description("Custom error message (see ResultText field for description)")]
      CustomError = -10,

      [Description("Source transaction could not be found")]
      SourceTransNotFound = -11,

      [Description("Lost card")]
      CardLost = -12,

      [Description("Stolen card")]
      StolenCard = -13,

      [Description("Insufficient funds")]
      InsufficientFunds = -14,

      [Description("Invalid transaction – Electronic transactions only")]
      InvalidTrans = -15,

      [Description("Profile not linked to this terminal")]
      WrongProfileTerminal = -16,

      [Description("Card not linked to this profile")]
      CardNotLinkedToThisProfikle = -17,

      [Description("Card is not allocated")]
      CardNotAllocated = -18,

      [Description("Card already allocated")]
      CardAlreadyAllocated = -19,

      [Description("Duplicate transaction")]
      DuplicateTrans = -20,

      [Description("The card was stopped")]
      CardStopped = -21,

      [Description("Account already exists/card already linked")]
      CardAlreadyLinked = -22
    }

    public enum AdminRequestType
    {
      [Description("Not Set")]
      NotSet = 0,

      #region General administrative tasks

      [Description("Check whether branch has been properly configured for NuCard usage")]
      CheckBranchConfigured = 1,

      [Description("Check if NuPay NuCard services and encryption protocol")]
      CheckNuCardOnLine = 2,

      #endregion

      #region Card general tasks

      [Description("Link card to a specific profile")]
      LinkCard = 10,

      [Description("Allocate a card to a person")]
      AllocateCard = 11,

      [Description("Get balance on a card")]
      CardBalance = 12,

      [Description("Get card statement")]
      CardStatement = 13,

      [Description("Stop a card")]
      CardStop = 14,

      [Description("Update allocated card")]
      UpdateAllocatedCard = 15,

      [Description("Get card status")]
      CardStatus = 16,

      [Description("Unstop a stopped card")]
      UnstopAStoppedCard = 17,

      [Description("Multi-step card(s) process")]
      MultistepProcess = 18,

      [Description("Request card PIN be reset ")]
      ResetPin = 23,

      [Description("De-link card from profile")]
      DeLinkCard = 19,

      #endregion

      #region Fund related tasks (high security)

      [Description("Load card, deduct from Profile")]
      LoadCardDeductProfile = 20,

      [Description("Deduct from card, add to Profile")]
      DeductCardLoadProfile = 21,

      [Description("Transfer funds between cards")]
      TransferCardFunds = 22,

      [Description("Check if specified amount was deducted from a profile")]
      CheckAuthorisation = 24,

      [Description("Check if specified amount was loaded on a card")]
      CheckLoad = 25

      #endregion
    }

    public enum StockRequestType
    {
      [Description("Not Set")]
      NotSet = 0,

      [Description("Get card status")]
      GetCardStatus = 1,

      [Description("Receipt unknown card into stock")]
      ImportUnknownCard = 2,

      [Description("Get cards in transit for a branch")]
      GetCardsInTransitForBranch = 3,

      [Description("BranchAcceptInTransitCards")]
      BranchAcceptInTransitCards = 4,

      [Description("Send cards to a branch")]
      SendCardsToBranch = 5
    }

    public enum StockRequestResult
    {
      [Description("System error")]
      SystemError = 0,

      [Description("OK")]
      Successful = 1,

      [Description("Invalid SourceRequest parameters")]
      BadRequestParameters = 2,

      [Description("Can't locate card")]
      CantLocateCard = 3,

      [Description("Invalid cared number")]
      InvalidCardNumber = 4,

      [Description("Missing tracking number")]
      MissingTrackingNumber = 5,
    }

    public enum TransactionSourceType
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("WEB")]
      WEB = 1,
      [Description("API")]
      API = 2
    }

    public enum NuCardStatus
    {
      [Description("Status not set")]
      NotSet = 0,
      [Description("Card stopped as it has been lost")]
      Stopped_Lost = 1,
      [Description("Card stopped as it has been stolen")]
      Stopped_Stolen = 2,
      [Description("Card stopped pending outcome of query")]
      Stopped_OutcomeQuery = 3,
      [Description("Card stopped to consolidate onto single card")]
      Stopped_ConsolidateToSingle = 4,
      [Description("Card stopped as it is no longer active")]
      Stopped_NoLongerActive = 5,
      [Description("Card stopped as allowable PIN tries have been exceeded")]
      Stopped_PIN_Exceeded = 6,
      [Description("Suspected fraud")]
      Suspect_Fraud = 7,
      [Description("Emergency card replacement")]
      Emergency_Replacement = 8,
      [Description("Card in use")]
      USE = 9,
      [Description("Card is lost")]
      LOST = 10,
      [Description("Card is faulty")]
      FAULT = 11,
      [Description("Card has been issued")]
      ISSUE = 12,
      [Description("Card has expired")]
      EXPIR = 13,
      [Description("Card is active")]
      Active = 14,
      [Description("Card has Expired")]
      Expired = 15,
      [Description("Card is in stock")]
      InStock = 16,
      [Description("Card is in transit")]
      InTransit = 17,
      [Description("Card is linked to profile and not assigned")]
      Linked = 18
    }
  }
}
