using System;
using System.Collections.Generic;
using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Framework.Repository
{
  public interface IAssCiRepository
  {
    //void ImportNewAccounts(Enumerators.Stream.GroupType groupType);
    //List<long> ImportAccountTransactions(Dictionary<long, string> loanReferences);
    //void CalculateBalances(List<long> accountIds);
    //Dictionary<long, Tuple<long, bool?>> CheckPayment(List<IGetCaseStreamAction> ptpCases);
    //bool CheckForPayment(ICaseStreamAction ptpCase);
    //List<long> CheckForHandedOverClients(List<long> loanReferences);
    //Dictionary<long, bool> CheckForClientsWithNewerLoan(Dictionary<long, DateTime> clientsWithLoanDates);
    //Dictionary<long, Enumerators.CaseStatus.Type> CheckForHandoverAndPaidUps(List<long> loanReferences);

    #region Ass CI Report

    ICollection<IBasicLoan> RunBasicInfoQuery(ICollection<string> branchNos, DateTime startRange, DateTime endRange,
      TimeSpan? expiryTime = null);

    ICollection<IClientLoanInfo> RunClientInfoQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate,
      TimeSpan? expiryTime = null);

    ICollection<ICollectionRefund> RunCollectionRefundQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate,
      TimeSpan? expiryTime = null);

    ICollection<IVAP> RunVapQuery(ICollection<string> branchNos, DateTime startDate, DateTime endDate,
      TimeSpan? expiryTime = null);

    ICollection<IRolledAccounts> RunRolledAccountsQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate, TimeSpan? expiryTime = null);

    ICollection<IReswipes> RunReswipesQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate);

    ICollection<IHandoverInfo> RunHandoverInfoQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate);

    ICollection<IHandoverInfo_New> RunNewHandoverInfoQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate);

    ICollection<IPossibleHandover> RunPossibleHandoverQuery(ICollection<string> branchNos);

    ICollection<IArrears> RunArrearsQuery(ICollection<string> branchNos);

    ICollection<ICollections> RunCollectionsQuery(ICollection<string> branchNos);

    ICollection<IDebtorsBook> RunDebtorsBookQuery(ICollection<string> branchNos);

    ICollection<ILoansFlagged> RunLoansFlaggedQuery(ICollection<string> branchNos);

    #endregion
  }
}