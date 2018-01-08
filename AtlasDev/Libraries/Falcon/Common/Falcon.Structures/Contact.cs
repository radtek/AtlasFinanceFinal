using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures
{
  public sealed class Contact : IContact
  {
    public long DebtorContactId { get; set; }
    public IContactType ContactType { get; set; }
    public string Value { get; set; }
  }
}
