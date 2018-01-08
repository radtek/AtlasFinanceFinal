

using DevExpress.Xpo;
using System.ComponentModel;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class Contact : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public int ContactId { get; set; }
    [Persistent("ContactTypeId")]
    public ContactType ContactType { get; set; }
    public string Value { get; set; }
    [Persistent("ClientId")]
    [Association("Contact")]
    public Client Client { get; set; }

    public Contact() : base() { }
    public Contact(Session session) : base(session) { }
  }
}