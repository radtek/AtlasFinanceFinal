using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualBasic;

using Atlas.Domain.DTO;


namespace Atlas.LoanEngine.General
{
  internal static class Calculations
  {
    /// <summary>
    /// Calculate Instalment for the desired loan amount according to period, interest, etc
    /// </summary>
    /// <param name="capitalAmount">amount instalments is calculated</param>
    /// <param name="interestRate">interest rate of the loan</param>
    /// <param name="period">length of loan</param>
    /// <param name="frequency">Daily/weekly/monthly loan</param>
    /// <param name="roundPeriod">rounds the period if true, the period will be rounded to the nearest int</param>
    /// <param name="lastPaymentDate">if period is not supplied, this can be used</param>
    /// <returns></returns>
    public static decimal CalculateInstalment(decimal capitalAmount, float interestRate, double period, Atlas.Enumerators.Account.PeriodFrequency frequency, bool? roundPeriod = null, DateTime? lastPaymentDate = null)
    {
      var interestRateFrequency = (interestRate / 100 / (frequency == Atlas.Enumerators.Account.PeriodFrequency.Daily ? 365 : (frequency == Atlas.Enumerators.Account.PeriodFrequency.Monthly ? 12 : 52)));
      //interestRateFrequency = 0.0017F;

      if (Atlas.Enumerators.Account.PeriodFrequency.Monthly == frequency)
      {
        if (roundPeriod != null && lastPaymentDate != null)
        {
          var days = (Convert.ToDateTime(lastPaymentDate) - DateTime.Today).Days;
          days = days < 0 ? days * -1 : days;
          period = days / 30.00;
          if (roundPeriod == true)
          {
            period = Math.Round(period);
          }
        }

        return Convert.ToDecimal(Financial.Pmt(interestRateFrequency, period, -Convert.ToDouble(capitalAmount), 0, DueDate.EndOfPeriod));
        //return (capitalAmount * (interestRateFrequency * Common.Utils.Math.Pow((1 + interestRateFrequency), period))) / (Common.Utils.Math.Pow((1 + interestRateFrequency), period) - 1);
      }
      else
      {
        if (lastPaymentDate != null)
          period = (Convert.ToDateTime(lastPaymentDate) - DateTime.Today).Days;
        //var a = ((Capitial + CalculateInitiationFee(Capitial) + ((CalculateInitiationFee(Capitial) + 50) * 0.14M)) * ((Interest / 365) / 100) * Term) + ((CalculateInitiationFee(Capitial) * 1.14M) + 57);

        return Math.Round(((capitalAmount * Convert.ToDecimal(interestRateFrequency)) * Convert.ToDecimal(period)) + capitalAmount, 2);
      }
    }


    public static decimal CalculateInterest(float interestRate, decimal amount, double period, Atlas.Enumerators.Account.PeriodFrequency frequency)
    {
      float interestRateFrequency = 0;
      switch (frequency)
      {
        case Enumerators.Account.PeriodFrequency.Daily:
          interestRateFrequency = (interestRate / 100 / 365);
          break;
        case Enumerators.Account.PeriodFrequency.Weekly:
          interestRateFrequency = (interestRate / 100 / 52);
          break;
        case Enumerators.Account.PeriodFrequency.Monthly:
          interestRateFrequency = (interestRate / 100 / 12);
          break;
      }
      return Math.Round(amount * Convert.ToDecimal(interestRateFrequency) * Convert.ToDecimal(period), 2);
    }


    public static decimal CalculateDailyInterest(float interestRate, decimal amount, double period)
    {
      return CalculateInterest(interestRate, amount, period, Atlas.Enumerators.Account.PeriodFrequency.Daily);
    }


    /// <summary>
    /// Repo Rate calculation for Unsecured Loans
    /// </summary>
    /// <param name="repoRate">The NCA Repo Rate</param>
    /// <param name="repoFactor">Factor from NCA</param>
    /// <param name="interestRate">Actual interest to be charged with the repo rate</param>
    /// <returns></returns>
    public static float CalculateRepoRate(float repoRate, float repoFactor, float interestRate)
    {
      return ((repoRate * repoFactor) + interestRate); // Formula based on NCA
    }


    /// <summary>
    /// Calculates the amount of the fee
    /// </summary>
    /// <param name="fee">fee to be calculated</param>
    /// <param name="option">option fee is calculated on</param>
    /// <returns></returns>
    public static decimal CalculateFee(LGR_FeeDTO fee, decimal? loanAmount, decimal? loanBalance, int period)
    {
      // if Range Type is amount
      if (fee.FeeRangeType == Atlas.Enumerators.GeneralLedger.FeeRangeType.Amount)
      {
        if (!((loanAmount ?? 0) >= fee.RangeStart && (loanAmount ?? 0) <= fee.RangeEnd))
        {
          return 0;
        }
      }
      // if range is based on period
      else if (fee.FeeRangeType == Atlas.Enumerators.GeneralLedger.FeeRangeType.Period)
      {
        if (!(period >= fee.RangeStart && period <= fee.RangeEnd))
        {
          return 0;
        }
      }

      decimal amount = 0M;
      // if fee has a set amount
      if (fee.Amount != null && fee.Amount > 0)
      {
        amount = Convert.ToDecimal(fee.Amount);
      }
      // if fee amount is based on percentage
      else if (fee.Percentage != null && fee.Percentage > 0)
      {
        var calculatingAmount = (fee.CalculateOnAccountBalance ?? false) ? loanBalance : loanAmount;
        amount = Convert.ToDecimal(calculatingAmount - (fee.LessAmount ?? 0)) * (Convert.ToDecimal(fee.Percentage) / 100M);
      }
      // Calculate VAT if applicable
      if (fee.VAT != null && fee.VAT > 0)
        amount = (amount * (Convert.ToDecimal(fee.VAT) / 100M)) + amount;
      return amount;
    }

  }
}