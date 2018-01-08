

using DevExpress.Xpo;
using System.ComponentModel;

namespace Atlas.Online.Data.Models.DTO
{
  public sealed class ContactDto
  {
    public int ContactId { get; set; }
    public ContactTypeDto ContactType { get; set; }
    public string Value { get; set; }
  }
}