using System;
using Falcon.Common.Interfaces.Structures.Target;

namespace Falcon.Common.Structures.Target
{
  public class LoanMixModel:ILoanMixModel
  {
    public long LoanMixId { get; set; }
    public DateTime TargetDate { get; set; }
    public int PayNo { get; set; }
    public float Percent { get; set; }
  }
}
