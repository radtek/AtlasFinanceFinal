using System;

namespace Atlas.Common.Attributes
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
  public class FormatAttribute : Attribute
  {
    public string Format { get; set; }

    public FormatAttribute(string format)
    {
      Format = format;
    }
  }
}