using System;

namespace Falcon.Common.Interfaces.Structures
{
  public interface IContact
  {
    long DebtorContactId { get; set; }
    IContactType ContactType { get; set; }
    string Value { get; set; }
  }
}