using System;


namespace Falcon.DuckHawk.Jobs.Attributes
{
  [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited=false)]
  public class TriggerNameAttribute : Attribute
  {
    public string Value { get; set; }

    public TriggerNameAttribute(string value)
    {
      this.Value = value;
    }
  }
}
