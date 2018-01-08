using Atlas.Enumerators;

namespace Stream.Framework.Services
{
  public interface IStreamService
  {
    void ImportNewCollectionAccounts();
    void ImportNewSaleAccounts();
    void ImportSalesTransactions();
    void ImportCollectionsTransactions();
    void CloseUpToDateCollectionCases(General.Host host);
    void ClosePaidupAccounts();
    void CheckAllCollectionPtps();

    bool CheckForPtpPayment(long caseStreamId);

    void RemoveAssArrearClientsFromSalesStream();
    void RemoveAssClientsWithNewLoansFromSalesStream();
    void RemoveAssDeceaseClients();
    void SetCaseCategory(long caseId, Enumerators.Stream.GroupType groupType);
    void SetCasePriority(long caseId, Enumerators.Stream.GroupType groupType);

    void RemoveDuplicateAccounts(Enumerators.Stream.GroupType groupType);
  }
}
