using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Workflow.ProcessDataExt.QuickQuote
{
  [Serializable]
  public class QuickQuote
  {
    public Int64 AccountId { get; set; }
    public Int64 ScoreCardId { get; set; }
    public Int64 HostId { get; set; }
  }
}