using System;
using Atlas.Enumerators;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public sealed class Case : ICase
  {
    public long CaseId { get; set; }
    public long BranchId { get; set; }
    public string Reference { get; set; }
    public long DebtorId { get; set; }
    public General.Host Host { get; set; }
    public Framework.Enumerators.Stream.GroupType? GroupType { get; set; }
    public Framework.Enumerators.CaseStatus.Type? CaseStatusType { get; set; }
    public Framework.Enumerators.Stream.SubCategory SubCategoryType { get; set; }
    public Framework.Enumerators.Stream.PriorityType? Priority { get; set; }
    public Framework.Enumerators.Category.Type? CategoryType { get; set; }
    public DateTime LastStatusDate { get; set; }
    public decimal TotalLoanAmount { get; set; }
    public decimal TotalBalance { get; set; }
    public DateTime? LastReceiptDate { get; set; }
    public decimal? LastReceiptAmount { get; set; }
    public decimal TotalRequiredPayment { get; set; }
    public int TotalInstalmentsOutstanding { get; set; }
    public decimal TotalArrearsAmount { get; set; }
    public long AllocatedUserId { get; set; }
    public string AllocatedUser { get; set; }
    public int SmsCount { get; set; }
    public bool? WorkableCase { get; set; }
    public DateTime? CompleteDate { get; set; }
    public DateTime CreateDate { get; set; }
  }
}