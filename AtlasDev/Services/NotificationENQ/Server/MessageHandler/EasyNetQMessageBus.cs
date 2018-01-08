using System;
using System.Threading.Tasks;

using EasyNetQ;

using Atlas.NotificationENQ.Dto;
using Atlas.Common.Interface;
using NotificationServerENQ.Senders;
using NotificationServerENQ.Db;


namespace NotificationServerENQ.MessageHandler
{
  /// <summary>
  /// EasyNetQ message bus handler
  /// </summary>
  internal class EasyNetQMessageBus : IMessageBusHandler
  {
    public EasyNetQMessageBus(ILogging log, IConfigSettings config, ISmsSender smsSender, IEmailSender emailSender, 
      IDbRepository dbRepos, IMessageInProgressTracker tracker)
    {
      _log = log;
      _config = config;
      _smsSender = smsSender;
      _emailSender = emailSender;
      _dbRepos = dbRepos;
      _tracker = tracker; 
    }


    public void Start()
    {
      var address = _config.GetRabbitMQServerHost();
      var virtualHost = _config.GetRabbitMQVirtualHost();
      var userName = _config.GetRabbitMQServerUsername();
      var password = _config.GetRabbitMQServerPassword();
           
      var connectionString = string.Format("host={0};virtualHost={1};username={2};password={3};timeout=20;requestedHeartbeat=120",
        address, virtualHost, userName, password);

      _log.Information("Starting bus.. {Connection}", connectionString);
      _bus = RabbitHutch.CreateBus(connectionString);
      _log.Information("Bus started");

      _bus.RespondAsync<SendEmailMessageRequest, SendEmailMessageResponse>(request => 
        Task.Run(async () =>
        {
          _log.Information("[MessageBus] Got new e-mail request {@Request}", new { request.From, request.To, request.Subject } );       
          var recId = _dbRepos.AddEmail(request);
          _tracker.SetInProgress(recId, true);
          try
          {
            var sendEmail = await _emailSender.Send(_log, _config, (request)).ConfigureAwait(false);
            _dbRepos.UpdateStatus(recId, sendEmail.Item1, TimeSpan.FromMinutes(30));
          }
          finally
          {
            _tracker.SetInProgress(recId, false);
          }
          return new SendEmailMessageResponse { RecId = recId };
        }));

      _bus.RespondAsync<SendSmsMessageRequest, SendSmsMessageResponse>(request => 
        Task.Run(async () =>
        {
          _log.Information("[MessageBus] Got new SMS request {@Request}", request);
          var recId = _dbRepos.AddSMS(request);
          try
          {
            _tracker.SetInProgress(recId, true);
            var sensSms = await _smsSender.Send(_log, _config, (request)).ConfigureAwait(false);
            _dbRepos.UpdateStatus(recId, sensSms.Item1 > 0, TimeSpan.FromMinutes(15), sensSms.Item1);
          }
          finally
          {
            _tracker.SetInProgress(recId, false);
          }
          return new SendSmsMessageResponse { RecId = recId };
        }));
    }


    public void Stop()
    {
      if (_bus != null)
      {
        _log.Information("Message bus stopping...");
        _bus.Dispose();
        _bus = null;
        _log.Information("Message bus stopped");
      }
    }


    #region Private fields

    private readonly IConfigSettings _config;
    private readonly ILogging _log;
    private readonly ISmsSender _smsSender;
    private readonly IEmailSender _emailSender;
    private readonly IDbRepository _dbRepos;
    private readonly IMessageInProgressTracker _tracker;

    private IBus _bus;

    #endregion

  }
}
