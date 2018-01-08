using System;


namespace Atlas.DocServer.WCF.Implementation.Admin
{
  internal class ChunkStartUpload_Impl
  {
    internal static string Execute()
    {
      var correlationId = Guid.NewGuid().ToString("N");      
      return correlationId;
    }
  }
}
