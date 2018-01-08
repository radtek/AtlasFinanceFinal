using System;
using Falcon.Common.Interfaces.Structures;

namespace Stream.Framework.Structures
{
  public interface IActuator
  {
    int ActuatorId { get; set; }
    IBranch Branch { get; set; }
    IRegion Region { get; set; }
    DateTime RangeStart { get; set; }
    DateTime RangeEnd { get; set; }
    bool IsActive { get; set; }
    DateTime? DisableDate { get; set; }
    DateTime CreateDate { get; set; }
  }
}