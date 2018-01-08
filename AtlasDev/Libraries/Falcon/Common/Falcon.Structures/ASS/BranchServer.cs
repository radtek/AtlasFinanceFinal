using System;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures.ASS
{
  public class BranchServer : IBranchServer
  {
    public long BranchServerId { get; set; }
    public long BranchId { get; set; }
    public string BranchName { get; set; }
    public DateTime LastSyncDate { get; set; }
  }
}