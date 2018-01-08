using System;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures.Bureau
{
  public class CompuscanProducts : ICompuscanProducts
  {
    public long BranchId { get; set; }
    public DateTime Date { get; set; }
    public int OneMonth { get; set; }
    public int OneMThin { get; set; }
    public int OneMCapped { get; set; }
    public int TwoToFourMonths { get; set; }
    public int FiveToSixMonths { get; set; }
    public int TwelveMonths { get; set; }
    public int Declined { get; set; }
  }
}