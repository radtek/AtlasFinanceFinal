using System;
using System.Drawing;

namespace Falcon.Exporter.CiReport.Infrastructure.Attributes
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
  public class HeaderFormatAttribute : Attribute
  {
    public bool Bold;
    public bool Italic;
    public Color FontColor = Color.Black;
    public Color BackgroundColor = Color.White;

    public HeaderFormatAttribute()
    {
    }

    public HeaderFormatAttribute(Color fontColor)
    {
      FontColor = fontColor;
    }

    public HeaderFormatAttribute(Color fontColor, Color backgroundColor, bool bold = false, bool italic = false)
    {
      FontColor = fontColor;
      BackgroundColor = backgroundColor;
      Bold = bold;
      Italic = italic;
    }

    public HeaderFormatAttribute(bool bold, bool italic = false)
    {
      Bold = bold;
      Italic = italic;
    }
  }
}