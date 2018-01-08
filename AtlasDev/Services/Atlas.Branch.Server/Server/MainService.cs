/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013-2016 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Main TopShelf service code
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     13 June 2013 - Created
 *  
 *     15 August 2013- Added wyUpdate
 * 
 *     7 March 2014  - Clean-up and add additional tasks
 * 
 *     Sept 2014     - Started adding basic FTP server for scanner uploads & barcode detection
 *     
 *     July 2016     - Switched from binary DataSet/DataTable serialization to fastJson -> 
 *                     the serialization size is much smaller, much faster to serialize/deserialize! 
 *                     (Tried fastBinaryJson, but it's bigger!)
 * 
 *     
 *  Comments:
 *  ------------------
 *   
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using ASSSyncClient.QuartzTasks;
using ASSSyncClient.Utils;
using ASSSyncClient.Utils.Settings;              // Don't remove when in debug
using Atlas.DataSync.WCF.Client.ClientProxies;
using Atlas.DataSync.WCF.Interface;              // Don't remove when in debug
using Quartz;
using Quartz.Impl;                               // Don't remove when in debug
using Serilog;


namespace ASSSyncClient
{
  public class MainService
  {
    #region Public methods

    /// <summary>
    /// Called on service start- NOTE: Don't take too long, else service manager will fail to start this service
    /// </summary>
    public bool Start(string pSqlServiceName, Topshelf.HostControl control)
    {
      try
      {
        #region Ensure config file contains necessary configuration
        var legacyBranchNum = ConfigurationManager.AppSettings["legacyBranchNum"];
        if (legacyBranchNum == null)
        {
          var error = new Exception("Config file is missing critical setting- 'legacyBranchNum'");
          _log.Fatal(error, "Config file is missing 'legacyBranchNum'");
          return false;
        }
        #endregion

        #region Wait for a non-loopback/ IP addresses... sometimes we only get the 192.168... IP Addresses after a while...?
        var attempts = 0;
        while (attempts++ < 10 && !NetworkOperational())
        {
          _log.Warning("Start- Waiting for network interface... {Attempts}", attempts);
          control.RequestAdditionalTime(TimeSpan.FromSeconds(6));
          Thread.Sleep(5000);
        }
        #endregion

        #region Try sync local time
        var success = false;
        attempts = 0;
        do
        {
          try
          {
            DateTime serverDateTime;
            using (var client = new DataSyncDataClient(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15)))
            {
              serverDateTime = client.GetServerDateTime();
            }

            if (serverDateTime > DateTime.MinValue)
            {
              _log.Information("Successfully got server date: {ServerDate}. Local: {LocalDate}", serverDateTime, DateTime.Now);
              ASSSyncClient.API.Windows.NativeMethods.SetLocalTime(serverDateTime);
              success = true;
            }
          }
          catch (Exception err)
          {
            _log.Error(err, "Start- Sync time");
            control.RequestAdditionalTime(TimeSpan.FromSeconds(20));
          }
        }
        while (!success && ++attempts < 3);
        #endregion
                
        success = false;
        attempts = 0;
#if !DEBUG
        #region Are we registered with the central server and is this machine authorised to access ASS server sync services?
        while (!success && attempts++ < 3)
        {
          PingResult allowed = null;
          try
          {
            using (var client = new DataSyncDataClient(openTimeout: TimeSpan.FromSeconds(10), sendTimeout: TimeSpan.FromSeconds(15)))
            {
              allowed = client.Ping(ASSSyncClient.Utils.WCF.SyncSourceRequest.CreateSourceRequest());
            }
          }
          catch (Exception err)
          {
            control.RequestAdditionalTime(TimeSpan.FromSeconds(20));
            _log.Warning(err, "Ping");
            Thread.Sleep(1000);
          }

          if (allowed == null)
          {
            var error = new Exception("Ping() returned an empty result set");
            _log.Error(error, null);
          }
          else if (!allowed.MachineAuthorised)
          {
            _log.Fatal(new Exception("Machine not authorised for DataSync services"), "{ErrorMessage}", allowed.ErrorMessage);
            return false;
          }
          else
          {
            success = true;
          }
        }
        #endregion

        #region Check if we can obtain PostgreSQL login credentials (either cached in the Registry or the ASS Data Sync Server)
        try
        {
          // This will invoke the static constructor and try obtain the settings
          if (AppSettings.A == "")
          {
          }
        }
        catch (Exception err)
        {
          _log.Fatal(err, "Settings");

          // Critical- try upload error
          LogEvents.Log(DateTime.Now, "MainService.Start- Get settings", err.Message, 10);
          new LogEventsToServer().Execute(null);
          return false;
        }
        #endregion
#endif

#if !DEBUG
        #region Start Quartz tasks
        _log.Information("Starting scheduler engine");
        // Construct standard scheduler factory
        _schedFact = new StdSchedulerFactory(/*props*/);

        // Get a scheduler
        var sched = _schedFact.GetScheduler();

        var random = new Random();
        
        #region Quartz Task to update local database DDL to match server requirements
        try
        {
          // Run now (once)
          var updateLocalDBVerJobNow = JobBuilder.Create<DBUpdateStrucVer>()
            .WithIdentity("UpdateLocalDBVerJobNow", "General").Build();
          var updatelocalDBNowSched = TriggerBuilder.Create()
            .WithIdentity("UpdatelocalDBNowSched", "General")
            .WithSimpleSchedule(s => s
              .WithRepeatCount(0)
              .WithMisfireHandlingInstructionIgnoreMisfires())
            .StartNow()
            .Build();
          sched.ScheduleJob(updateLocalDBVerJobNow, updatelocalDBNowSched);

          // Check for master table structure updates every 10-15 minutes, between 05:00-23:00, Mon-Sat
          var updateLocalDBVerJob = JobBuilder.Create<DBUpdateStrucVer>()
            .WithIdentity("UpdateLocalDBVerJob", "General").Build();
          var updateLocalDBVerSched = (ICronTrigger)TriggerBuilder.Create()
            .WithIdentity("UpdateLocalDBVerSched", "General")
            .WithCronSchedule(string.Format("{0} 0/{1} 5-23 ? * MON-SUN", random.Next(5) + 10, random.Next(10) + 5),
              s => s.WithMisfireHandlingInstructionFireAndProceed())
            .Build();
          sched.ScheduleJob(updateLocalDBVerJob, updateLocalDBVerSched);
        }
        catch (Exception err)
        {
          _log.Error(err, "Quartz.net: Error scheduling local DDL update");
          LogEvents.Log(DateTime.Now, "DBUpdateStrucVer Schedule", err.Message, 5);
        }
        #endregion

        #region Quartz Task to upload local changes made to server every 1-2 minutes
        try
        {
          // Run 06:xx-23:59, Mon-Sat, every 2 minutes
          var uploadChangesJob = JobBuilder.Create<UploadChanges>().WithIdentity("UploadChanges", "General").Build();
          var uploadChangesSchedule = (ICronTrigger)TriggerBuilder.Create()
            .WithIdentity("UploadChanges", "General")
            .WithCronSchedule(string.Format("{0} {1}/2 0-23 ? * MON-SUN", random.Next(60), random.Next(2)),
              s => s.WithMisfireHandlingInstructionDoNothing())
            .Build();

          sched.ScheduleJob(uploadChangesJob, uploadChangesSchedule);
        }
        catch (Exception err)
        {
          _log.Error(err, "Quartz.net: Error scheduling the upload database changes task");
          LogEvents.Log(DateTime.Now, "UploadChanges Schedule", err.Message, 5);
        }
        #endregion

        #region Quartz Task to sync local 'live lookup' tables (i.e. ASSTMAST) with server changes, 07:-18:59, Mon-Sat, every 3-6 minutes
        try
        {
          var updateLocalLiveTablesTask = JobBuilder.Create<UpdateLocalMasterTables>()
            .WithIdentity("UpdateLocalLiveTablesTask", "General").Build();
          var updateLocalLiveTablesSched = (ICronTrigger)TriggerBuilder.Create()
            .WithIdentity("UpdateLocalLiveTablesSched", "General")
            .WithCronSchedule(string.Format("{0} 0/{1} 7-22 ? * MON-SAT", random.Next(60), random.Next(3) + 3),
              s => s.WithMisfireHandlingInstructionDoNothing())
            .Build();
          sched.ScheduleJob(updateLocalLiveTablesTask, updateLocalLiveTablesSched);
        }
        catch (Exception err)
        {
          _log.Error(err, "Quartz.net: Error scheduling update of local live tables");
          LogEvents.Log(DateTime.Now, "UpdateLocalLiveTables Schedule", err.Message, 5);
        }
        #endregion

        #region Quartz Task to upload local server logging events to server every 3-5 minutes
        try
        {
          var logEventsToServerTask = JobBuilder.Create<LogEventsToServer>()
            .WithIdentity("LogEventsToServerTask", "General").Build();
          var logEventsToServerSched = (ICronTrigger)TriggerBuilder.Create()
            .WithIdentity("LogEventsToServer", "General")
            .WithCronSchedule(string.Format("{0} 0/{1} 6-22 ? * MON-SAT", random.Next(60), random.Next(3) + 2),
              s => s.WithMisfireHandlingInstructionDoNothing())
            .Build();
          sched.ScheduleJob(logEventsToServerTask, logEventsToServerSched);
        }
        catch (Exception err)
        {
          _log.Error(err, "Quartz.net: Error scheduling logging of local events of local live tables");
        }

        #endregion

        #region Quartz Task to sync local time with server time, now and every 4 hours
        try
        {
          var syncTimeEvery4HoursTask = JobBuilder.Create<SyncTimeWithServer>()
            .WithIdentity("SyncTimeEvery4HoursJob", "General").Build();

          var syncTimeEvery4HoursSched = (ICronTrigger)TriggerBuilder.Create()
            .WithIdentity("SyncTimeEvery4HoursSched", "General")
            .WithCronSchedule(string.Format("{0} {1} 6-19/4 ? * MON-SAT", random.Next(60), random.Next(40) + 3),
              s => s.WithMisfireHandlingInstructionFireAndProceed())
              .StartAt(DateTime.UtcNow.Subtract(TimeSpan.FromHours(4))) // Force misfire
            .Build();
          sched.ScheduleJob(syncTimeEvery4HoursTask, syncTimeEvery4HoursSched);
        }
        catch (Exception err)
        {
          _log.Error(err, "Quartz.net: Error scheduling logging of local events of local live tables");
        }
        #endregion

        #region Quartz Task to perform daily PostgreSQL backup (a BZIP2 compressed PostgreSQL dump) in 30 minutes' time and at 04:00
        var backupDatabaseJobIn10MinJob = JobBuilder.Create<BackupDatabase>()
            .WithIdentity("BackupDatabaseIn30Min", "General").Build();
        var backupDatabaseJobIn10MinSched = TriggerBuilder.Create()
          .WithIdentity("BackupDatabaseIn30Min", "General")
          .WithSimpleSchedule(s => s
            .WithRepeatCount(0)
            .WithMisfireHandlingInstructionIgnoreMisfires())
#if DEBUG
          .StartAt(DateTime.UtcNow.AddMinutes(1))
#else
          .StartAt(DateTime.UtcNow.AddMinutes(30))
#endif
          .Build();
        sched.ScheduleJob(backupDatabaseJobIn10MinJob, backupDatabaseJobIn10MinSched);

        var backupDatabaseDailyJob = JobBuilder.Create<BackupDatabase>()
            .WithIdentity("BackupDatabaseDaily", "General").Build();
        var backupDatabaseDailySched = (ICronTrigger)TriggerBuilder.Create()
            .WithIdentity("BackupDatabaseDaily", "General")
            .WithCronSchedule("0 0 4 ? * MON-SUN", s => s.WithMisfireHandlingInstructionDoNothing())
            .Build();
        sched.ScheduleJob(backupDatabaseDailyJob, backupDatabaseDailySched);
        #endregion

        #region Quartz Task to check for wyBuild updates to this software every 1 hour
        var checkForAppUpdatesJob = JobBuilder.Create<CheckForAppUpdates>()
            .WithIdentity("CheckForAppUpdates", "General").Build();
        checkForAppUpdatesJob.JobDataMap.Add("serviceName", pSqlServiceName);
        var checkForAppUpdatesSched = (ICronTrigger)TriggerBuilder.Create()
            .WithIdentity("CheckForAppUpdates", "General")
            .WithCronSchedule(string.Format("{0} {1} 0-23/1 ? * MON-SUN", random.Next(45) + 10, random.Next(50) + 3),
              s => s.WithMisfireHandlingInstructionDoNothing())
            .Build();
        sched.ScheduleJob(checkForAppUpdatesJob, checkForAppUpdatesSched);
        #endregion

        #region Quartz task to delete old ASS work files
        var loadAssDirCleanJob = JobBuilder.Create<LocalAssDirCleanUp>()
          .WithIdentity("AssDirClean", "General").Build();
        var loadAssDorCleanSched = (ICronTrigger)TriggerBuilder.Create()
            .WithIdentity("AssDirCleanDaily", "General")
            .WithCronSchedule(string.Format("{0} {1} 8 ? * MON-SUN", random.Next(45) + 2, random.Next(50) + 3),
              s => s.WithMisfireHandlingInstructionFireAndProceed())
            .Build();
        sched.ScheduleJob(loadAssDirCleanJob, loadAssDorCleanSched);
        #endregion

        #region Quartz task to clean up local DB (delete old NLR/CS responses) on a Tuesday afternoon
        var localDbCleanUps = JobBuilder.Create<LocalDbCleanUps>()
          .WithIdentity("LocalDbCleanUps", "General").Build();
        var localDbCleanUpsSched = (ICronTrigger)TriggerBuilder.Create()
            .WithIdentity("LocalDbCleanUpsDaily", "General")
            .WithCronSchedule(string.Format("{0} {1} 16 ? * TUE", random.Next(45) + 2, random.Next(50) + 3),
              s => s.WithMisfireHandlingInstructionDoNothing())
            .Build();
        sched.ScheduleJob(localDbCleanUps, localDbCleanUpsSched);
        #endregion

        #region Quartz task to reboot server every Sat evening, if has been running >48 hours
        var rebootWeeklyTask = JobBuilder.Create<RebootServerWeekly>()
          .WithIdentity("RebootWeekly", "General").Build();
        var rebootWeeklySched = (ICronTrigger)TriggerBuilder.Create()
            .WithIdentity("RebootWeekly", "General")
            .WithCronSchedule("0 0 20 ? * SAT *",
              s => s.WithMisfireHandlingInstructionDoNothing())
            .Build();
        sched.ScheduleJob(rebootWeeklyTask, rebootWeeklySched);
#if DEBUG
        var reboot = new RebootServerWeekly();
        reboot.Execute(null);
#endif
        #endregion

        #region Quartz task to Check that PostgreSQL responding to connections every minute and restart after a number of consecutive failures
        var checkPSQLTask = JobBuilder.Create<PostgresCheckConn>()
          .WithIdentity("CheckPSQLConn", "General").Build();
        var checkPSQLSched = (ICronTrigger)TriggerBuilder.Create()
            .WithIdentity("CheckPSQLConn", "General")
            .WithCronSchedule("0 0/1 0-23 ? * MON-SUN *",
              s => s.WithMisfireHandlingInstructionDoNothing())
            .Build();
        sched.ScheduleJob(checkPSQLTask, checkPSQLSched);
#if DEBUG
        var checkConn = new PostgresCheckConn();
        checkConn.Execute(null);
#endif
        #endregion

        #region Quartz task to check PostgreSQL service running every 1 minute and restart machine if it is not running and service cannot be started
        var checkPSQLServiceTask = JobBuilder.Create<PostgresCheckService>()
          .WithIdentity("CheckPSQLService", "General").Build();
        var checkPSQLServiceSched = (ICronTrigger)TriggerBuilder.Create()
            .WithIdentity("CheckPSQLService", "General")
            .WithCronSchedule("0 0/1 0-23 ? * MON-SUN *",
              s => s.WithMisfireHandlingInstructionDoNothing())
            .Build();
        sched.ScheduleJob(checkPSQLServiceTask, checkPSQLServiceSched);
        #endregion

        #region Quartz task to delete clients >3 yrs Wed afternnon
        var popiDeleteDailyJob = JobBuilder.Create<POPIDelete>()
            .WithIdentity("PopiDeleteDaily", "General").Build();
        var popiDeleteSched = (ICronTrigger)TriggerBuilder.Create()
            .WithIdentity("PopiDeleteDaily", "General")
            .WithCronSchedule(string.Format("{0} {1} 16 ? * WED", random.Next(45) + 2, random.Next(50) + 3), s => s.WithMisfireHandlingInstructionDoNothing())
            .Build();
        sched.ScheduleJob(popiDeleteDailyJob, popiDeleteSched);
        _log.Information("POPI delete next fire: {0:yyyy-MM-dd HH:mm:ss}", popiDeleteSched.GetNextFireTimeUtc());
        #endregion

        #region Quartz task to randomly check for ass updates every 2 hours
        var checkAssVerUpdate = JobBuilder.Create<AssVerUpdate>()
          .WithIdentity("CheckAssVerUpdate", "General").Build();
        var checkAssVerUpdateSched = (ICronTrigger)TriggerBuilder.Create()
            .WithIdentity("AssVerUpdate", "General")
            .WithCronSchedule(string.Format("{0} {1} 0/2 ? * MON-SUN *", random.Next(60), random.Next(60)),
              s => s.WithMisfireHandlingInstructionDoNothing())
            .Build();
        sched.ScheduleJob(checkAssVerUpdate, checkAssVerUpdateSched);
        #endregion

        #region Quartz task to upload the max value of lrep_rec_tracking, so central server can determine how far behind this branch is with uploading changes
        var uploadlrepRecId = JobBuilder.Create<UploadLocalLrepRecId>()
          .WithIdentity("UploadLocalLrepRecId", "General").Build();
        var uploadlrepRecIdSched = (ICronTrigger)TriggerBuilder.Create()
            .WithIdentity("UploadLocalLrepRecId", "General")
            .WithCronSchedule(string.Format("{0} 0/1 6-21  ? * MON-SUN *", random.Next(60)),
              s => s.WithMisfireHandlingInstructionDoNothing())
            .Build();
        sched.ScheduleJob(uploadlrepRecId, uploadlrepRecIdSched);        
        #endregion
        
        #region Quartz task to upload audit logs
        var uploadAuditLogs = JobBuilder.Create<UploadAuditLogs>()
          .WithIdentity("UploadAuditLogs").Build();
        var uploadAuditLogsSched = (ICronTrigger)TriggerBuilder.Create()
          .WithIdentity("UploadAuditLogs", "General")
          .WithCronSchedule(string.Format("{0} 0/{1} 6-20 ? * MON-SUN *", random.Next(60), 5 + random.Next(2) + 2),
          s => s.WithMisfireHandlingInstructionDoNothing())
          .Build();
        sched.ScheduleJob(uploadAuditLogs, uploadAuditLogsSched);
        #endregion

        sched.StartDelayed(TimeSpan.FromMinutes(3)); // Give PSQL/Windows system time to start / stabilize...

        #endregion

#endif
        // Start the FTP server to handle scanner file uploads
        _ftpServer = new Utils.FTP.ScannerFtpServer();

        LogEvents.Log(DateTime.Now, "Start", "Service started successfully", 1);
        _log.Information("Server started successfully");

        return true;
      }
      catch (Exception err)
      {
        _log.Fatal(err, "Service.Start");

        // Critical failure- try upload immediately via WCF
        LogEvents.Log(DateTime.Now, "MainService.Start", err.Message, 10);
        var log = new LogEventsToServer();
        log.Execute(null);

        return false;
      }
    }


    /// <summary>
    /// Called on service stop- close down WCF/Quartz
    /// </summary>
    public void Stop()
    {
      try
      {
#if !DEBUG
        #region Close scheduler
        _log.Information("Quartz scheduler stopping");
        if (_schedFact != null)
        {
          var scheduler = _schedFact.GetScheduler();
          if (scheduler != null)
          {
            scheduler.Shutdown(true);
          }
        }
        _log.Information("Quartz scheduler stopped");
        #endregion
#endif
        //_ftpServer.Stop();
      }
      catch (Exception err)
      {
        _log.Error(err, "Service.Stop");
      }
    }

    #endregion


     #region Private methods

    /// <summary>
    /// Determine if normal LAN is up (excludes loopback and tunnel connections)
    /// </summary>
    /// <returns>true if LAN (non loopback and tunnel) is up, else false</returns>
    private static bool NetworkOperational()
    {
      return NetworkInterface.GetAllNetworkInterfaces()
        .Any(s => (s.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
            s.NetworkInterfaceType != NetworkInterfaceType.Tunnel) &&
            s.OperationalStatus == OperationalStatus.Up);
    }

    #endregion


    #region Private fields

    /// <summary>
    /// Logging
    /// </summary>
    private static readonly ILogger _log = Log.Logger.ForContext<MainService>();

    /// <summary>
    /// Quartz scheduler factory
    /// </summary>
    private ISchedulerFactory _schedFact; // NOTE: do not delete while in debug mode!!


    private static ASSSyncClient.Utils.FTP.ScannerFtpServer _ftpServer;

    #endregion

  }
}
