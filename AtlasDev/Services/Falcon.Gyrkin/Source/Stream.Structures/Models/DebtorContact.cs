using System;
using Atlas.Enumerators;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class DebtorContact : IDebtorContact
  {
    public long DebtorContactId { get; set; }
    public long DebtorId { get; set; }
    public General.ContactType ContactType { get; set; }
    public string Value { get; set; }
    public bool IsActive { get; set; }
    public string CreateUser { get; set; }
    public long CreateUserId { get; set; }
    public DateTime CreateDate { get; set; }
  }
}
