using System;

namespace Falcon.Gyrkin.Controllers.Api.Models.Target
{
  public class LoanMixModel 
  {
    public long LoanMixId { get; set; }
    public DateTime TargetDate { get; set; }
    public int PayNo { get; set; }
    public float Percent { get; set; }
  }
}