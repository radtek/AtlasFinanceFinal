using System;
using System.Collections.Generic;
using System.Linq;

using Atlas.Evolution.Server.Code.Data.ASS_Models;


namespace Atlas.Evolution.Server.Code.Utils
{
  /// <summary>
  /// Class to perform Evolution calculations from ass data
  /// </summary>
  internal static class EvoCalcs
  {
    /// <summary>
    /// Calculates value (effectively settlement balance) of account as at 'asAt'
    /// </summary>
    /// <param name="cheque"></param>
    /// <param name="transactions"></param>
    /// <param name="asAt"></param>
    /// <returns></returns>
    internal static decimal CurrentBalance(decimal cheque, List<trans> transactions, DateTime asAt)
    {
      var sorted = transactions
        .Where(s => s.trdate <= asAt && s.trstat != s.trtype && s.tramount != null && s.trtype != "H") // trstat == trtype : exclude from calcs
        .OrderBy(s => s.trdate).ToList();

      var balance = cheque;

      if (sorted.Any())
      {
        foreach (var trans in sorted)
        {
          if (trans.trtype == "R" && trans.order == trans.seqno) // expected repayment- add accrued fees/interest - only 'R' where: order == seqno
          {
            balance += (trans.instinitfe ?? 0) +
                       (trans.instinitva ?? 0) +

                       (trans.interest ?? 0) +

                       (trans.servicefee ?? 0) +
                       (trans.servicevat ?? 0) +

                       (trans.inspremval ?? 0) +
                       (trans.inspremvat ?? 0) +

                       (trans.inspolival ?? 0) +
                       (trans.inspolivat ?? 0);
          }
          else if (trans.trtype != "R") // else an adjustment, payment, etc - subtract full amount from balance
          {
            balance -= trans.tramount.Value;
          }
        }
      }

      return balance;
    }


    /// <summary>
    /// Calculates amount overdue as at 'asAt'
    /// </summary>
    /// <param name="transactions"></param>
    /// <param name="asAt"></param>
    /// <returns></returns>
    internal static decimal OverdueAmount(List<trans> transactions, DateTime asAt)
    {
      var sorted = transactions
        .Where(s => s.trdate <= asAt && s.trstat != s.trtype && s.tramount != null && s.trtype != "H") // trstat == trtype : exclude from calcs
        .OrderBy(s => s.trdate).ToList();

      var overdue = 0M;

      if (sorted.Any())
      {
        foreach (var trans in sorted)
        {
          if (trans.trtype == "R" && trans.order == trans.seqno) // expected repayment. only 'R' where: order == seqno
          {
            overdue += trans.tramount.Value;
          }
          else if (trans.trtype != "R") // else an adjustment, payment, etc - subtract full amount from balance
          {
            overdue -= trans.tramount.Value;
          }
        }
      }

      return overdue;
    }

    /// <summary>
    /// Get date of most recent receipt, null if none
    /// </summary>
    /// <param name="transactions"></param>
    /// <returns></returns>
    internal static DateTime? LastReceipt(List<trans> transactions, DateTime asAt)
    {
      var lastPayment = transactions
        .Where(s => s.trdate <= asAt && s.trtype == "P")
        .OrderByDescending(s => s.trdate)
        .FirstOrDefault();
      return lastPayment?.trdate;
    }


    /// <summary>
    /// Get date of first incomplete payment
    /// </summary>
    /// <param name="transactions"></param>
    /// <returns></returns>
    internal static DateTime? FirstMissedPayment(List<trans> transactions, DateTime asAt)
    {
      var firstIncomplete = transactions
        .Where(s => s.trdate <= asAt && s.trtype == "R" && (s.trstat == " " || s.trstat == "H") && s.tramount != null)
        .OrderBy(s => s.trdate)
        .FirstOrDefault();
      return firstIncomplete?.trdate;
    }


    /// <summary>
    /// Get when account first became overdue
    /// </summary>
    /// <param name="transactions"></param>
    /// <param name="asAt"></param>
    /// <returns></returns>
    internal static DateTime? OverdueSince(List<trans> transactions, DateTime asAt)
    {
      if (OverdueAmount(transactions, asAt) <= 0)
      {
        return null;
      }

      if (transactions.Any())
      {
        var sorted = transactions
          .Where(s => s.trdate <= asAt)
          .OrderBy(s => s.trdate);

        return sorted
          .Where(s => s.trtype == "R" && s.tramount > 0 && (string.IsNullOrWhiteSpace(s.trstat) || s.trstat == "H"))
          .FirstOrDefault()?.trdate;
        //  What happens if they allocate payments wrong? They receipt for next instalment, instead of this one?!?

        /*
        var sortedDesc = transactions.Where(s => s.trdate <= asAt).OrderByDescending(s => s.trdate);        
        var lastActualPaid = sortedDesc.FirstOrDefault(s => s.trtype == "P"); //  P == paid / receipt of payment
        if (lastActualPaid == null) // never paid... use first instalment date as overdue since
        {
          var firstInstalment = sortedAsc.FirstOrDefault(s => s.trtype == "R"); // R == expected instalment
          return firstInstalment?.trdate;
        }

        // Next instalment due date, after last paid, is overdue date      
        var nextInstalment = sortedAsc.Where(s => s.trdate >= lastActualPaid.trdate).FirstOrDefault(s => s.trtype == "R");
        return nextInstalment?.trdate;
        */
      }

      return null;
    }

    internal static bool LoanIsMoreThan1Month(string payFrequency, int loanPeriod)
    {
      switch (payFrequency)
      {
        case "W":
          return loanPeriod > 4;

        case "B":
          return loanPeriod > 2;

        default:
          return loanPeriod > 1;
      }
    }


    internal static decimal GetMonthSalary(string payFrequency, decimal payAmount)
    {
      switch (payFrequency)
      {
        case "W":
          return payAmount * 4;

        case "B":
          return payAmount * 2;

        default:
          return payAmount;
      }
    }


    internal static uint GetLoanPeriodInMonths(string payFrequency, int loanPeriod)
    {
      switch (payFrequency)
      {
        case "W":
          return (uint)((decimal)loanPeriod / 4M);

        case "B":
          return (uint)((decimal)loanPeriod / 2M);

        default:
          return (uint)loanPeriod;
      }
    }

  }
}
