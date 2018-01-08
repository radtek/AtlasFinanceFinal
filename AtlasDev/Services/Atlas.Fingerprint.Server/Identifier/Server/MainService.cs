using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

using EasyNetQ;
using Topshelf;

using Atlas.FP.Identifier.ThreadSafe;
using Atlas.FP.Identifier.EasyNetQ;
using Atlas.FP.Identifier.SDK.Utils;
using Atlas.FP.Identifier.MessageTypes.RequestResponse;
using Atlas.FP.Identifier.MessageTypes.PubSub;
using Atlas.Common.Interface;


namespace Atlas.FP.Identifier
{
  internal class MainService
  {
    public MainService(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    internal bool Start(HostControl hc)
    {
      try
      {
        #region Start EasyNetQ ...
        _log.Information("Starting RabbitMQ bus...");
        var address = ConfigurationManager.AppSettings["fp-rabbitmq-address"];
        var virtualHost = ConfigurationManager.AppSettings["fp-rabbitmq-virtualhost"];
        var userName = ConfigurationManager.AppSettings["fp-rabbitmq-username"];
        var password = ConfigurationManager.AppSettings["fp-rabbitmq-password"];

        var connectionString = string.Format("host={0};virtualHost={1};username={2};password={3};persistentMessages=false;product=fp.identifier;timeout=20;requestedHeartbeat=120", 
          address, virtualHost, userName, password);
        _bus = RabbitHutch.CreateBus(connectionString);

        // Star (pub)sub here, so we never miss an event while loading templates. Subscription name must be unique, else will round-robin/compete- use machine name
        _bus.Subscribe<FPNewTemplates>(string.Format("New_{0}", Environment.MachineName), notification => { FPThreadSafe.AddNewFingerprint(_log, notification.Templates); });

        // Subscription name must be unique, else will round-robin/compete- use machine name
        _bus.Subscribe<FPDeleteTemplates>(string.Format("Del_{0}", Environment.MachineName), notification => { FPThreadSafe.DeleteFingerprint(_log, notification.PersonId); });
        #endregion

        // Load templates        
        hc.RequestAdditionalTime(TimeSpan.FromSeconds(30));
        FPThreadSafe.Initialize(_log, _config, () => { hc.RequestAdditionalTime(TimeSpan.FromSeconds(30)); });

        // Create threadpool to process incoming requests
        IBUtils.Initialize();

        #region EasyNetQ responses to requests
        _bus.Respond<GetFingersRequest, GetFingersResponse>(request => IBConsumer.GetFingers(request));
        _bus.Respond<GetTemplatesForRequest, GetTemplatesForResponse>(request => IBConsumer.GetTemplatesFor(_log, request));
        _bus.Respond<CheckAnyCImagesMatchRequest, CheckAnyCImagesMatchResponse>(request => IBConsumer.CheckCImageMatch(_log, request));
        _bus.Respond<CheckAnyTemplatesMatchRequest, CheckAnyTemplatesMatchResponse>(request => IBConsumer.CheckTemplatesMatch(_log, request));
        _bus.Respond<CreateTemplateRequest, CreateTemplateResponse>(request => IBConsumer.CreateTemplate(request));
        // This process will take a while and will block the EasyNetQ message processing queue- must be async
        _bus.RespondAsync<IdentifyFromImageRequest, IdentifyFromImageResponse>(request => 
          Task.Run(() => { return IBConsumer.IdentifyFromImage(_log, request); }));
        #endregion

        // Process pub-sub template deletes/adds
        _handleQueued = new Timer((x) => { FPThreadSafe.ProcessPendingUpdates(_log); }, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5)); // what is min. time between enroll and first verify?

        _log.Information("Service started");
      }
      catch (Exception err)
      {
        _log.Error(err, "Start()");
        return false;
      }

      return true;
    }


    internal bool Stop()
    {
      try
      {
        _log.Information("Stopping bus...");

        if (_bus != null)
        {
          _bus.Dispose();
          _bus = null;
        }

        _log.Information("Bus stopped");
      }
      catch (Exception err)
      {
        _log.Error(err, "Start()");
      }

      return true;
    }


    /// <summary>
    /// EasyNetQ bus control
    /// </summary>
    IBus _bus;


    /// <summary>
    /// Logging
    /// </summary>
    private readonly ILogging _log;

    /// <summary>
    /// Timer to process pending pubsub notifications
    /// </summary>
    private Timer _handleQueued;
    private readonly IConfigSettings _config;
  }
}
