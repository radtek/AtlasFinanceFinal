using System;

namespace Falcon.Exporter.CiReport.Infrastructure.Attributes
{
  public class ColumnWidthAttribute : Attribute
  {
    public int Width { get; set; }

    public ColumnWidthAttribute(int width)
    {
      Width = width;
    }
  }
}