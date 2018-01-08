using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class GroupStream : IGroupStream
  {
    public int GroupStreamId { get; set; }
    public IGroup Group { get; set; }
    public IStream Stream { get; set; }
    public int Ordinal { get; set; }
  }
}