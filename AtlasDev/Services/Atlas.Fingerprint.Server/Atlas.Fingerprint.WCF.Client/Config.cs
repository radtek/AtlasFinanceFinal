using System;


namespace Atlas.Fingerprint.WCF.Client
{
  internal class Config
  {
    public static string FPServerAddress
    {
      get
      {
        return System.Configuration.ConfigurationManager.AppSettings["Fingerprint.Server.Address"] as string ?? FP_SERVER_ADDRESS;
      }
    }


    /// <summary>
    /// The WCF Document server endpoint
    /// </summary>
#if (DEBUG)
    private static readonly string FP_SERVER_ADDRESS = "net.tcp://172.31.75.38:8200";   //"net.tcp://127.0.0.1:8200";
#else
    private static readonly string FP_SERVER_ADDRESS = "net.tcp://172.31.75.38:8200";    
#endif
  }
}
