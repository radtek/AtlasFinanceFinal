using Atlas.Common.Extensions;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures
{
  public sealed class Bank : IBank
  {
    public Bank()
    {

    }

    public Bank(General.BankName bank)
    {
      BankId = (int)bank;
      BankName = bank.ToStringEnum();
    }

    public long BankId { get; set; }
    public string BankName { get; set; }
  }
}
