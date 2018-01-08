using System;
using System.Collections.Generic;

namespace Falcon.Common.Structures
{
  public sealed class ServiceState
  {
    public List<StateClass> States { get; set; }
  }

  public sealed class StateClass
  {
    public State State { get; set; }
    public string Description { get;set;}
    public DateTime StatDate { get;set;}
  }

  public enum State 
  {
    UP,
    DOWN
  }
}