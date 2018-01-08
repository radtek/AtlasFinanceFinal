using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures
{
  public sealed class Host : IHost
  {
    public int HostId { get; set; }
    public string HostName { get; set; }
    public General.Host Type { get; set; }
  }
}
