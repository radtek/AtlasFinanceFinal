using System;
using Atlas.Enumerators;
using Stream.Framework.Enumerators;

namespace Stream.Framework.Structures
{
  public interface ICase
  {
    long CaseId { get; set; }
    long BranchId { get; set; }
    string Reference { get; set; }
    long DebtorId { get; set; }
    General.Host Host { get; set; }
    Enumerators.Stream.GroupType? GroupType { get; set; }
    CaseStatus.Type? CaseStatusType { get; set; }
    Enumerators.Stream.SubCategory SubCategoryType{get;set;}
    Enumerators.Stream.PriorityType? Priority{get;set;}
    Category.Type? CategoryType { get; set; }
    DateTime LastStatusDate { get; set; }
    decimal TotalLoanAmount{ get; set; }
    decimal TotalBalance { get; set; }
    DateTime? LastReceiptDate { get; set; }
    decimal? LastReceiptAmount { get; set; }
    decimal TotalRequiredPayment { get; set; }
    int TotalInstalmentsOutstanding { get; set; }
    decimal TotalArrearsAmount { get; set; }
    long AllocatedUserId { get; set; }
    string AllocatedUser { get; set; }
    int SmsCount { get; set; }
    DateTime? CompleteDate { get; set; }
    DateTime CreateDate { get; set; }
  }
}