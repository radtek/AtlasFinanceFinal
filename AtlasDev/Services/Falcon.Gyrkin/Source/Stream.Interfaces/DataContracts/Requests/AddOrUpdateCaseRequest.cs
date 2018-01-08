using System;
using Atlas.Enumerators;

namespace Stream.Framework.DataContracts.Requests
{
  public class AddOrUpdateCaseRequest
  {
    public AddOrUpdateCaseRequest()
    {
      WorkableCase = null;
    }

    public long CaseId { get; set; }
    public long BranchId { get; set; }
    public string Reference { get; set; }
    public long DebtorId { get; set; }
    public General.Host? Host { get; set; }
    public Enumerators.Stream.GroupType? GroupType { get; set; }
    public Enumerators.CaseStatus.Type? CaseStatus { get; set; }
    public Enumerators.Stream.SubCategory? SubCategory { get; set; }
    public Enumerators.Stream.PriorityType? Priority { get; set; }
    public DateTime LastStatusDate { get; set; }
    public decimal TotalLoanAmount{ get; set; }
    public decimal TotalBalance { get; set; }
    public DateTime? LastReceiptDate { get; set; }
    public decimal? LastReceiptAmount { get; set; }
    public decimal TotalRequiredPayment { get; set; }
    public int TotalInstalmentsOutstanding { get; set; }
    public decimal TotalArrearsAmount { get; set; }
    public long AllocatedUserId { get; set; }
    public int SmsCount { get; set; }
    public bool? WorkableCase { get; set; }
    public DateTime? CompleteDate { get; set; }
  }
}
