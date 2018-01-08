using System;
using System.Collections.Generic;
using Falcon.Common.Interfaces.Structures;

namespace Stream.Framework.Structures
{
  public interface IWorkItem
  {
    long CaseId { get; set; }
    long CaseStreamId { get; set; }
    long CaseStreamActionId { get; set; }
    long DebtorId { get; set; }
    long BranchId { get; set; }
    int GroupId { get; set; }
    int StreamId { get; set; }
    int NoActionCount { get; set; }
    int ActionTypeId { get; set; }
    int InstalmentsOustanding { get; set; }
    int EscalationId { get; set; }
    bool PerformNewCreditEnquiry { get; set; }
    DateTime ActionDate { get; set; }
    DateTime? LastLoanDate { get; set; }
    decimal LastLoanAmount{ get; set; }
    int LastLoanTerm { get; set; }
    string LastLoanFrequency{ get; set; }
    DateTime? LastReceiptDate { get; set; }
    decimal? ActionAmount { get; set; }
    decimal ArrearsAmount { get; set; }
    decimal Balance { get; set; }
    decimal LoanAmount { get; set; }
    decimal? LastReceiptAmount { get; set; }
    string Group { get; set; }
    string CaseReference { get; set; }
    string Category { get; set; }
    string SubCategory { get; set; }
    string DebtorFirstname { get; set; }
    string DebtorLastname { get; set; }
    string DebtorIdNumber { get; set; }
    List<IContact> DebtorCell { get; set; }
    List<IContact> DebtorEmail { get; set; }
    List<IContact> DebtorFax { get; set; }
    List<IContact> DebtorWorkNo { get; set; }
    List<IWorkItemAccount> LinkedAccounts { get; set; }
    List<ITransaction> Transactions { get; set; }
  }
}