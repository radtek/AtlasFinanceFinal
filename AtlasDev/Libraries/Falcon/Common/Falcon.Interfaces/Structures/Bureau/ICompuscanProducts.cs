using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Falcon.Common.Interfaces.Structures.Bureau
{
  public interface ICompuscanProducts
  {
    long BranchId { get; set; }
    DateTime Date { get; set; }
    int OneMonth { get; set; }
    int OneMThin { get; set; }
    int OneMCapped { get; set; }
    int TwoToFourMonths { get; set; }
    int FiveToSixMonths { get; set; }
    int TwelveMonths { get; set; }
    int Declined { get; set; }
  }
}