using System;


namespace Atlas.WCF.FPServer.ClientState
{
  /// <summary>
  /// Class to store FP hardware status
  /// </summary>
  public class FPHardware
  {
    /// <summary>
    /// Date/time created
    /// </summary>
    public DateTime CreatedTS { get; set; }

    /// <summary>
    /// Fingerprint device count
    /// </summary> 
    public int FPDeviceCount { get; set; }

    /// <summary>
    /// Last time hw/sw was persisted to the database
    /// </summary>
    public DateTime LastDBEntry { get; set; }

    /// <summary>
    /// Makes a deep copy of this object
    /// </summary>
    /// <returns></returns>
    internal Atlas.WCF.FPServer.ClientState.FPHardware DeepCopy()
    {
      var result = new Atlas.WCF.FPServer.ClientState.FPHardware() { CreatedTS = CreatedTS, FPDeviceCount = FPDeviceCount, LastDBEntry = LastDBEntry };
      return result;
    }
  }
}
