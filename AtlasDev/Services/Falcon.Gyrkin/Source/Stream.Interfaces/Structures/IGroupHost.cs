using System;

namespace Stream.Framework.Structures
{
  public interface IGroupHost
  {
    int GroupHostId { get; set; }
    IGroup Group { get; set; }
    //IHost Host { get; set; }
    DateTime? DisableDate { get; set; }
  }
}