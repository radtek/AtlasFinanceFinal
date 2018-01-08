using Atlas.Enumerators;

namespace Falcon.Common.Interfaces.Structures
{
  public interface IHost
  {
    int HostId { get; set; }
    string HostName { get; set; }

    General.Host Type { get; set; }
  }
}