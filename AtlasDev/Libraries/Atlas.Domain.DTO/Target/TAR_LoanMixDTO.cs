using System;

namespace Atlas.Domain.DTO.Target
{
  public class TAR_LoanMixDTO
  {
    public long LoanMixId { get; set; }
    public DateTime TargetDate { get; set; }
    public int PayNo { get; set; }
    public float Percent { get; set; }
  }
}
