using System;
using System.Runtime.Serialization;

namespace Atlas.Integration
{
  [DataContract]
  public class Parameters
  {
    public string Parameter { get; set; }
    public string Value { get; set; }
    public bool PositiveOutcome { get; set; }
  }
}
