/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     WCF routines used to make data file requests- get branch DBF's/PSQL dump & have server process an uploaded 
 *     DBF ZIP.
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     24 May 2013 - Created
 *     
 * 
 *  Comments:
 *  ------------------
 *     
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.ServiceModel;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;


namespace ASSServer.WCF.Implementation.DataFileRequest
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class DataRequestServer : IDataRequestServer
  {
    public DataRequestServer(ILogging log, IConfigSettings config, ICacheServer cache)
    {
      _log = log;
      _config = config;
      _cache = cache;
    }


    #region Public methods

    /// <summary>
    /// Start process to get DBF files for a branch
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <returns></returns>
    public ProcessStatus StartGetBranchDBFs(SourceRequest sourceRequest)
    {
      return StartGetBranchDBFs_Impl.Execute(_log, _cache, _config, sourceRequest);
    }


    /// <summary>
    /// Start process to get PostgreSQL 'Directory' formatted dump, which is ZIPPED for a branch
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <returns></returns>
    public ProcessStatus StartGetBranchPSQL(SourceRequest sourceRequest)
    {
      return StartGetBranchPSQL_Impl.Execute(_log, _cache, _config, sourceRequest);
    }


    /// <summary>
    /// Get status of process request
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    public ProcessStatus GetProcessStatus(SourceRequest sourceRequest, string transactionId)
    {
      return GetProcessStatus_Impl.Execute(_log, sourceRequest, transactionId);
    }


    /// <summary>
    /// Import a  branch DBF ZIPPED file- this will extract, import and 
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="clientTransactionId"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public ProcessStatus ProcessUploadedBranchZIPDBF(SourceRequest sourceRequest, string clientTransactionId, string fileName)
    {
      return ProcessUploadedBranchZIPDBF_Impl.Execute(_log, _cache, _config, sourceRequest, clientTransactionId, fileName);
    }

    #endregion


    #region Private fields

    private readonly ILogging _log;
    private readonly IConfigSettings _config;
    private readonly ICacheServer _cache;

    #endregion

  }
}