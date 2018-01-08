using System;

namespace Falcon.Common.Interfaces.Structures.Target
{
  public interface ILoanMixModel
  {
    long LoanMixId { get; set; }
    DateTime TargetDate { get; set; }
    int PayNo { get; set; }
    float Percent { get; set; }
  }
}
