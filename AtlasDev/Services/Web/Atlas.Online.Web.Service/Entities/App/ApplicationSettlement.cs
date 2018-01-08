using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Web.Service.Entities.App
{
  public class ApplicationSettlementSubmission
  {
    public long ApplicationId { get; set; }
    public DateTime RepaymentDate { get; set; }
  }

  public class ApplicationSettlementResult
  {
    public decimal RepaymentAmount { get; set; }
  }


  public class ApplicationSettlementResponse
  {
    public DateTime RepaymentDate { get; set; }
    public decimal Amount { get; set; }
  }
}
