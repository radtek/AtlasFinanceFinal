namespace Stream.Framework.Structures
{
  public interface IGroupStream
  {
    int GroupStreamId { get; set; }
    IGroup Group { get; set; }
    IStream Stream { get; set; }
    int Ordinal { get; set; }
  }
}