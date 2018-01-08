using System;


namespace Atlas.WCF.FPServer.WCF.Implementation
{
  public class Machine
  {
    public Machine(Int64 id, string name, string ipAddresses, string hardwareKey, DateTime lastAccessDT, string legacyBranchNum)
    {
      Id = id;
      Name = name;
      IPAddresses = ipAddresses;
      HardwareKey = hardwareKey;
      LastAccessDT = lastAccessDT;
      LegacyBranchNum = legacyBranchNum;
    }

    public Int64 Id { get; private set; }

    public string Name { get; private set; }

    public string IPAddresses { get; private set; }

    public string HardwareKey { get; private set; }

    public DateTime LastAccessDT { get; set; }

    public string LegacyBranchNum { get; set; }

  }
}
