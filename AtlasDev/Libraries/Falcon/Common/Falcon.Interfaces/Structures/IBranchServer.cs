using System;

namespace Falcon.Common.Interfaces.Structures
{
  public interface IBranchServer
  {
    long BranchServerId { get; set; }
    long BranchId { get; set; }
    string BranchName { get; set; }
    //COR_MachineDTO Machine { get; set; }
    //bool MachineAuthorised { get; set; }
    DateTime LastSyncDate { get; set; }
    //ASS_DbUpdateScriptDTO RunningDBVersion { get; set; }
    //ASS_DbUpdateScriptDTO UseDBVersion { get; set; }
    //Int64 LastProcessedClientRecId { get; set; }
  }
}
