using Atlas.Online.Node.Core;
using MassTransit;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.Net.Mail;
using Atlas.Common.Extensions;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Common.Utils;
using Atlas.Enumerators;
using DevExpress.Xpo;
using System.Linq;
using Atlas.RabbitMQ.Messages.Online;

namespace Atlas.Online.Node.EmailNode
{
  public sealed class EmailServiceNode : AbstractService<EmailServiceNode, EmailMessage, SinkMessage>
  {
    private IKernel _kernel;

    public EmailServiceNode(ILogger ilogger, IServiceBus ibus, IKernel kernel)
      : base(ilogger, ibus)
    {
      this._kernel = kernel;
    }

    public override void Start()
    {
      base.Start();
      // Setup subscription handler for specific message type
      this._bus.SubscribeHandler<EmailMessage>(Handle);
    }

    public override void Stop()
    {
      base.Stop();
    }

    public override void Publish(SinkMessage message)
    {
      throw new NotImplementedException();
    }

    public override void Handle(EmailMessage message)
    {
      try
      {
        using (_kernel.BeginBlock())
        {
          _logger.Info(string.Format("Received a new message with body {0}", message.CorrelationId.ToString()));

          new EmailServer.EmailServerClient().Using((client) =>
          {
            _logger.Info("Prompting Email Server with email request...");

            EmailServer.EnumeratorsEmailMessageType? emailEnum = null;
            switch (message.MessageType)
            {
              case General.EmailMessageType.Standard:
                emailEnum = EmailServer.EnumeratorsEmailMessageType.Standard;
                break;
              case General.EmailMessageType.Statement:
                emailEnum = EmailServer.EnumeratorsEmailMessageType.Statement;
                break;
              case General.EmailMessageType.Payment_Due:
                emailEnum = EmailServer.EnumeratorsEmailMessageType.Payment_Due;
                break;
              case General.EmailMessageType.Overdue:
                emailEnum = EmailServer.EnumeratorsEmailMessageType.Overdue;
                break;
              case General.EmailMessageType.Legal_Notice:
                emailEnum = EmailServer.EnumeratorsEmailMessageType.Legal_Notice;
                break;
              default:
                break;
            }
            client.Send(message.From, message.To, message.Subject, message.Body, message.IsBodyHTML,
                                                  message.Priority == MailPriority.High ? EmailServer.Priority.High : EmailServer.Priority.Normal,
                                                  (EmailServer.EnumeratorsEmailMessageType)emailEnum, message.ActionDate, message.IsActionDateTriggered);

            _logger.Info("Email message has been queued");

            message.Destination = NodeType.Nodes.Sink;
            this.AddRouteHistory(message);
          });
        }
      }
      catch (Exception ex)
      {
        _logger.Info(string.Format("Failed Credit Enquiry - messageId:{0}", message.MessageId));
        _logger.Fatal(string.Format("Stack {0} - Message {1}", ex.StackTrace.ToString(), ex.Message));
      }
    }

    public override long AddRouteHistory(dynamic message)
    {
      throw new NotImplementedException();     
    }
  }
}