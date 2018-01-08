using System;
using System.Linq;
using System.Collections.Generic;

using DevExpress.Xpo;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Domain.Model;
using Atlas.Cache.DomainMapper;
using Atlas.Cache.Interfaces.Classes;


namespace ASSServer.WCF.Implementation.Admin
{
  internal class UpdatedBranchServer_Impl
  {
    internal static void Execute(ILogging _log, ICacheServer cache, SourceRequest sourceRequest, string legacyBranchNum)
    {   
      _log.Information("UpdatedBranchServer_Impl.Execute: {@Request}", sourceRequest);

      try
      {
        using (var uow = new UnitOfWork())
        {
          var branch = uow.Query<BRN_Branch>().FirstOrDefault(s => s.LegacyBranchNum.PadLeft(3, '0') == legacyBranchNum.PadLeft(3, '0'));
          if (branch != null)
          {
            cache.Set(new List<BRN_Branch_Cached> { CacheDomainMapper.BRN_Branch_Mapper(branch) });
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "UpdatedBranchServer_Impl.Execute");
      }
    }
  }
}
