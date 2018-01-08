using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Falcon.Models
{
  public class AssUserOverrideModel
  {
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string BranchNum { get; set; }
    public string UserOperatorCode { get; set; }
    public string RegionalOperatorCode { get; set; }
    public byte NewLevel { get; set; }
    public string Reason { get; set; }
    
  }
}