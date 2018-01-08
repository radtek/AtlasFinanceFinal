using Atlas.Domain.DTO;
using Atlas.Domain.Structures;
using Atlas.Enumerators;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using Atlas.Common.Attributes;
using DevExpress.Xpo;
using Atlas.Orchestration.Server.Structures;

namespace Atlas.Orchestration.Server.WCF.Interface
{
  [ServiceContract]
	[CyclicReferencesAware(true)]
  public interface IOrchestrationService
  {
    [OperationContract]
    PER_PersonDTO GetByIdNo(string idNo);

    [OperationContract]
    Tuple<long, long> Save(Person client);

    [OperationContract]
    PER_PersonDTO GetbyPk(long pk);

    [OperationContract]
    List<BankDetailDTO> GetBankDetails(long? personId);

    [OperationContract]
    BankDetailDTO GetBankDetail(long personId, string accountNo, bool isActive);

    [OperationContract]
    AccountVerification GetAccountVerification(string idNo, string accountNo, Enumerators.General.BankName bank, int daysAgo);

    [OperationContract]
    AccountVerification GetAccountVerificationById(long transactionId);

    [OperationContract]
    BankDetailDTO GetBankDetailByDetailId(long personId, long bankDetailId);

    [OperationContract]
    long? SaveBank(long personId, General.BankName bank, General.BankAccountType accountType, string accountName, string accountNo, General.BankPeriod accountPeriod, string branchCode);

    [OperationContract]
    void UpdateBankDetails(long personId, long detailId, bool isActive);

    [OperationContract]
    Tuple<Int32, string> GenerateOTP();

    [OperationContract]
    bool VerifyOTP(string security,int otp);

    [OperationContract]
    string GetCompiledTemplate(Notification.NotificationTemplate notificationTemplate, Dictionary<string, string> searchReplace);

    [OperationContract]
    long CreatePerson(string firstName, string lastName, string idNo);

    [OperationContract]
    bool PerformCDV(long bankId, long accountTypeId, string bankAccountNo, string branchCode);

    #region Policy
    
    [OperationContract]
    List<Enumerators.Account.Policy> CompanyPolicies(long accountId);
    
    [OperationContract]
    List<ACC_PolicyDTO> AccountPolicies(long personId);

    [OperationContract]
    int GetReApplyDelay(long personId);

    #endregion 
  }
}