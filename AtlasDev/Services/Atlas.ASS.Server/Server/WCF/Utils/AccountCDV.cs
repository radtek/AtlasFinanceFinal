/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Bank Verification server
 * 
 * 
 *  Author:
 *  ------------------
 *     Lee Venkatsamy
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-10-23 - Initial Version
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.Common.Utils;


namespace Atlas.Business.BankVerification
{
  public class AccountCDV
  {
    #region Public methods

    public bool PerformCDV(long bankId, long accountTypeId, string bankAccountNo, string branchCode)
    {
      var passed = false;

      if (StringUtils.IsNumeric(bankAccountNo))
      {
        using (var UoW = new UnitOfWork())
        {
          var bank = new XPQuery<BNK_Bank>(UoW).FirstOrDefault(b => b.BankId == bankId);
          if (bank != null)
          {
            var bankCDV = new XPQuery<BNK_CDV>(UoW).FirstOrDefault(b => b.Bank.BankId == bank.BankId && b.AccountType.AccountTypeId == accountTypeId);
            if (bankCDV != null)
            {
              passed = StartCDV(bank.Type, bankCDV.Weighting, bankAccountNo, branchCode, bankCDV.Modulus,
                                                         bankCDV.FudgeFactor, bankCDV.ExceptCode);
            }
          }
          else
          {
            throw new Exception("Bank does not exist in DB");
          }
        }
      }
      return passed;
    }

    #endregion


    #region Private methods

    private bool VerifyException(string accountNo, string branchCode, string exceptCode)
    {
      var passed = false;
      if (!string.IsNullOrEmpty(exceptCode))
      {
        if (exceptCode == "b")
        {
          var allowSingleRemainder = false;
          var weighting = "1A987654321";
          byte modulus = 11;

          var leastDigit11 = int.Parse(accountNo.Substring(accountNo.Length - 1, 1));
          if (leastDigit11 <= 1)
            allowSingleRemainder = true;

          if (int.Parse(branchCode) >= 450236 && int.Parse(branchCode) <= 450237)
          {
            weighting = "00000000000";
            modulus = 0;
            allowSingleRemainder = true;
          }

          if (CheckResult(accountNo, weighting, 0, modulus, allowSingleRemainder))
            passed = true;
        }
        else if (exceptCode == "e")
        {
          accountNo = accountNo.PadLeft(11, '0');
          var weighting = "18765432100";
          var modulus = (byte)11;

          // Only if CDV fails, thereafter will Exception tested
          if (CheckResult(accountNo, weighting, 0, modulus, false))
          {
            passed = true;
          }
          else
          {
            var leastDigit1_2 = int.Parse(accountNo.Substring(0, 2));
            var leastDigit10 = int.Parse(accountNo.Substring(accountNo.Length - 10, 1));
            var leastDigit11 = int.Parse(accountNo.Substring(accountNo.Length - 11, 1));

            if (leastDigit1_2 > 0 && leastDigit11 == 0 && leastDigit10 > 0)
              passed = true;
          }
        }
        else if (exceptCode == "f")
        {
          if (accountNo.Length == 11)
          {
            if (accountNo.StartsWith("53"))
            {
              if (CheckResult(accountNo, "00000000000", 0, 0, false))
                passed = true;
            }
          }

          if (!passed)
          {
            var arrNextWeighting = new string[] { "17329874321", "14327654321", "54327654321", "11327654321" };
            byte weightCount = 0;
            while (weightCount < arrNextWeighting.Length)
            {
              passed = CheckResult(accountNo, arrNextWeighting[weightCount], 0, (weightCount == (byte)0 ? (byte)10 : (byte)11),
                                 (weightCount == 2 ? true : false));
              if (passed)
                break;
              weightCount += 1;
            }
          }

          if (!passed)
          {
            if (CheckResult(accountNo, "17329874321", 0, 10, false))
              passed = true;
          }

          if (!passed)
          {
            if (CheckResult(accountNo, "14327654321", 0, 11, false))
              passed = true;
          }

          if (!passed)
          {
            if (CheckResult(accountNo, "54327654321", 0, 11, false))
              passed = true;
          }

          if (!passed)
          {
            if (CheckResult(accountNo, "11327654321", 0, 11, true))
              passed = true;
          }

          if (!passed)
          {
            if (accountNo.Length < 10)
            {
              if (!CheckResult(accountNo, "11327654321", 0, 11, false))
              {
                var leastDigit1 = (int.Parse(accountNo.Substring(accountNo.Length - 1, 1)) + 6).ToString();
                var changedAccountNo = accountNo.Substring(0, accountNo.Length - 1) + leastDigit1.Substring(leastDigit1.Length - 1, 1);
                if (CheckResult(changedAccountNo, "11327654321", 0, 11, false))
                  passed = true;
              }
            }
          }

          if (!passed)
          {
            if (CheckResult(accountNo, "14329874321", 0, 10, false))
              passed = true;
          }
        }
        else if (exceptCode == "m") // Standard Bank
        {
          if (accountNo.Length <= 9)
          {
            accountNo = accountNo.PadLeft(9, Convert.ToChar("0"));
            if (CheckResult(accountNo, "11987654321", 0, 11, false))
              passed = true;
          }
          else if (accountNo.StartsWith("1") && accountNo.Length == 11)
          {
            if (CheckResult(accountNo, "1312987654321", 0, 11, false))
              passed = true;
          }
        }
      }
      return passed;
    }


    private bool CheckResult(string accountNo, string weighting, byte fudgeFactor, byte modulus, bool allowSingleRemainder)
    {
      if (accountNo.Length > 11)
      {
        return false;
      }

      // Pad account no.'s
      if (accountNo.Length != weighting.Length && weighting.Length <= 11)
      {
        accountNo = accountNo.PadLeft(weighting.Length, Convert.ToChar("0"));
      }

      // split weighting if >= 11 characters for CDV Routine 
      var weightingSplit = new int[11];
      if (weighting.Length >= 11)
      {
        if (weighting.Length > 11)
        {
          weightingSplit[0] = int.Parse(weighting.Substring(0, 2));
          weightingSplit[1] = int.Parse(weighting.Substring(2, 2));
        }
        else
        {
          weightingSplit[0] = GetWeightingValue(weighting.Substring(0, 1));
          weightingSplit[1] = GetWeightingValue(weighting.Substring(1, 1));
        }

        for (var i = 2; i < 11; i++)
        {
          weightingSplit[i] = GetWeightingValue(weighting[i + ((weighting.Length == 11) ? 0 : 2)].ToString());
        }
      }

      var passed = false;
      var totalWeight = 0;

      for (var i = accountNo.Length; i >= 1; i--)
      {
        if (((i + weighting.Length) - accountNo.Length) == 0)
          totalWeight = totalWeight + (int.Parse(accountNo[i].ToString()) * GetWeightingValue(weighting[(i + weighting.Length) - accountNo.Length].ToString()));
      }
      for (var i = 0; i <= accountNo.Length - 1; i++)
      {
        var weightingValue = GetWeightingValue(weighting[i + ((weighting.Length == 11) ? 0 : 2)].ToString());
        if (accountNo.Length >= 11)
        {
          weightingValue = weightingSplit[i];
        }
        totalWeight = totalWeight + (int.Parse(accountNo[i].ToString()) * weightingValue);
      }
      totalWeight = totalWeight + fudgeFactor;

      var modRemainder = 0;
      if (modulus > 0)
        modRemainder = totalWeight % modulus;
      if (modRemainder == 0 | (allowSingleRemainder && modRemainder == 1))
        passed = true;
      return passed;
    }


    private static int GetWeightingValue(string value)
    {
      switch (value)
      {
        case "A":
          return 10;
        case "B":
          return 11;
        case "C":
          return 12;
        case "D":
          return 13;
        case "E":
          return 14;
        case "F":
          return 15;
        case "G":
          return 16;
        case "H":
          return 17;
        case "I":
          return 18;
        case "J":
          return 19;
        case "K":
          return 20;
        case "L":
          return 21;
        case "M":
          return 22;
        case "N":
          return 23;
        case "O":
          return 24;
        case "P":
          return 25;
        case "Q":
          return 26;
        case "R":
          return 27;
        case "S":
          return 28;
        case "T":
          return 29;
        case "U":
          return 30;
        case "V":
          return 31;
        case "W":
          return 32;
        case "X":
          return 33;
        case "Y":
          return 34;
        case "Z":
          return 35;
        default:
          return int.Parse(value);
      }
    }


    private bool StartCDV(Enumerators.General.BankName bankType, string weighting, string accountNo, string branchCode, byte modulus, byte fudgeFactor, string exceptCode)
    {
      var cdvPass = false;
      var finalWeighting = "1";

      var bankAccountNo = accountNo;

      if (string.IsNullOrEmpty(weighting) && string.IsNullOrEmpty(exceptCode))
      {
        weighting = GetWeighting(bankType, branchCode);
      }

      if (weighting != null && string.IsNullOrEmpty(exceptCode))
      {
        finalWeighting = weighting;

        if (weighting != "1")
        {
          if (bankAccountNo.Length > weighting.Length)
            bankAccountNo = bankAccountNo.Substring(0, weighting.Length);
          else if (bankAccountNo.Length < weighting.Length)
            bankAccountNo = bankAccountNo.PadLeft(weighting.Length, Convert.ToChar("0"));
        }

      }
      if (weighting != null && string.IsNullOrEmpty(exceptCode))
      {
        if (CheckResult(bankAccountNo, finalWeighting, fudgeFactor, modulus, false))
          cdvPass = true;
      }
      else
      {
        if (VerifyException(bankAccountNo, branchCode, exceptCode))
          cdvPass = true;
      }

      return cdvPass;
    }


    private static string GetWeighting(Enumerators.General.BankName bankType, string branchCode)
    {
      if (bankType == Enumerators.General.BankName.UBA)
      {
        return (int.Parse(branchCode) >= 431000 && int.Parse(branchCode) <= 431979) ? "19876543211" : "27654321000";
      }
      return null;
    }

    #endregion

  }
}