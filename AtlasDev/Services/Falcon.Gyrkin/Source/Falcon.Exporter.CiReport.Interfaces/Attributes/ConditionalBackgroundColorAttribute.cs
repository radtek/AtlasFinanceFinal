using System;
using System.Drawing;

namespace Falcon.Exporter.CiReport.Infrastructure.Attributes
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
  public class ConditionalBackgroundColorAttribute : Attribute
  {
    public Color BackgroundColor = Color.White;
    public string Condition;
    public ConditionalBackgroundColorAttribute(string condition, string backgroundColor)
    {
      Condition = condition;
      BackgroundColor = Color.FromName(backgroundColor);
    }
  }
}
