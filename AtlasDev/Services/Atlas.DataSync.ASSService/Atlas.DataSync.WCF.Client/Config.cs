namespace Atlas.DataSync.WCF.Client
{
  internal class Config
  {
    /// <summary>
    /// The WCF Data Sync server endpoint
    /// </summary>
#if (DEBUG)
    internal static readonly string DATASYNC_SERVER_ADDRESS = "net.tcp://172.31.75.38:8200";

#else
    internal static readonly string DATASYNC_SERVER_ADDRESS = "net.tcp://172.31.75.38:8200";    
#endif
  }
}
