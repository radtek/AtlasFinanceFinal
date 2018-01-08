using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Workflow.ProcessDataExt.Payout
{
  [Serializable]
  public class Payout
  {
    public long AccountId { get; set; }
    public long PayoutId { get; set; }
    public int HostId { get; set; }
  }
}
