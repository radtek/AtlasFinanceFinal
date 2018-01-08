/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Config Service
 * 
 * 
 *  Author:
 *  ------------------
 *     Fabian Franco-Roldan
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

namespace Atlas.PDF.Server.WCF
{
  #region Using

  using System;
  using System.Timers;
  using System.Diagnostics;
  using System.Net;
  using System.Configuration;
  using System.ServiceModel;
  using System.ServiceModel.Description;
  using DevExpress.Xpo;
  using log4net;
  using log4net.Config;
  using DevExpress.Xpo.DB;
  using System.Threading;
using Atlas.PDF.Server.WCF.Implemenation;


  #endregion

  public class PDFService
  {
    #region Private Members

    private readonly Stopwatch _logIsActive = new Stopwatch();
    private readonly System.Timers.Timer _timer;
    private static readonly ILog _log = LogManager.GetLogger(typeof(PDFService));
    private readonly static Stopwatch _upTime = new Stopwatch();

    private ServiceHost pdfService;

    #endregion
    
    #region Private utility functions

    private static void EnumerateEndpointsActive(ServiceDescription service)
    {
      _log.Info(string.Format("WCF endpoints active: {0}", service.Endpoints.Count));
      foreach (var endpoint in service.Endpoints)
      {
        string info = string.Format("WCF endpoint is active- Address: '{0}', Contract name: '{1}', Binding name: '{2}'",
            endpoint.Address, endpoint.Contract.Name, endpoint.Binding.Name);

        _log.Info(info);
      }
    }

    #endregion

    #region Constrcutor

    public PDFService()
    {
      #region Log4Net

      GlobalContext.Properties["HostName"] = Environment.MachineName;
      var fileVer = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
      GlobalContext.Properties["FileVer"] = fileVer.FileVersion;
      GlobalContext.Properties["ProdVer"] = fileVer.ProductVersion;

      BasicConfigurator.Configure();
      XmlConfigurator.Configure();
      _log.Info("Logging service was created");

      #endregion

      #region Timer to log server responsive

      _timer = new System.Timers.Timer(new TimeSpan(1, 0, 0).TotalMilliseconds) { AutoReset = false };
      _timer.Elapsed += OnTimer;
      _timer.Enabled = true;
      _upTime.Start();

      #endregion


      _logIsActive.Start();
      _log.Info("Service is starting");

    }

    #endregion

    #region Public Methods

    public void OnTimer(object s, EventArgs e)
    {
      _timer.Enabled = false;
      try
      {
        if (_logIsActive.Elapsed > new TimeSpan(1, 0, 0))
        {
          _log.Info(string.Format("ConfigService is active, service uptime: {0:#,#.##}s", _upTime.ElapsedMilliseconds / 1000));
          _logIsActive.Restart();
        }
      }
      finally
      {
        _timer.Enabled = true;
      }
    }

    public void Start()
    {
      // 2 is default
      ServicePointManager.DefaultConnectionLimit = 100;

      #region Start WCF services
    
      try
      {
        pdfService = new ServiceHost(typeof(PDFServer));
        pdfService.Open();
        _log.Info("Successfully loaded 'PDF' service");
        EnumerateEndpointsActive(pdfService.Description);
      }
      catch (Exception err)
      {
        pdfService = null;
        _log.Fatal("Failed to load 'PDF' service", err);
      }

      #endregion

      _timer.Start();
      _log.Info("Service was started");
    }


    public void Stop()
    {
      _timer.Stop();
      _timer.Dispose();


      #region Close WCF services

      _log.Info("Closing WCF services");

      if (pdfService != null)
      {
        pdfService.Close();
        pdfService = null;
        _log.Info("PDF Service Closed");
      }


      _log.Info("WCF services stopped");
    
      #endregion

      _log.Info("Service was stopped");
    }

    #endregion
  }
}
