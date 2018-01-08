using System;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class AccountStreamAction : IAccountStreamAction
  {
    public long CaseId { get; set; }
    public long CaseStreamId { get; set; }
    public long CaseStreamActionId { get; set; }
    public string AccountReference { get; set; }
    public string DebtorIdNumber { get; set; }
    public string DebtorFullName { get; set; }
    public string Priority { get; set; }
    public string CaseStatus { get; set; }
    public long AllocatedUserId { get; set; }
    public string AllocatedUserFullName { get; set; }
    public int StreamId { get; set; }
    public int ActionTypeId { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal ArrearsAmount { get; set; }
    public decimal Balance { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public DateTime ActionDate { get; set; }
    public int NoActionCount { get; set; }
  }
}