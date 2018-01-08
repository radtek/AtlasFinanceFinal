using System;
using System.Collections.Generic;

namespace Falcon.TBR.Bureau.Interfaces
{
  public interface ICreditResponse
  {
    DateTime? ScoreDate { get; set; }
    string Score { get; set; }
    List<IProduct> Products { get; set; }
    string Error { get; set; }
    int Age { get; set; }
  }
  public interface IReason
  {
    string Description { get; set; }
  }
  public interface IProduct
  {
    string Description { get; set; }
    string Outcome { get; set; }
    List<IReason> Reasons { get; set; }
  }
}