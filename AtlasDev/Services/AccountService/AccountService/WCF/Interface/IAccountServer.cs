using Atlas.Common.Attributes;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Domain.Structures;
using Atlas.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.WCF.Interface
{
  [ServiceContract]
  public interface IAccountServer
  {
    [OperationContract]
    AccountInfo GetActiveAccount(long personId, out string error, out int result);

    [OperationContract]
    AccountInfo GetAccountInfo(long accountId, out string error, out int result);

    [OperationContract]
    List<AccountInfo> GetAllAccounts(long personId, out string error, out int result);

    [OperationContract]
    BankDetail GetActiveBankDetail(long personId, out string error, out int result);

    [OperationContract]
    List<PendingAVS> GetPendingAVS(long personId, long accountId, out string error, out int result);

    [OperationContract]
    AccountStatement GetStatement(long accountId);

    [OperationContract]
    int GetAVSFailureCount(long personId);

    [OperationContract]
    AffordabilityOption GetAffordabilityOption(long accountId, out string error, out int result);

    [OperationContract]
    bool AcceptAffordabilityOption(long accountId, long affordabilityOptionId, out string error, out int result);

    [OperationContract]
    bool RejectAffordabilityOption(long accountId, long affordabilityOptionId, out string error, out int result);

    [OperationContract]
    Quotation GetQuote(long accountId, out string error, out int result);

    [OperationContract]
    bool AcceptQuote(long accountId, out string error, out int result);

    [OperationContract]
    bool RejectQuote(long accountId, out string error, out int result);

    [OperationContract]
    decimal? GetSettlementAmount(long accountId, DateTime settlementDate, out string error, out int result);

    [OperationContract]
    Atlas.Domain.Structures.Settlement GetSettlementQuotation(long accountId, long settlementId);

    [OperationContract]
    decimal GetOverdueAmount(long accountId);

    [OperationContract]
    Settlement PostSettlement(long accountId, DateTime settlementDate, out string error, out int result);

    [OperationContract]
    bool UpdateAccountStatus(long accountId, Account.AccountStatus status, Account.AccountStatusReason? reason, Account.AccountStatusSubReason? subReason);

    [OperationContract]
    void SaveAffordabilityItem(long accountId, List<ACC_AffordabilityDTO> affordability);

    [OperationContract]
    [CyclicReferencesAware(true)]
    AccountInfo SaveAccount(decimal amount, int period, Account.PeriodFrequency frequency, long? personId, General.Host host);

    [OperationContract]
    AccountInfo GetAccount(long personId);

    [OperationContract]
    void WorkflowStart(long accountId);

    [OperationContract]
    void WorkflowComplete(long accountId);

    [OperationContract]
    void WorkflowStepUp(long accountId, Atlas.Enumerators.Workflow.ProcessStep? currentProcessStepId, Atlas.Enumerators.Workflow.ProcessStep? nextProcessStepId = 0);

    [OperationContract]
    bool UpdateBankDetail(long bankDetailId, bool isActive);
    
  }
}