using System;

using GdPicture12;

using Atlas.DocServer.WCF.DI;
using Atlas.Common.Interface;


namespace Atlas.DocServer
{
  class MainService
  {
    public MainService(ILogging log,
      IDocumentAdminServiceHost adminService, IDocumentConvertServiceHost convertService,
      IDocumentGeneratorServiceHost generatorService, IDocumentRecognitionServiceHost recognitionService)
    {
      _log = log;
      _adminService = adminService;
      _convertService = convertService;
      _generatorService = generatorService;
      _recognitionService = recognitionService;
    }


    public bool Start()
    {
      try
      {
        _log.Information("Starting...");

        #region Start WCF services
        _log.Information("Starting WCF services");
        try
        {
          _convertService.Open();
          _convertService.LogEndpoints(_log);
        }
        catch (Exception err)
        {
          _convertService?.Close();
          _log.Fatal(err, "Failed to load 'DocumentConvertServer' WCF service");
        }

        try
        {
          _generatorService.Open();
          _generatorService.LogEndpoints(_log);
        }
        catch (Exception err)
        {
          _generatorService?.Close();
          _log.Fatal(err, "Failed to load 'DocumentGeneratorServer' WCF service");
        }

        try
        {
          _adminService.Open();
          _adminService.LogEndpoints(_log);
        }
        catch (Exception err)
        {
          _adminService?.Close();
          _log.Fatal(err, "Failed to load 'DocumentAdminServer' WCF service");
        }

        try
        {
          _recognitionService.Open();
          _recognitionService.LogEndpoints(_log);
        }
        catch (Exception err)
        {
          _recognitionService.Close();
          _log.Fatal(err, "Failed to load 'DocumentRecognition' WCF service");
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
      try
      {
        _log.Information("Service stopping...");

        #region Close WCF Services
        if (_convertService != null)
        {
          _convertService.Close();
          _log.Information("Document conversion service closed");
        }

        if (_generatorService != null)
        {
          _log.Information("Closing document generation service");
          _generatorService.Close();
          _log.Information("Document generation service closed");
        }

        if (_adminService != null)
        {
          _log.Information("Closing document admin service");
          _adminService.Close();
          _log.Information("Document admin service closed");
        }

        if (_recognitionService != null)
        {
          _log.Information("Closing document recognition service");
          _recognitionService.Close();
          _log.Information("Document recognition service closed");

        }
        #endregion

        _log.Information("Service stopped");

        return true;
      }
      catch (Exception err)
      {
        _log.Error(err, "Stop()");
        throw;
      }
    }


    #region Private vars

    /// <summary>
    /// Administrative interface- add document, get document, add comments, add history, etc.
    /// </summary>
    private readonly IDocumentAdminServiceHost _adminService;

    /// <summary>
    /// Document conversion- convert between document format types
    /// </summary>
    private readonly IDocumentConvertServiceHost _convertService;
        
    /// <summary>
    /// Document type recognition service
    /// </summary>
    private readonly IDocumentRecognitionServiceHost _recognitionService;

    /// <summary>
    /// Document generation server
    /// </summary>
    private readonly IDocumentGeneratorServiceHost _generatorService;

    private readonly ILogging _log;

    #endregion

  }
}
