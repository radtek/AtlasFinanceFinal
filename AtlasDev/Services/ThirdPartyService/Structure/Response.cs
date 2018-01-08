using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.ThirdParty.Service.Structure
{
  public class Response
  {
    public long ReferenceId { get; set; }
    public string ThirdPartyReference { get; set; }
    public string ResponseCode { get; set; }
    public string ResponseDescription { get; set; }
    public decimal Amount { get; set; }
    public Atlas.ThirdParty.Service.Structure.Enums.ControlStatus ControlStatus { get; set; }
    public Atlas.ThirdParty.Service.Structure.Enums.Status Status { get; set; }
    public List<Atlas.ThirdParty.Service.Structure.Enums.ValidationError> ValidationErrors { get; set; }
    public DateTime InstalmentDate { get; set; }
  }
}