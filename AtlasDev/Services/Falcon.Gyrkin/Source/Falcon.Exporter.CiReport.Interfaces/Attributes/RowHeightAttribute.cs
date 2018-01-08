using System;

namespace Falcon.Exporter.CiReport.Infrastructure.Attributes
{
  public class RowHeightAttribute : Attribute
  {
    public int RowHeight;

    public RowHeightAttribute(int rowHeight = 60)
    {
      RowHeight = rowHeight;
    }
  }
}