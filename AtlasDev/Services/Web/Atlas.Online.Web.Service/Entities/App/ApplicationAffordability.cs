using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Web.Service.Entities.App
{
  public class ApplicationAffordability
  {
    public decimal Amount { get; set; }
    public decimal Fees { get; set; }
    public decimal InstalmentAmount { get; set; }
    public int Period { get; set; }
    public decimal RepaymentAmount { get; set; }
    public decimal InterestCharges { get; set; }
  }
}
