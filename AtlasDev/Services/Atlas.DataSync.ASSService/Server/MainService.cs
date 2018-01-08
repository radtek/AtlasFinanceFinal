/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty) Ltd.
 * 
 *  Description:
 *  ------------------
 *    The main TopShelf service
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;

using Atlas.Common.Interface;
using ASSServer.WCF.DI;
using ASSServer.Shared;
using Atlas.Cache.Interfaces;


namespace ASSServer
{
  public class MainService
  {
    public MainService(ILogging log, ICacheServer cache, IConfigSettings config,
      IDataSyncServiceHost dataSync, IDataFileServiceHost dataFile,
      IAdminServiceHost admin, IDataRequestServiceHost dataRequest)
    {
      _log = log;
      _cache = cache;
      _config = config;
      _dataSyncServiceHost = dataSync;
      _dataFileServiceHost = dataFile;
      _adminServiceHost = admin;
      _dataRequestServiceHost = dataRequest;

      Cache.DataCache.Init(cache, log);      
    }


    #region Public methods

    public bool Start()
    {
      try
      {
        #region Start WCF services

        _log.Information("Starting WCF services");
        try
        {
          _dataFileServiceHost.Open();
          _dataFileServiceHost.LogEndpoints(_log);
        }
        catch (Exception err)
        {
          _dataFileServiceHost = null;
          _log.Fatal(err, "Failed to load 'DataRequestServer' WCF service");
        }

        try
        {
          _dataSyncServiceHost.Open();
          _dataSyncServiceHost.LogEndpoints(_log);
        }
        catch (Exception err)
        {
          _dataSyncServiceHost = null;
          _log.Fatal(err, "Failed to load 'DataSyncServer' WCF service");
        }

        try
        {
          _dataRequestServiceHost.Open();
          _dataRequestServiceHost.LogEndpoints(_log);
        }
        catch (Exception err)
        {
          _dataRequestServiceHost = null;
          _log.Fatal(err, "Failed to load 'DataRequest' WCF service'");
        }

        try
        {
          _adminServiceHost.Open();
          _adminServiceHost.LogEndpoints(_log);
        }
        catch (Exception err)
        {
          _adminServiceHost = null;
          _log.Fatal(err, "Failed to load 'AdminServer' WCF service'");
        }
        #endregion

        return true;
      }
      catch (Exception err)
      {
        _log.Error(err, "Start()");
        return false;
      }
    }


    public bool Stop()
    {
      _log.Information("Service stopping...");

      try
      {
        #region Close WCF Services
        if (_dataFileServiceHost != null)
        {
          _dataFileServiceHost.Close();
          _dataFileServiceHost = null;
        }

        if (_dataSyncServiceHost != null)
        {
          _dataSyncServiceHost.Close();
          _dataSyncServiceHost = null;
        }

        if (_adminServiceHost != null)
        {
          _adminServiceHost.Close();
          _adminServiceHost = null;
        }

        if (_dataRequestServiceHost != null)
        {
          _dataRequestServiceHost.Close();
          _dataRequestServiceHost = null;
        }
        #endregion

        TempFiles.DeleteFilesOlderThan(_log, 0);
      }
      catch
      {
      }
      _log.Information("Service stopped");

      return true;
    }

    #endregion


    #region Private vars

    private readonly ILogging _log;
    private IDataSyncServiceHost _dataSyncServiceHost;
    private IDataFileServiceHost _dataFileServiceHost;
    private IAdminServiceHost _adminServiceHost;
    private IDataRequestServiceHost _dataRequestServiceHost;
    private readonly ICacheServer _cache;
    private readonly IConfigSettings _config;

    #endregion

  }
}
