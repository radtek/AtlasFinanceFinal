using System;
using System.ComponentModel;

namespace Atlas.Enumerators
{
  public class Payout
  {
    public enum PayoutStatus
    {
      [Description("New")]
      New = 1,
      [Description("Cancelled")]
      Cancelled = 2,
      [Description("On Hold")]
      OnHold = 3,
      [Description("Batched")]
      Batched = 4,
      [Description("Submitted")]
      Submitted = 5,
      [Description("Successful")]
      Successful = 6,
      [Description("Failed")]
      Failed = 7,
      [Description("Removed")]
      Removed = 8
    }

    public enum PayoutBatchStatus
    {
      [Description("New")]
      New = 1,
      [Description("Validated with Errors")]
      ValidatedWithErrors = 2,
      [Description("Submitted - Waiting in Outbox")]
      SubmittedWaitingOutbox = 3,
      [Description("Submitted - Waiting for Reply")]
      SubmittedWaitingForReply = 4,
      [Description("Completed")]
      Completed = 5,
      [Description("Completed with Errors")]
      CompletedWithErrors = 6,
      [Description("Closed")]
      Closed = 7
    }

    public enum PayoutServiceType
    {
      [Description("RTC")]
      RTC = 1,
      [Description("PAAF")]
      PAAF = 2
    }

    public enum PayoutReplyCodeType
    {
      [Description("Successful")]
      Successful = 1,
      [Description("Warning")]
      Warning = 2,
      [Description("Failed")]
      Failed = 3
    }

    public enum Validation
    {
      [Description("AVS Not Successful")]
      AVSNotSuccessful = 1,
      [Description("Payout Amount Mismatch")]
      PayoutAmountMismatch = 2,
      [Description("Action date in the past")]
      ActionDatePast = 3
    }

    public enum ReplyCode
    {
      [Description("ACCEPTED")]
      Unknown_Accepted = 1,
      [Description("ACCT")]
      RTC_Accepted = 2,
      [Description("REJECTED")]
      Unknown_Rejected = 3,
      [Description("REJT")]
      RTC_Rejected = 4
    }

    public enum ResultQualifierCode
    {
      [Description("00001")]
      SecurityMessageHoldOnAccount = 1,
      [Description("00002")]
      CourtOrderHoldOnAccount = 2,
      [Description("00003")]
      CuratorshipHoldPresent = 3,
      [Description("00004")]
      DeceasedEstateHoldPresent = 4,
      [Description("00005")]
      InsolventEstateHoldPresent = 5,
      [Description("00006")]
      SpouseDeceasedHoldPresent = 6,
      [Description("00007")]
      AccountStoppedHold = 7,
      [Description("00008")]
      AccountFrozenHold = 8,
      [Description("00009")]
      BadDebtHoldOnAccount = 9,
      [Description("00010")]
      PotentialBadDebtHoldOnAccount = 10,
      [Description("00011")]
      LegalActionPresent = 11,
      [Description("00012")]
      AccountClosed = 12,
      [Description("00013")]
      AccountTransferred = 13,
      [Description("00014")]
      AccountClosurePending = 14,
      [Description("00015")]
      ThirdPartyPaymentsNotPermitted = 15,
      [Description("00016")]
      InsufficientFunds = 16,
      [Description("00017")]
      InsufficientFunds_AmountsNotCleared = 17,
      [Description("00018")]
      AccountDormant = 18,
      [Description("00019")]
      MaximumBalanceReached = 19,
      [Description("00020")]
      MaximumNumberOfCreditsPerDayReached = 20,
      [Description("00021")]
      MaximumNumberOfCreditsPerMonthReached = 21,
      [Description("00022")]
      MaximumNumberOfDepositsPerDayReached = 22,
      [Description("00023")]
      MaximumNumberOfDepositsPerMonthReached = 23,
      [Description("00024")]
      MaximumNumberOfDebitTransactionsPerDayReached = 24,
      [Description("00025")]
      MaximumNumberOfDebitTransactionsPerMonthReached = 25,
      [Description("00026")]
      MaximumNumberOfWithdrawalsPerDayReached = 26,
      [Description("00027")]
      MaximumNumberOfWithdrawalsPerMonthReached = 27,
      [Description("00028")]
      MaximumWithdrawalAmountPerDayReached = 28,
      [Description("00030")]
      MaximumWithdrawalAmountPerMonthReached = 30,
      [Description("00031")]
      ParameterSystemValidationError = 31,
      [Description("00032")]
      TransactionNotCateredForOnSavings = 32,
      [Description("00033")]
      Source_TargetNotTheSameAsSort = 33,
      [Description("00034")]
      TransactionTooFarBackdated = 34,
      [Description("00035")]
      TransactionNotFoundToCorrect = 35,
      [Description("00036")]
      ZeroTransactionAmountNotValid = 36,
      [Description("00037")]
      AccountNotFound = 37,
      [Description("00038")]
      TransactionInFuture = 38,
      [Description("00039")]
      InvalidSourceInitiator = 39,
      [Description("00040")]
      TransactionNotPermittedOnThisAccount = 40,
      [Description("00041")]
      CreditAmountLessThanMinimumAllowed = 41,
      [Description("00042")]
      NonResident_BlockedAccount = 42,
      [Description("00043")]
      AccountNotADepositAccount = 43,
      [Description("00044")]
      InstructionNotFound_CapitalTransfer = 44,
      [Description("00045")]
      MaturedAccount = 45,
      [Description("00046")]
      EffectiveDateNotEqualToCommencementDate = 46,
      [Description("80001")]
      InvalidTargetAccountClientNumber = 80001,
      [Description("80002")]
      InvalidTargetAccountClientName = 80002,
      [Description("80003")]
      InvalidAction_EffectiveDate = 80003,
      [Description("80004")]
      InvalidTargetAccountNumber_Redirect = 80004,
      [Description("80005")]
      AmountField_SNotNumericOrNegative = 80005,
      [Description("80501")]
      UnpaidViaAcb = 80501,
      [Description("81002")]
      TxMasterTiebreakInvalid = 81002,
      [Description("81003")]
      InputFileNumberInvalid = 81003,
      [Description("81004")]
      TransactionTraceNumberInvalid = 81004,
      [Description("81005")]
      TransactionPhaseInvalid = 81005,
      [Description("81007")]
      InputDistUserCodeInvalid = 81007,
      [Description("81008")]
      SourceOutputUserCodeInvalid = 81008,
      [Description("81009")]
      TargetOutputUserCodeInvalid = 81009,
      [Description("81010")]
      FileIdInvalid = 81010,
      [Description("81011")]
      FileTypeOfServiceInvalid = 81011,
      [Description("81012")]
      InvalidFileProcessingDate = 81012,
      [Description("81013")]
      TransactionEffectiveDateInvalid = 81013,
      [Description("81014")]
      TransactionStatusInvalid = 81014,
      [Description("81015")]
      RejectionReasonNotNumeric = 81015,
      [Description("81016")]
      RejectionQualifierInvalid = 81016,
      [Description("81017")]
      PositiveConfirmationRequiredInvalid = 81017,
      [Description("81019")]
      CdvValidationIndicatorInvalid = 81019,
      [Description("81021")]
      FileRejectedAsPerUserSelection = 81021,
      [Description("81028")]
      TransactionTypeInvalid = 81028,
      [Description("81029")]
      TransactionSubTypeInvalid = 81029,
      [Description("81030")]
      ProcessingSequenceInvalid = 81030,
      [Description("81031")]
      AcbUserCodeInvalid = 81031,
      [Description("81032")]
      AcbUserSequenceNumberNotNumeric = 81032,
      [Description("81034")]
      SourceAccountFinancialIndicatorInvalid = 81034,
      [Description("81035")]
      SourceAccountBranchInvalid = 81035,
      [Description("81036")]
      SourceAccountNumberInvalid = 81036,
      [Description("81038")]
      SourceAccountNumberInvalid_Length = 81038,
      [Description("81039")]
      SourceAccountTypeInvalid = 81039,
      [Description("81044")]
      SourceAccountIndexNotNumeric = 81044,
      [Description("81045")]
      TargetAccountFinancialIndicatorInvalid = 81045,
      [Description("81046")]
      TargetAccountBranchInvalid = 81046,
      [Description("81047")]
      TargetAccountNumberInvalid = 81047,
      [Description("81049")]
      TargetAccountNumberInvalid2 = 81049,
      [Description("81050")]
      TargetAccountTypeInvalid_Length_ = 81050,
      [Description("81058")]
      ActionDateInvalid = 81058,
      [Description("81059")]
      AmountRequestedNotNumericOrZero = 81059,
      [Description("81061")]
      TransactionTypeOfServiceInvalid = 81061,
      [Description("81063")]
      ConsolidatedTransactionIndicatorInvalid = 81063,
      [Description("81064")]
      TransactionLegIdentifierInvalid = 81064,
      [Description("81065")]
      EntryClassInvalid = 81065,
      [Description("81066")]
      TaxCodeInvalid = 81066,
      [Description("81090")]
      ProcessingOptionInvalid = 81090,
      [Description("81091")]
      InvalidOtrAccountNumber = 81091,
      [Description("81092")]
      SourceAccDetailsForBureauClientNotFound = 81092,
      [Description("81093")]
      TransactionType_SubTypeInvalid = 81093,
      [Description("81094")]
      Sdd11_Target_Acc_IndexNotToBe_GreaterThan_11 = 81094,
      [Description("81095")]
      InvalidIntiator = 81095,
      [Description("81096")]
      PaymentTypeIndicatorInvalid = 81096,
      [Description("81110")]
      InvalidTieBreak = 81110,
      [Description("81116")]
      PaymentTypeIndicatorInvalid2 = 81116,
      [Description("81128")]
      InvalidTargetAccountDroppedDate = 81128,
      [Description("81129")]
      Sdd11_Target_Acc_IndexMayNotBe_GreaterThan_11 = 81129,
      [Description("81141")]
      InvalidTransactionStatus = 81141,
      [Description("81142")]
      InputFileNotNumeric = 81142,
      [Description("81143")]
      InputFileNotValid = 81143,
      [Description("81144")]
      InvalidInternalTransactionNo = 81144,
      [Description("81145")]
      InvalidInternalTransactionNo2 = 81145,
      [Description("81146")]
      InvalidTransactionPhase = 81146,
      [Description("81147")]
      InvalidTransactionLog = 81147,
      [Description("81148")]
      InvalidDistributionUser = 81148,
      [Description("81149")]
      InvalidDistributionUser2 = 81149,
      [Description("81150")]
      InvalidDistributionUser3 = 81150,
      [Description("81151")]
      InvalidSourceDistributionUser = 81151,
      [Description("81152")]
      InvalidSourceDistributionUser2 = 81152,
      [Description("81153")]
      InvalidTargetDistributionUser = 81153,
      [Description("81154")]
      InvalidTargetDistributionUser2 = 81154,
      [Description("81155")]
      InvalidFileTypeOfService = 81155,
      [Description("81156")]
      InvalidFileProcessingDate2 = 81156,
      [Description("81157")]
      InvalidFileProcessingDate3 = 81157,
      [Description("81158")]
      InvalidTransactionEffectiveDate = 81158,
      [Description("81159")]
      InvalidTransactionEffectiveDate2 = 81159,
      [Description("81160")]
      InvalidTransactionEffectiveDate3 = 81160,
      [Description("81161")]
      InvalidRejectionReason = 81161,
      [Description("81162")]
      InvalidRejectionQualifier = 81162,
      [Description("81163")]
      InvalidRejectionQualifier2 = 81163,
      [Description("81164")]
      InvalidPositiveConfirmationRequiredIndicator = 81164,
      [Description("81165")]
      InvalidPendingHoldIndicator = 81165,
      [Description("81166")]
      InvalidCdvIndicator = 81166,
      [Description("81167")]
      InvalidDateRolledIndicator = 81167,
      [Description("81168")]
      InvalidSourceAccountDroppedDate = 81168,
      [Description("81169")]
      InvalidServiceRolledIndicator = 81169,
      [Description("81170")]
      InvalidRetryIndicator = 81170,
      [Description("81171")]
      InvalidRetryDays = 81171,
      [Description("81172")]
      InvalidRetryDays2 = 81172,
      [Description("81173")]
      InvalidTransactionType = 81173,
      [Description("81174")]
      InvalidTransactionSubType = 81174,
      [Description("81175")]
      InvalidTransactionType_SubTypeCombination = 81175,
      [Description("81176")]
      InvalidProcessingSequence = 81176,
      [Description("81177")]
      InvalidAcbUserCode = 81177,
      [Description("81178")]
      InvalidAcbUserCode2 = 81178,
      [Description("81179")]
      InvalidAcbUserSequence = 81179,
      [Description("81180")]
      InvalidSourceIndicator = 81180,
      [Description("81181")]
      InvalidSourceAccountBranch = 81181,
      [Description("81182")]
      InvalidSourceAccountBranch2 = 81182,
      [Description("81183")]
      InvalidSourceAccountLength = 81183,
      [Description("81184")]
      InvalidSourceAccountLength2 = 81184,
      [Description("81185")]
      InvalidSourceOriginalNumber = 81185,
      [Description("81186")]
      InvalidSourceOriginalType = 81186,
      [Description("81187")]
      InvalidSourceOriginalNo = 81187,
      [Description("81188")]
      InvalidSourceOriginalNo2 = 81188,
      [Description("81189")]
      InvalidSourceAccountIndex = 81189,
      [Description("81190")]
      InvalidTargetIndicator = 81190,
      [Description("81191")]
      InvalidTargetBranch = 81191,
      [Description("81192")]
      InvalidTargetBranch2 = 81192,
      [Description("81193")]
      InvalidTargetOriginalNo = 81193,
      [Description("81194")]
      InvalidTargetNoLength = 81194,
      [Description("81195")]
      InvalidTargetNoLength2 = 81195,
      [Description("81196")]
      InvalidTargetAccountType = 81196,
      [Description("81197")]
      InvalidTargetAccountType2 = 81197,
      [Description("81198")]
      InvalidTargetAccountHost = 81198,
      [Description("81199")]
      TransactionType_SubTypeInvalid1 = 81199,
      [Description("81501")]
      TransactionHasBeen_Withdrawn = 81501,
      [Description("81502")]
      TransactionHasBeen_Recalled = 81502,
      [Description("81503")]
      StopPaymentOnTransactionHasBeenActioned = 81503,
      [Description("81504")]
      InvalidReturnCodeFromDsm700D = 81504,
      [Description("81521")]
      FileRejectedAsPerUserSelection2 = 81521,
      [Description("82001")]
      DistUserInputRulesNotRegisteredForAcbUser = 82001,
      [Description("82002")]
      AcbUserCodeNotRegisteredForDistUser_InputRules = 82002,
      [Description("82021")]
      FileRejectedAsPerUserSelection3 = 82021,
      [Description("82045")]
      InvalidCharacterIn_S_TranRefField_CannotBeRepla = 82045,
      [Description("82046")]
      InvalidCharacterIn_T_TransRefField, CannotBeRepla = 82046,
      [Description("82047")]
      InvalidCharacterIn_T_TransactionAccountName = 82047,
      [Description("82048")]
      InvalidCharacterInTransactionClientNumber = 82048,
      [Description("82049")]
      InvalidCharacterIn_T_TransactionClientSName = 82049,
      [Description("82050")]
      Source_NominatedAccNumberNotRegisteredForDistUser = 82050,
      [Description("82051")]
      ShortNameDoesNotMatchFirst10CharOfReferenceFiel = 82051,
      [Description("82052")]
      InvalidSiteCodeOnInputDistributionUser = 82052,
      [Description("82501")]
      Source_NominatedAccountFailedTheCdvCheck = 82501,
      [Description("82502")]
      Target_HomingAccountFailedCdvCheck = 82502,
      [Description("82503")]
      Source_NominatedBranchCodeNotRegisteredAtAcb = 82503,
      [Description("82504")]
      Target_HomingBranchCodeNotRegisteredAtAcb = 82504,
      [Description("82505")]
      InvalidTypeForSource_NominatedAccount = 82505,
      [Description("82506")]
      InvalidTypeForTarget_HomingAccount = 82506,
      [Description("82507")]
      Cr_DrNotAllowedForSource_NominatedAccount = 82507,
      [Description("82508")]
      Cr_DrNotAllowedForTheTarget_HomingAccount = 82508,
      [Description("82509")]
      Cr_DrNotAllowedOnTheSource_NominatedAccount = 82509,
      [Description("82510")]
      Cr_DrNotAllowedForTheTarget_HomingAccount2 = 82510,
      [Description("82511")]
      InvalidReturnCodeFormDsm923D = 82511,
      [Description("82512")]
      InvalidReturnCodeFromDsm923D = 82512,
      [Description("82513")]
      AcbSourceAccountNumberMustBe_LessThan_13Digits = 82513,
      [Description("82514")]
      AcbTargetAccountNumberMustBe_LessThan_13Digits = 82514,
      [Description("82515")]
      InvalidAcbTypeOfService = 82515,
      [Description("82521")]
      FileRejectedAsPerUserSelection4 = 82521,
      [Description("83001")]
      Source_NominatedAccountNotFoundOnAccount_Look_Up = 83001,
      [Description("83002")]
      TargetAccountNotFoundOnAccount_Look_Up = 83002,
      [Description("83003")]
      InvalidSourceProduct = 83003,
      [Description("83004")]
      InvalidTargetProduct = 83004,
      [Description("83005")]
      IncomingAcbTargetAccIsNotAValidAbsaAccount = 83005,
      [Description("83006")]
      SourceAccount_Eps_IsNotOnTheAbsaChassis = 83006,
      [Description("83009")]
      SourceAccountFollow_MeNotAllowed = 83009,
      [Description("83010")]
      TargetAccountNumberInvalid_Follow_Me = 83010,
      [Description("83021")]
      FileRejectedAsPerUserSelection5 = 83021,
      [Description("83102")]
      Target_HomingAccountNumberNotFoundOnAcc_Look_Up = 83102,
      [Description("83202")]
      OutputDistUserNotRegistered = 83202,
      [Description("83221")]
      FileRejectedAsPerUserSelection6 = 83221,
      [Description("83241")]
      InvalidTransactionDateForServiceSelected = 83241,
      [Description("83242")]
      InvalidTransactionEffectiveDate_NoBackdatingSelect = 83242,
      [Description("83243")]
      DateModuleNotReturningNextProcessingDate = 83243,
      [Description("83244")]
      DateModuleNotReturningNextProcessingDate1 = 83244,
      [Description("83245")]
      DateModuleNotReturningDropDateOfTransaction = 83245,
      [Description("83501")]
      Transaction_Withdrawn = 83501,
      [Description("83502")]
      TransactionHasBeen_Recalled2 = 83502,
      [Description("83503")]
      PossibleStopPayment = 83503,
      [Description("83504")]
      InvalidReturnCodeFromDsm700D2 = 83504,
      [Description("83505")]
      Percent100_Match_PaymentStoppedAndUnpaid = 83505,
      [Description("83521")]
      FileRejectedAsPerUserSelection7 = 83521,
      [Description("83601")]
      InvalidReturnCodeFromDsm700D3 = 83601,
      [Description("83621")]
      FileRejectedAsPerUserSelection8 = 83621,
      [Description("84001")]
      FileIdNotRegisteredForInternalUser = 84001,
      [Description("84002")]
      ItemLimitExceeded_InputFileRejectedOnUserRequest = 84002,
      [Description("84003")]
      ItemLimitExceeded_OutputFileRejectedOnUserRequest = 84003,
      [Description("84004")]
      FileIdNotRegisteredForInternalUser1 = 84004,
      [Description("84005")]
      AcbUserCodeNotRegisteredForInputUser = 84005,
      [Description("84006")]
      AcbUserCodeNotRegisteredForOutputUser = 84006,
      [Description("84007")]
      AcbUserCodeNotRegisteredForUser = 84007,
      [Description("84008")]
      NominatedAccForDd_DcNotRegisteredForUser = 84008,
      [Description("84009")]
      DirectDebitItemLimitExceeded = 84009,
      [Description("84010")]
      DirectCreditItemLimitExceeded = 84010,
      [Description("84011")]
      VariableDebitItemLimitExceeded = 84011,
      [Description("84012")]
      VariableCreditItemLimitExceeded = 84012,
      [Description("84013")]
      NominatedAccForDo_SoNotRegisteredForInputUser = 84013,
      [Description("84014")]
      StopOrderExceedsDebitItemLimit_Input = 84014,
      [Description("84015")]
      StopOrderExceedsCreditItemLimit_Input = 84015,
      [Description("84016")]
      DebitOrderExceedsCreditItemLimit_Input = 84016,
      [Description("84017")]
      DebitOrderExceedsDebitItemLimit_Input = 84017,
      [Description("84018")]
      NominatedAccForDo_SoNotRegisteredForOutputUser = 84018,
      [Description("84019")]
      StopOrderExceedsDebitItemLimit_Output = 84019,
      [Description("84020")]
      StopOrderExceedsCreditItemLimit_Output = 84020,
      [Description("84021")]
      DebitOrderExceedsDebitItemLimit_Output = 84021,
      [Description("84022")]
      DebitOrderExceedsCreditItemLimit_Output = 84022,
      [Description("84023")]
      InvalidReturnCodeFromDsm095D_WakeUpCall = 84023,
      [Description("84024")]
      AggregateLimitExceeded_FileRejectedOnUserRequest = 84024,
      [Description("84025")]
      AggregateLimitExceeded_TranRejectedOnUserRequest = 84025,
      [Description("84026")]
      Warning_DebitAggregateLimitExceeded = 84026,
      [Description("84027")]
      Warning_CreditAggregateLimitExceeded = 84027,
      [Description("84028")]
      InvalidReturnCodeFromDsm095D_TransactionCall = 84028,
      [Description("84029")]
      InvalidReturnCodeFromDsm095D_TerminateCall = 84029,
      [Description("84030")]
      DebitsExceededPchLimit = 84030,
      [Description("84031")]
      CreditExceedPchLimit = 84031,
      [Description("84201")]
      DebitContraRecordRejected = 84201,
      [Description("84202")]
      CreditContraRecordRejected = 84202,
      [Description("84203")]
      Dr_CrContraRecordRejected_CorporativeFile = 84203,
      [Description("87101")]
      ManagersReferralRej950UsedIncorrectly_NotCqProd = 87101,
      [Description("88110")]
      InvalidDataOnTran = 88110,
      [Description("88174")]
      InvalidTransactionSub_Type = 88174,
      [Description("88175")]
      InvalidType_Sub_TypeCombination = 88175,
      [Description("88901")]
      TransactionNotOnTxMaster_Unpaid_Redirect = 88901,
      [Description("89201")]
      InvalidEntryToGl = 89201,
      [Description("91193")]
      InvalidHomingAccountNumber = 91193,
      [Description("91194")]
      InvalidHomingAccount = 91194,
      [Description("91195")]
      InvalidHomingAccount2 = 91195,
      [Description("91196")]
      InvalidAccountTypeForHomingAccountSelected = 91196,
      [Description("91197")]
      InvalidAccountTypeForHomingAccountSelected2 = 91197,
      [Description("92801")]
      TransactionReversed = 92801,
      [Description("92802")]
      TransactionStopped = 92802,
      [Description("92803")]
      TransactionDeclinedByManagersReferral = 92803,
      [Description("99901")]
      Error_AccNo_ClearingCode_TxType_Prod_Amount = 99901
    }

    public enum ResultCode
    {
      [Description("000")]
      Successful = 0,
      [Description("002")]
      NotProvidedFor = 2,
      [Description("003")]
      Debits_CreditsNotAllowedToAccount = 3,
      [Description("004")]
      PaymentStopped = 4,
      [Description("006")]
      AccountFrozen = 6,
      [Description("008")]
      AccountInLiquidation = 8,
      [Description("010")]
      AccountInSequestration = 10,
      [Description("012")]
      AccountClosed = 12,
      [Description("014")]
      AccountTransferredWithinBankingGroup = 14,
      [Description("016")]
      AccountTransferredToOtherBankingGroup = 16,
      [Description("018")]
      AccountHolderDeceased = 18,
      [Description("022")]
      AccountEffectsNotCleared = 22,
      [Description("026")]
      NoSuchAccount = 26,
      [Description("028")]
      Recall_Withdrawal = 28,
      [Description("030")]
      ClientDidNotAuthoriseDebit = 30,
      [Description("032")]
      DebitContravenesClientSAuthority = 32,
      [Description("034")]
      AuthorisationCancelled = 34,
      [Description("036")]
      PreviouslyStoppedAsStopPayment = 36,
      [Description("056")]
      NotFicaCompliant = 56,
      [Description("898")]
      PossibleStopPayment = 898,
      [Description("899")]
      DistributionUpfrontRejection = 899,
      [Description("900")]
      Interest_CapitalExceeded = 900,
      [Description("901")]
      PostDatedTransaction = 901,
      [Description("902")]
      LimitsViolation = 902,
      [Description("904")]
      SubscriptionAmountRequired = 904,
      [Description("905")]
      HistoryRecordNotFound = 905,
      [Description("906")]
      DataBaseDown = 906,
      [Description("907")]
      InterestCalculationError = 907,
      [Description("908")]
      ExceptionsError = 908,
      [Description("909")]
      Old_NewBalancesDiffer = 909,
      [Description("910")]
      NoBookError = 910,
      [Description("911")]
      OriginalTranNotFound = 911,
      [Description("912")]
      TranBackdatedBeyondLimit = 912,
      [Description("913")]
      InvalidBranch = 913,
      [Description("914")]
      BalanceExceedsMaximum = 914,
      [Description("915")]
      InvalidMode = 915,
      [Description("916")]
      BondCancelled = 916,
      [Description("917")]
      OverrideRequired = 917,
      [Description("918")]
      ClosedBeneficiaryCode = 918,
      [Description("919")]
      ClosedAcbBranchCode = 919,
      [Description("921")]
      NoTransferAccountInAdvance = 921,
      [Description("922")]
      AccountOpenNotPaidOut = 922,
      [Description("924")]
      AccountInAdvance = 924,
      [Description("925")]
      BridgesError = 925,
      [Description("926")]
      OtrError_ReferEpsqHistory = 926,
      [Description("927")]
      OnlineTransactionInProgress = 927,
      [Description("928")]
      TransactionWithdrawal = 928,
      [Description("929")]
      NewMortgageLoansInvalidDueDate = 929,
      [Description("930")]
      NewMortgageLoanFinancialError = 930,
      [Description("932")]
      PaafInsufficientFunds = 932,
      [Description("950")]
      TranOnManagerSReferral = 950,
      [Description("999")]
      InvalidData = 999,
      [Description("ACCT")]
      RTCSuccessful = 99901,
      [Description("REJT")]
      RTCRejected = 99902
    }
  }
}
