using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class StreamAccountNoteType : IStreamAccountNoteType
  {
    public int AccountNoteTypeId { get; set; }
    public string Description { get; set; }
  }
}
