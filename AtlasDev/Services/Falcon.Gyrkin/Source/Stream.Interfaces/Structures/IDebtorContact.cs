using System;

namespace Stream.Framework.Structures
{
  public interface IDebtorContact
  {
    long DebtorContactId { get; set; }
    long DebtorId { get; set; }
    Atlas.Enumerators.General.ContactType ContactType { get; set; }
    string Value { get; set; }
    bool IsActive { get; set; }
    string CreateUser { get; set; }
    long CreateUserId { get; set; }
    DateTime CreateDate { get; set; }
  }
}