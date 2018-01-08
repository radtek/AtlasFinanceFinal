using System;
using System.Linq;
using System.Collections.Generic;

using DevExpress.Xpo;

using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Domain.Model;


namespace ASSServer.Utils.PSQL.DbfImport
{
  public class SetBranchRunningDBVersion
  {
    /// <summary>
    /// Updates ASS_BranchServer with version they are running
    /// </summary>
    /// <param name="branchServerId"></param>
    /// <param name="version"></param>
    public static void Execute(ICacheServer cache, long branchServerId, string version)
    {
      // which is better/faster?
      //var versionId = cache.GetAll<ASS_DbUpdateScript_VerString_Cached>()
      //  .Where(s => s.DBVersion == version)
      //  .Select(s => s.GetId()).FirstOrDefault();

      long versionId;
      using (var uow = new UnitOfWork())
      {
        versionId = uow.Query<ASS_DbUpdateScript>().Where(s => s.DbVersion == version).Select(s => s.DbUpdateScriptId).FirstOrDefault();
      }
      
      if (versionId > 0)
      {
        cache.GetAndUpdateLocked<ASS_BranchServer_Cached>(branchServerId, server =>
        {
          server.RunningDBVersion = versionId;          
          return server;
        });
      }
    }

  }

}
