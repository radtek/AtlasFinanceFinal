using System;
using System.Collections.Generic;
using Falcon.Common.Interfaces.Structures;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class WorkItem : IWorkItem
  {
    public long CaseId { get; set; }
    public long CaseStreamId { get; set; }
    public long CaseStreamActionId { get; set; }
    public long DebtorId { get; set; }
    public long BranchId { get; set; }
    public int GroupId { get; set; }
    public int StreamId { get; set; }
    public int NoActionCount { get; set; }
    public int ActionTypeId { get; set; }
    public int InstalmentsOustanding { get; set; }
    public int EscalationId { get; set; }
    public bool PerformNewCreditEnquiry { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? LastLoanDate { get; set; }
    public decimal LastLoanAmount{get; set; }
    public int LastLoanTerm { get; set; }
    public string LastLoanFrequency { get; set; }
    public DateTime? LastReceiptDate { get; set; }
    public decimal? ActionAmount { get; set; }
    public decimal ArrearsAmount { get; set; }
    public decimal Balance { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal? LastReceiptAmount { get; set; }
    public string Group { get; set; }
    public string CaseReference { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public string DebtorFirstname { get; set; }
    public string DebtorLastname { get; set; }
    public string DebtorIdNumber { get; set; }
    public List<IContact> DebtorCell { get; set; }
    public List<IContact> DebtorEmail { get; set; }
    public List<IContact> DebtorFax { get; set; }
    public List<IContact> DebtorWorkNo { get; set; }
    public List<IWorkItemAccount> LinkedAccounts { get; set; }
    public List<ITransaction> Transactions { get; set; }
  }
}