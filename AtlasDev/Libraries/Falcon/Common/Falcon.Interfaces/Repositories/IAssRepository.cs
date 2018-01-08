using System;
using System.Collections.Generic;
using Atlas.Ass.Queries.DTO;

namespace Falcon.Common.Interfaces.Repositories
{
  public interface IAssRepository
  {
    void ImportNewAccounts(Enumerators.Stream.GroupType groupType);
    List<long> ImportAccountTransactions(Dictionary<long, string> loanReferences);
    void CalculateBalances(List<long> accountIds);
    Dictionary<long, Tuple<long, bool?>> CheckPayment(List<IGetCaseStreamAction> ptpCases);
    bool CheckForPayment(ICaseStreamAction ptpCase);
    List<long> CheckForHandedOverClients(List<long> loanReferences);
    Dictionary<long, bool> CheckForClientsWithNewerLoan(Dictionary<long, DateTime> clientsWithLoanDates);
    Dictionary<long, Stream.CaseStatus> CheckForHandoverAndPaidUps(List<long> loanReferences);
    List<long> GetDeceasedClients(List<long> clientReferences);
    List<long> GetAssActiveBranchIds();

    #region Ass CI Report

    ICollection<BasicLoan> RunBasicInfoQuery(ICollection<string> branchNos, DateTime startDate, DateTime endDate,
      TimeSpan? expiryTime = null);

    ICollection<ClientLoanInfo> RunClientInfoQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate,
      TimeSpan? expiryTime = null);

    ICollection<CollectionRefund> RunCollectionRefundQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate,
      TimeSpan? expiryTime = null);

    ICollection<VAP> RunVapQuery(ICollection<string> branchNos, DateTime startDate, DateTime endDate,
      TimeSpan? expiryTime = null);

    ICollection<RolledAccounts> RunRolledAccountsQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate, TimeSpan? expiryTime = null);

    ICollection<Reswipes> RunReswipesQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate);

    ICollection<HandoverInfo> RunHandoverInfoQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate);

    ICollection<HandoverInfo_New> RunNewHandoverInfoQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate);

    ICollection<PossibleHandover> RunPossibleHandoverQuery(ICollection<string> branchNos);

    ICollection<Arrears> RunArrearsQuery(ICollection<string> branchNos);

    ICollection<Collections> RunCollectionsQuery(ICollection<string> branchNos);

    ICollection<DebtorsBook> RunDebtorsBookQuery(ICollection<string> branchNos);

    ICollection<LoansFlagged> RunLoansFlaggedQuery(ICollection<string> branchNos);

    #endregion
  }
}