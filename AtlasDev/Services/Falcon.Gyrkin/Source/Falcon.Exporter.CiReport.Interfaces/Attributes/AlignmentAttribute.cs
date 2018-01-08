using System;

namespace Falcon.Exporter.CiReport.Infrastructure.Attributes
{
  public class AlignmentAttribute : Attribute
  {
    public bool LeftAlign { get; set; }

    public AlignmentAttribute()
    {
      LeftAlign = true;
    }

    public AlignmentAttribute(string alignment)
    {
    }
  }
}