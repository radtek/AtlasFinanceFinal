using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;


namespace ASSServer.WCF.Implementation.DataSync
{
  public static class GetServerMasterSyncId_Impl
  {
    public static Int64 Execute(ILogging log, SourceRequest sourceRequest)
    {
      var methodName = "GetServerMasterSyncId";
    
      try
      {
        log.Information("{MethodName} starting, {@Request}", methodName, sourceRequest);

        using (var unitOfWork = new UnitOfWork())
        {
          return unitOfWork.Query<ASS_MasterTableChangeTracking>().Max(s => s.RecId);
        }
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}- {@Request}", methodName, sourceRequest);
        return -1;
      }
    }

  }
}
