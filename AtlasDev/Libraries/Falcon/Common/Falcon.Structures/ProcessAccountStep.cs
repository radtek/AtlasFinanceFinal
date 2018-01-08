using System;

namespace Falcon.Common.Structures
{
  public sealed class ProcessAccount
  {
    public long AccountId { get; set; }
    public long PersonId { get; set; }
    public string AccountNo { get; set; }
    public long ProcessJobId { get; set; }
    public string Process { get; set; }
    public string IdNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string AccountType { get; set; }
    public string Status { get; set; }
    public DateTime ProcessStartDate { get; set; }
    public DateTime? ProcessEndDate { get; set; }
  }

  public class ProcessStepAccount
  {
    public long ProcessStepJobId { get; set; }
    public string ProcessStep { get; set; }
    public DateTime ProcessStepStartDate { get; set; }
    public DateTime? ProcessStepEndDate { get; set; }
    public string Process { get; set; }
  }
}
