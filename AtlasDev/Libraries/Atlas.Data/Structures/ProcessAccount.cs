using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Structures
{
  public class ProcessAccount
  {
    public long AccountId { get; set; }
    public DateTime AccountCreateDate { get; set; }
    public long PersonId { get; set; }
    public string AccountNo { get; set; }
    public long ProcessJobId { get; set; }
    public string Process { get; set; }
    public string IdNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string AccountType { get; set; }
    public string Status { get; set; }
    public DateTime StatusChange { get; set; }
    public decimal AppliedAmount { get; set; }
    public decimal Amount { get; set; }
    public decimal TotalPayBack { get; set; }
    public int Period { get; set; }
    public float InterestRate { get; set; }
    public string PeriodFrequency { get; set; }
    public DateTime ProcessStartDate { get; set; }
    public DateTime? ProcessEndDate { get; set; }
    public List<ProcessStepAccount> ProcessSteps { get; set; }
  }

  public class ProcessStepAccount
  {
    public long ProcessStepJobId { get; set; }
    public string ProcessStep { get; set; }
    public long ProcessStepJobAccountId { get; set; }
    public DateTime ProcessStepStartDate { get; set; }
    public DateTime? ProcessStepEndDate { get; set; }
    public string Process { get; set; }
  }
}
