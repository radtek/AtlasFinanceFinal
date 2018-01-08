using Falcon.Common.Interfaces.Structures.AVS;

namespace Falcon.Common.Structures.Avs
{
  public sealed class AvsServiceBank : IAvsServiceBank
  {
    public int ServiceId { get; set; }
    public long BankId { get; set; }
    public string BankName { get; set; }
    public bool IsLinked { get; set; }
  }
}
