using Falcon.Common.Interfaces.Structures.AVS;

namespace Falcon.Gyrkin.Controllers.Api.Models.Avs
{
  public class AvsServiceBank : IAvsServiceBank
  {
    public int ServiceId { get; set; }
    public long BankId { get; set; }
    public string BankName { get; set; }
    public bool IsLinked { get; set; }
  }
}
