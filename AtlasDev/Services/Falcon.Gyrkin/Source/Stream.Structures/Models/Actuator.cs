using System;
using Falcon.Common.Interfaces.Structures;
using Falcon.Common.Interfaces.Structures.Reports.General;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class Actuator:IActuator
  {
    public int ActuatorId { get; set; }
    public IBranch Branch { get; set; }
    public IRegion Region { get; set; }
    public DateTime RangeStart { get; set; }
    public DateTime RangeEnd { get; set; }
    public bool IsActive { get; set; }
    public DateTime? DisableDate { get; set; }
    public DateTime CreateDate { get; set; }
  }
}
