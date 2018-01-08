using System;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class GroupHost : IGroupHost
  {
    public int GroupHostId { get; set; }
    public IGroup Group { get; set; }
    public DateTime? DisableDate { get; set; }
  }
}