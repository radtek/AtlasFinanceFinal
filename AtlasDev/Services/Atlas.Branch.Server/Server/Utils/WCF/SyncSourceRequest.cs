using System;

using Atlas.DataSync.WCF.Client.Utils;


namespace ASSSyncClient.Utils.WCF
{
  internal static class SyncSourceRequest
  {
    internal static Atlas.DataSync.WCF.Interface.SourceRequest CreateSourceRequest()
    {
      return SourceRequestUtils.CreateRequest(System.Configuration.ConfigurationManager.AppSettings["legacyBranchNum"], 0);
    }
  }
}
