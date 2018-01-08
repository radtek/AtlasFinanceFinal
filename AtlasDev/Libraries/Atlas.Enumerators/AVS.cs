using System;
using System.ComponentModel;

namespace Atlas.Enumerators
{
  public class AVS
  {
    public enum Service
    {
      [Obsolete("ABSA- Removed")]
      [Description("ABSA")]
      ABSA = 1,

      [Obsolete("Hyphen- Removed")]
      [Description("Hyphen")]
      Hyphen = 2,

      [Obsolete("ABSA- Removed")]
      [Description("ABSA - Capitec")]
      ABSA_Capitec = 3,

      [Obsolete("ABSA- Removed")]
      [Description("ABSA - STD Bank")]
      ABSA_STDBank = 4,

      [Obsolete("ABSA- Removed")]
      [Description("ABSA - Nedbank")]
      ABSA_Nedbank = 5,

      [Obsolete("ABSA- Removed")]
      [Description("ABSA - FNB")]
      ABSA_FNB = 6,

      [Obsolete("CompuScan- unstable")]
      [Description("Compuscan")]
      Compuscan = 7,

      [Description("XDS")]
      XDS = 8,

      [Description("NuCard")]
      NuCard = 9,
    }

    [Obsolete("Check your usage- this is no longer applicable")]
    public enum ServiceType
    {
      [Obsolete("ABSA Removed")]
      [Description("ABSA to Other")]
      ABSA2Other = 1,

      [Obsolete("ABSA Removed")]
      [Description("ABSA to ABSA")]
      ABSA2ABSA = 2,

      [Obsolete("Hyphen Removed")]
      [Description("Hyphen")]
      Hyphen = 3,

      [Description("XDS")]
      XDS = 4,

      [Obsolete("CompuScan unstable Removed")]
      [Description("Compuscan")]
      Compuscan = 5,

      [Description("NuCard")]
      NuCard = 6
    }

    public enum Result
    {
      [Description("Passed")]
      Passed = 1,
      [Description("Passed With Warnings")]
      PassedWithWarnings = 2,
      [Description("Failed")]
      Failed = 3,
      [Description("No Result")]
      NoResult = 4,
      [Description("Locked")] // DO NOT USE ON BACKEND *** USED ON FE(WEB) AS LOCK DOWN INDICATOR ***
      Locked = 5
    }

    public enum Status
    {
      [Description("Queued")]
      Queued = 1,
      [Description("Pending")]
      Pending = 2,
      [Description("Complete")]
      Complete = 3,
      [Description("Cancelled")]
      Cancelled = 4
    }

    public enum ResponseCode
    {
      [Description("00")]
      Match = 1,
      [Description("01")]
      DoesNotExist = 2,
      [Description("02")]
      IDNumberDoesNotMatch = 3,
      [Description("03")]
      InitialsDoNotMatch = 4,
      [Description("04")]
      SurnameDoesNotMatch = 5,
      [Description("05")]
      AccountOpen_NoMatch = 6,
      [Description("06")]
      AcceptsDebit_NoMatch = 7,
      [Description("07")]
      AcceptsCredit_NoMatch = 8,
      [Description("08")]
      OpenThreeMonths_NoMatch = 9,
      [Description("21")]
      ErrorOnBankServ = 10,
      [Description("77")]
      NotValidatedNotParticipatingBanks = 11,
      [Description("88")]
      ResponseNotReceivedWithin1Hour = 12,
      [Description("99")]
      AccountDoesNotExist = 13
    }

    // Hypen Enquiry Result
    public enum VerificationResult
    {
      [Description("00001")]
      TimeoutReached = 1,
      [Description("00002")]
      ValidBranchCodeIsRequired = 2,
      [Description("00003")]
      AccountNumberMustNotBeBlank = 3,
      [Description("00004")]
      ValidUserCodeMustBeSupplied = 4,
      [Description("00005")]
      IdOrCompanyNumberMustBeSupplied = 5,
      [Description("00006")]
      UserIsNotRegisteredToUseThisFacility = 6
    }

    public enum ResponseGroup
    {
      [Description("Account Number")]
      AccountNumber = 1,
      [Description("Id Number")]
      IdNumber = 2,
      [Description("Initials")]
      Initials = 3,
      [Description("Surname")]
      Surname = 4,
      [Description("Account Open")]
      AccountOpen = 5,
      [Description("Accepts Debit")]
      AcceptsDebit = 6,
      [Description("Accepts Credit")]
      AcceptsCredit = 7,
      [Description("Open Three Months")]
      OpenThreeMonths = 8
    }

    public enum ResponseResult
    {
      [Description("Passed")]
      Passed = 1,
      [Description("Failed")]
      Failed = 2,
      [Description("No Response")]
      NoResponse = 3
    }
  }
}
