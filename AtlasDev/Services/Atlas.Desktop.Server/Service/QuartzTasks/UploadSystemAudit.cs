/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Task to upload a hardware and software audit of the local machine
 *     
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-08-22 Created
 *     
 * ----------------------------------------------------------------------------------------------------------------- */

using System;

using Quartz;
using Newtonsoft.Json;
using Serilog;

using Atlas.Fingerprint.WCF.Client;
using Atlas.Fingerprint.WCF.Client.ClientProxies;


namespace AClientSvc.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class UploadSystemAudit : IJob
  {
    /// <summary>
    /// Upload system information/audit
    /// </summary>
    /// <param name="context"></param>    
    public void Execute(IJobExecutionContext context)
    {
      try
      {
        _log.Information("System audit- task starting");
        var si = Atlas.Shared.Audit.AuditUtils.SysInfoAuditUtils.GetSystemInfo();
        var json = JsonConvert.SerializeObject(si, new JsonSerializerSettings() { Formatting = Formatting.None });
        var jsonCompressed = Atlas.Shared.Audit.AuditUtils.ZipUtils.CompressString(json);

        string errorMessage = null;
        using (var client = new FPCommsClient(sendTimeout: TimeSpan.FromSeconds(60), openTimeout: TimeSpan.FromSeconds(10)))
        {
          client.FPC_UploadClientHWSWStatus(SourceRequestUtils.CreateSourceRequest(), jsonCompressed, out errorMessage);
        }

        _log.Information("System audit- task completed");
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute");
      }
    }


    #region Private vars

    // Logging
    private static readonly ILogger _log = Log.ForContext<UploadSystemAudit>();

    #endregion

  }
}
