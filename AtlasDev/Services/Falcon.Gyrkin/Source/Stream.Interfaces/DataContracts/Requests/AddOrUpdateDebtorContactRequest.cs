using Atlas.Enumerators;

namespace Stream.Framework.DataContracts.Requests
{
  public class AddOrUpdateDebtorContactRequest
  {
    public long DebtorContactId { get; set; }
    public long DebtorId { get; set; }
    public General.ContactType ContactType { get; set; }
    public string Value { get; set; }
    public bool IsActive { get; set; }
    public long CreateUserId { get; set; }
  }
}
