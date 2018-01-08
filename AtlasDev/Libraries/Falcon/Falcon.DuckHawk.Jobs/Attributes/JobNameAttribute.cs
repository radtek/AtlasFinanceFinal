using System;


namespace Falcon.DuckHawk.Jobs.Attributes
{
  [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class JobNameAttribute : Attribute
  {
    public string Value { get; set; }

    public JobNameAttribute(string value)
    {
      this.Value = value;
    }
  }
}
