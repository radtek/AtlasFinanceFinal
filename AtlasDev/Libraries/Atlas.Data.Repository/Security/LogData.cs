#region Using

using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model;

#endregion


namespace Atlas.Data.Repository
{
  public static class LogData
  {
    /// <summary>
    /// Logs an error to the database
    /// </summary>    
    /// <param name="branchServerId">ASS_BranchServer.BranchServerId</param>
    /// <param name="raisedDT">Date/time error was raised</param>
    /// <param name="task">The task the system was busy with</param>
    /// <param name="eventMessage">The error message</param>    
    public static void LogASSBranchServerEvent(Int64 branchServerId, DateTime raisedDT, string task, string eventMessage, int severity)
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
