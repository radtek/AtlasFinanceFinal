using System;


namespace Falcon.DuckHawk.Jobs.Attributes
{
  [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class CronExpressionAttribute : Attribute
  {
    public string Value { get; set; }

    public CronExpressionAttribute(string value)
    {
      this.Value = value;
    }
  }
}
