using System;


namespace Atlas.Document.WCF.Client
{
  internal class Config
  {
    public static string DocServerAddress
    {
      get
      {
        return System.Configuration.ConfigurationManager.AppSettings["Document.Server.Address"] as string ?? DOC_SERVER_ADDRESS;
      }
    }

    /// <summary>
    /// The WCF Document server endpoint
    /// </summary>
#if (DEBUG)    
    private static readonly string DOC_SERVER_ADDRESS = "net.tcp://172.31.75.38:8200";
#else
    private static readonly string DOC_SERVER_ADDRESS = "net.tcp://172.31.75.38:8200";    
#endif

  }
}
