using System;


namespace Falcon.DuckHawk.Jobs.Attributes
{
  [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class DisableJobAttribute : Attribute
  {
    public DisableJobAttribute()
    {
    }
  }
}
