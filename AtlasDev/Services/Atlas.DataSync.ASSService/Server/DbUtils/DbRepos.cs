using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Cache.Interfaces.Classes;
using Atlas.Cache.Interfaces;
using Atlas.Domain.Security;
using Atlas.Cache.DomainMapper;
using Atlas.Domain.Model;
using Atlas.Cache.DataUtils;

namespace ASSServer.DbUtils
{
  internal static class DbRepos
  {
    /// <summary>
    /// Log an ASS branch server event to DB
    /// </summary>
    /// <param name="branchServerId"></param>
    /// <param name="now"></param>
    /// <param name="methodName"></param>
    /// <param name="message"></param>
    /// <param name="v2"></param>
    internal static void LogASSBranchServerEvent(long branchServerId, DateTime raisedDT, string task, string eventMessage, int severity)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        var log = new ASS_LogSyncServerEvent(unitOfWork)
        {
          Server = unitOfWork.Query<ASS_BranchServer>().FirstOrDefault(s => s.BranchServerId == branchServerId),
          RaisedDT = raisedDT,
          Task = task,
          EventMesage = eventMessage,
          Severity = severity
        };

        unitOfWork.CommitChanges();
      }
    }

  }
}
