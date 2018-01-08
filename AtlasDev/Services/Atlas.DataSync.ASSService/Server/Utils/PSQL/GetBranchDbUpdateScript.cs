using System;
using System.Linq;
using System.Collections.Generic;

using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.DataSync.WCF.Interface;
using Atlas.Cache.Interfaces;


namespace ASSServer.Utils.PSQL
{
  public class GetBranchDbUpdateScript
  {
    /// <summary>
    /// Gets PostgeSQL scripts to be run, to convert database from 'dbLocalVersion' version, to latest version
    /// </summary>
    /// <param name="dbLocalVersion">The current database version</param>
    /// <param name="errorMessage">Any error message</param>
    /// <returns>List of VerUpdateScripts to be run, in order</returns>
    public static bool Execute(ICacheServer cache, string dbLocalVersion, out List<VerUpdateScripts> scripts,  out string errorMessage)
    {
      errorMessage = null;
      scripts = new List<VerUpdateScripts>();
      try
      {       
        using (var unitOfWork = new UnitOfWork())
        {
          // Get current script record, for the version they currently are on
          var clientCurrVer = unitOfWork.Query<ASS_DbUpdateScript>().FirstOrDefault(s => s.DbVersion == dbLocalVersion);
          if (clientCurrVer == null)
          {
            errorMessage = string.Format("Specified database version '{0}' is unknown", dbLocalVersion);
            return false;
          }
         
          // Get latest server version
          var serverCurrVer = unitOfWork.Query<ASS_DbUpdateScript>().Select(s => s.DbVersion).OrderByDescending(s => s).First();
          if (serverCurrVer == clientCurrVer.DbVersion)
          {
            return true;
          }

          #region Move through script hierarchy, to the script this branch is supposed to be on
          ASS_DbUpdateScript current = clientCurrVer;
          while (current != null && current.DbVersion != serverCurrVer)
          {
            current = unitOfWork.Query<ASS_DbUpdateScript>().FirstOrDefault(s => s.PreviousVersion == current);
            if (current != null && !string.IsNullOrEmpty(current.UpdateScript))
            {
              scripts.Add(new VerUpdateScripts() { Version = current.DbVersion, SQLScript = current.UpdateScript });
            }
          }
          #endregion
        }
      }
      catch (Exception err)
      {
        errorMessage = err.Message;
        return false;
      }

      return true;
    }

  }
}
