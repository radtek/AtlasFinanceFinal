using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Ass.Framework.Repository;
using Atlas.Ass.Framework.Structures.Stream;
using Atlas.Ass.Repository.Properties;
using Atlas.Ass.Structures.Stream;
using Atlas.Common.Utils;
using Falcon.Common.Interfaces.Services;

namespace Atlas.Ass.Repository
{
  public class AssStreamRepository : IAssStreamRepository
  {
    private readonly IConfigService _configService;

    public AssStreamRepository(IConfigService configService)
    {
      _configService = configService;
    }

    public ICollection<IClientLoan> GetClientLoansCollections(ICollection<long> notInAccountIds)
    {
      var clientLoans = new List<IClientLoan>();
      clientLoans.AddRange(GetCustomQuery<ClientLoan>(string.Format(Resources.QRY_STR_ImportAccount,
        notInAccountIds.Count == 0 ? "0" : string.Join(",", notInAccountIds))));

      return clientLoans;
    }

    public ICollection<IClientLoan> GetClientLoansSales(ICollection<long> notInAccountIds,
      ICollection<string> branchNumbers)
    {
      var clientLoans = new List<IClientLoan>();
      foreach (var branch in branchNumbers)
      {
        clientLoans.AddRange(GetCustomQuery<ClientLoan>(string.Format(Resources.QRY_STR_ImportSales,
          branch, !notInAccountIds.Any() ? "0" : string.Join(",", notInAccountIds))));
      }
      return clientLoans;
    }

    public ICollection<IAccountTransaction> GetPaymentsOnSpecificDate(DateTime[] transactionDate,
      params long[] accountReference2)
    {
      var queryDaysInterval = 4; // days to go back to bring payments
      var payments = GetCustomQuery<AccountTransaction>(string.Format(Resources.QRY_STR_ImportAccountPayments,
        !accountReference2.Any() ? "0" : string.Join(",", accountReference2), queryDaysInterval));
      return new List<IAccountTransaction>(payments.Where(p => accountReference2.Contains(p.LoanReference) &&
                                                               transactionDate.Select(d => d.Date)
                                                                 .Contains(p.TransactionDate.Date)));
    }

    /// <summary>
    /// Gets accounts that are closed/paid up/handedover from the provided account reference/loanreferences
    /// </summary>
    /// <param name="accountReference2"></param>
    /// <returns></returns>
    public ICollection<IAccount> GetClosedAccounts(params long[] accountReference2)
    {
      return new List<IAccount>(GetCustomQuery<Account>(string.Format(Resources.QRY_STR_CheckAccounBalanceToClose,
        !accountReference2.Any() ? "0" : string.Join(",", accountReference2))));
    }

    /// <summary>
    /// Gets accounts that are handedover from the provided account reference/loanreferences
    /// </summary>
    /// <param name="accountReference2"></param>
    /// <returns></returns>
    public ICollection<IAccount> GetHandedoverAccounts(DateTime startDate, DateTime endDate, params long[] accountReference2)
    {
      return new List<IAccount>(GetCustomQuery<Account>(string.Format(Resources.STR_GetHandedoverCases,
        !accountReference2.Any() ? "0" : string.Join(",", accountReference2), startDate.Date.ToString("yyyy-MM-dd"), endDate.Date.ToString("yyyy-MM-dd"))));
    }

    internal List<T> GetCustomQuery<T>(string sql) where T : class, new()
    {
      var queryUtil = new RawSql();
      var data = queryUtil.ExecuteObject<T>(sql, _configService.AssConnection, commandTimeout: 3600);
      return data;
    }
  }
}