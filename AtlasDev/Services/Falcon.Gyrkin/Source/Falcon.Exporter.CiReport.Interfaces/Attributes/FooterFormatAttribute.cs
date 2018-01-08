using System;
using System.Drawing;

namespace Falcon.Exporter.CiReport.Infrastructure.Attributes
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
  public class FooterFormatAttribute : Attribute
  {
    public bool Bold;
    public bool Italic;
    public Color ForeColor = Color.Black;
    public Color BackgroundColor = Color.White;

    public FooterFormatAttribute()
    {
    }

    public FooterFormatAttribute(Color foreColor)
    {
      ForeColor = foreColor;
    }

    public FooterFormatAttribute(Color foreColor, Color backgroundColor, bool bold = false, bool italic = false)
    {
      ForeColor = foreColor;
      BackgroundColor = backgroundColor;
      Bold = bold;
      Italic = italic;
    }

    public FooterFormatAttribute(bool bold, bool italic = false)
    {
      Bold = bold;
      Italic = italic;
    }
  }
}