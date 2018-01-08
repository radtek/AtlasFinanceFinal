using System;

namespace Atlas.Business.Constants
{
  public static class Messages
  {
    public const string PERSON_NOT_CONFIGURED_SECOND_LEVEL = "No person(s) found configured for second level approval";
    public const string OBJECT_CANNOT_BE_NULL = "{0} object cannot be null";
    public const string PROCESS_NOT_FOUND = "Process not found";
    public const string PERSON_ALLOCATED_REQUIRES_VALID_CELL_NO = "Person being allocated requires a valid cell phone number";
    public const string RECORD_IS_MISSING = "{0} record is missing";
    public const string CANNOT_ALLOCATE_CARD_ALLOCATED = "You cannot allocate an already allocated card";

    public const string ERROR_WHILE_PAYING_CARD = "An error occurred while trying to pay the card - {0}";

    public const string CARD_PAY_ERR_TITLE = "Pay Error";
    public const string APPROVAL_PROCESS_REDIRECT = "Approval process has started,  would you like to be directed to your work items?";
    public const string APPROVAL_PROCESS_REDIRECT_TITLE = "Approval";

    public const string NUCARD_ERROR_GETTING_CARD_BALANCE = "There was an error while trying to get the card balance - {0}";
    public const string NUCARD_CARD_BALANCE_ERR_TITLE = "Card Balance Error";
    public const string NUCARD_IS_NOT_ACTIVE = "Card is currently not active";
    public const string NUCARD_FUNDS_PROCESSED = "The amount of {0} has been transferred to nucard {1}";
    public const string NUCARD_CARD_TO_CARD = "A total of {0} has been transferred to the To card";
    public const string NUCARD_CARD_TO_CARD_TITLE = "Funds transferred";
    public const string NUCARD_CARD_TO_CARD_ERROR = "There was a problem with the transfer - {0}";
    public const string NUCARD_CARD_TO_CARD_ERROR_TITLE = "Transfer Error";
    public const string NUCARD_NO_STATEMENT = "There is currently no statement for this card";
    public const string NUCARD_NO_STATEMENT_TITLE = "No Statement";
    public const string NUCARD_ALLOCATION = "Card [{0}] has been linked to ID Number [{1}]";
    public const string NUCARD_ALLOCATION_TITLE = "Card Linked";
    public const string NUCARD_ALLOCATION_ERROR = "There was an error while trying to allocate the card - {0}";
    public const string NUCARD_ALLOCATION_ERROR_TITLE = "Allocation Error";
    public const string NUCARD_APPROVAL_PROCESS_HELD_UP = "You cannot approve this payment yet as the process is being held up by {0} {1}";
    public const string NUCARD_APPROVAL_PROCESS_HELD_UP_TITLE = "Payment Halted";
    public const string NUCARD_PAY_ERROR = "An error occurred while trying to pay the card - {0}";
    public const string NUCARD_PAY_ERROR_TITLE = "Pay Error";
    public const string NUCARD_NOT_YET_ALLOCATED = "The card is not allocated, please search for an allocated card";
    public const string NUCARD_NOT_YET_ALLOCATED_TITLE = "Not allocated";
    public const string NUCARD_CARD_NOT_FOUND = "Card not found";
    public const string NUCARD_STOP_CONFIRMATION = "Are you sure you wish to stop this card?";
    public const string NUCARD_STOP_CONFIRMATION_TITLE = "Stop Card";
    public const string NUCARD_CARD_HAS_BEEN_STOPPED = "The card has been stopped";
    public const string NUCARD_CARD_HAS_BEEN_STOPPED_TITLE = "Card Stopped";
    public const string NUCARD_STOP_ERROR = "An error occurred while attempting to stop the card - {0}";
    public const string NUCARD_STOP_ERROR_TITLE = "Stop Error";
    public const string NUCARD_PAYMENT_DECLINE_VERIFY = "Are you sure you want to decline this payment?";
    public const string NUCARD_PAYMENT_DECLINE_VERIFY_TITLE = "Payment Decline";
    public const string NUCARD_PAYMENT_DECLINE_CONFIRM = "Payment has been declined";

    public const string LOOK_UP_NONE_SELECTION = "No item selected";

    public const string CHEKING_CONNECTION = "Checking connection";
    public const string CONNECTION_ERROR = "Error with connection";
    public const string LOGIN_VERIFYING_CREDENTIALS = "Verifying user credentials";
    public const string LOGIN_INVALID_USERNAME_ANDOR_PASSWORD = "Invalid username and/or password";
    public const string LOGIN_INVALID_ACCOUNT_LOCKED = "The user account is locked";
    public const string LOGIN_VERIFIED = "Verified!";
    public const string LOGIN_USER_DOES_NOT_EXIST = "User does not exist";
    public const string LOGIN_USER_DOES_NOT_EXIST_TITLE = "Exist";
    public const string LOGIN_VERIFYING_HARDWARE_SIGNATURE = "Verifying hardware signature";

  }
}
