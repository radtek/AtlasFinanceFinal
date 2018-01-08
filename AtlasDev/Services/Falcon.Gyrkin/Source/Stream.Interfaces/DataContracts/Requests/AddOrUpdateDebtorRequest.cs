using Stream.Framework.Structures;

namespace Stream.Framework.DataContracts.Requests
{
  public class AddOrUpdateDebtorRequest
  {
    public IDebtor Debtor { get; set; }
  }
}