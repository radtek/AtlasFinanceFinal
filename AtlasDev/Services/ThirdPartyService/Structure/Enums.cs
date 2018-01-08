using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.ThirdParty.Service.Structure
{
  
  public class Enums
  {
    public enum Bank
    {
      StandardBank = 1,
      FirstNationalBank = 2,
      Nedbank = 3,
      Perm = 4,
      BolandBank = 5,
      BankOfLisbon = 6,
      Saambou = 7,
      NBS = 8,
      Telebank = 9,
      ABSA = 10,
      Unibank = 11,
      MercantileLisbon = 12,
      PostBank = 13,
      PEPBank = 14,
      PickNPay = 15,
      Capitec = 16,
      BankOfAthens = 17,
      AfricanBank = 18,
      InvestecBank = 19,
      UBank = 20,
      BidvestBank = 21
    }

    public enum BankAccountType
    {
      Cheque = 1,
      Savings = 2,
      Transmission = 3
    }

    public enum ControlStatus
    {
      New = 1,
      InProcess = 2,
      Completed = 3,
      Cancelled = 4,
      Cancelled_ValidationErrors = 5,
      CompletedWithFailedDebit = 6
    }

    public enum Status
    {
      New = 1,
      Cancelled = 2,
      OnHold = 3,
      Batched = 4,
      Submitted = 5,
      Successful = 6,
      Failed = 7,
      Disputed = 8
    }

    public enum TrackingDay
    {
      NoTracking = 0,
      OneDay = 1,
      TwoDays = 2,
      ThreeDays = 3,
      FourDays = 4,
      FiveDays = 5,
      SixDays = 6,
      SevenDays = 7,
      EightDays = 8,
      NineDays = 9,
      TenDays = 10,
      FourteenDays = 14,
      TwentyOneDays = 21,
      ThirtyTwoDays = 32
    }

    public enum ValidationError
    {
      InvalidAccountStatus = 1,
      BankAccountInvalidOrInactive = 2,
      AccountWithinClosingBalance = 3,
      AccountDoesNotExist = 4,
      ActionDateHasPassed = 5,
      AVSDoesNotExistOrPending = 6
    }

    public enum PayRule
    {
      Fri_Sat_Sun_To_Mon = 1,
      Sat_Sun_To_Fri = 2,
      Sat_Sun_To_Mon = 3,
      Sat_To_Fri_Sun_To_Mon = 4,
      Sun_To_Mon = 5,
      Sat_Sun_Mon_To_Friday = 6,
      Last_Working_Day_Of_Month = 7,
      Second_Last_Working_Day_Of_Month = 8,
      Last_Sunday = 9,
      Last_Monday = 10,
      Last_Tuesday = 11,
      Last_Wednesday = 12,
      Last_Thursday = 13,
      Last_Friday = 14,
      Last_Saturday = 15,
      Second_Last_Friday = 16,
      Friday_Before_Or_On_the_25th = 17
    }
  }
}