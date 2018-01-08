using System;

namespace Stream.Framework.Structures
{
  public interface IAccountStreamAction
  {
    long CaseId { get; set; }
    long CaseStreamId { get; set; }
    long CaseStreamActionId { get; set; }
    // case reference
    string AccountReference { get; set; } 
    string DebtorIdNumber { get; set; }
    string DebtorFullName { get; set; }
    string Priority { get; set; }
    string CaseStatus { get; set; }
    long AllocatedUserId{ get; set; }
    string AllocatedUserFullName { get; set; }
    int StreamId { get; set; }
    int ActionTypeId { get; set; }
    string Category { get; set; }
    string SubCategory { get; set; }
    decimal LoanAmount { get; set; }
    decimal ArrearsAmount { get; set; }
    decimal Balance { get; set; }
    DateTime ActionDate { get; set; }
    int NoActionCount{ get; set; }
  }
}