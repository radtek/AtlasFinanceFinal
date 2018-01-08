using System;
using System.Collections.Generic;
using Atlas.Ass.Framework.Structures.Stream;

namespace Atlas.Ass.Framework.Repository
{
  public interface IAssStreamRepository
  {
    ICollection<IClientLoan> GetClientLoansCollections(ICollection<long> notInAccountIds);
    ICollection<IClientLoan> GetClientLoansSales(ICollection<long> notInAccountIds, ICollection<string> branchNumbers);
    ICollection<IAccountTransaction> GetPaymentsOnSpecificDate(DateTime[] transactionDate, params long[] accountReference2);
    ICollection<IAccount> GetClosedAccounts(params long[] accountReference2);
    ICollection<IAccount> GetHandedoverAccounts(DateTime startDate, DateTime endDate, params long[] accountReference2);
  }
}