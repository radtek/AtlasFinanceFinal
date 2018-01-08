using System;
using Falcon.TBR.Bureau.Interfaces;

namespace Falcon.TBR.Bureau.Structures
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