using System;
using System.Drawing;

namespace Falcon.Exporter.CiReport.Infrastructure.Attributes
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
  public class DetailFormatAttribute : Attribute
  {
    public bool Bold;
    public bool Italic;
    public Color FontColor = Color.Black;
    public Color BackgroundColor = Color.White;
    public bool RightAlign;
    public bool Center;

    public DetailFormatAttribute()
    {
    }

    public DetailFormatAttribute(string alignment)
    {
      SetAlignment(alignment);
    }

    public DetailFormatAttribute(string fontColor, string backgroundColor, string alignment ="left", bool bold = false, bool italic = false)
    {
      FontColor = Color.FromName(fontColor);
      BackgroundColor = Color.FromName(backgroundColor);
      Bold = bold;
      Italic = italic;

      SetAlignment(alignment);
    }

    public DetailFormatAttribute(bool bold, bool italic = false, string alignment = "left")
    {
      Bold = bold;
      Italic = italic;

      SetAlignment(alignment);
    }

    private void SetAlignment(string alignment)
    {
      if (alignment.ToLower().Contains("right"))
      {
        RightAlign = true;
      }
      else if (alignment.ToLower().Contains("center"))
      {
        Center = true;
      }
    }
  }
}