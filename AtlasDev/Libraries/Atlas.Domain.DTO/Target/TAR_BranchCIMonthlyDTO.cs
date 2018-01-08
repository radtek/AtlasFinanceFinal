using System;

namespace Atlas.Domain.DTO
{
  public class TAR_BranchCIMonthlyDTO
  {
    public long BranchCIMonthlyId{ get; set; }
    public BRN_BranchDTO Branch { get; set; }
    public HostDTO Host { get; set; }
    public DateTime TargetDate { get; set; }
    public decimal Amount { get; set; }
    public float Percent { get; set; }
    public DateTime CreateDate { get; set; }
    public PER_PersonDTO CreateUser { get; set; }
  }
}