using Atlas.Online.Node.Core;
using Atlas.Common.ExceptionBase;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Node.CreditNode.CreditServer;
using MassTransit;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.Linq;
using Atlas.Common.Extensions;
using Atlas.Enumerators;
using DevExpress.Xpo;
using Atlas.Online.Data;
using Atlas.RabbitMQ.Messages.Online;
using Magnum;
using Atlas.RabbitMQ.Messages.Notification;
using Atlas.RabbitMQ.Messages.Credit;
using System.Collections.Generic;
using System.Configuration;
using Atlas.Online.Node.CreditNode.OrchestrationService;
using System.Threading.Tasks;
using Atlas.Common.Utils;
using Atlas.Online.Node.CreditNode.EasyNetQ;

namespace Atlas.Online.Node.CreditNode
{
  public sealed class CreditServiceNode : AbstractService<CreditServiceNode, CreditMessage, AffordabilityMessage>
  {
    private IKernel _kernel;
    private static object obj = new object();

    public CreditServiceNode(ILogger ilogger, IKernel kernel)
      : base(ilogger)
    {
      this._kernel = kernel;
    }

    public override void Start()
    {
      base.Start();

      var atlasOnlineServiceBus = _kernel.Get<AtlasOnlineServiceBus>();

      // Setup subscription handler for specific message type
      atlasOnlineServiceBus.GetServiceBus().Subscribe<CreditMessage>("queue_CreditMessage", Handle);

      ServiceLocator.SetServiceLocator(_kernel);
        }

    public override void Stop()
    {
      base.Stop();
    }

    public override void Publish(AffordabilityMessage message)
    {
      ServiceLocator.Get<AtlasOnlineServiceBus>()
                  .GetServiceBus().Publish(message);
    }

    public override void Handle(CreditMessage message)
    {
      try
      {
        using (_kernel.BeginBlock())
        {
          using (var uow = new UnitOfWork())
          {
            var client = new XPQuery<Client>(uow).FirstOrDefault(c => c.ClientId == message.ClientId);

            new CreditServerClient("CreditServer.NET").Using((svc) =>
            {
              new OrchestrationService.OrchestrationServiceClient("OrchestrationService.NET").Using((cli) =>
             {
               var person = cli.GetByIdNo(client.IDNumber);

               if (person.AddressDetails == null)
                 throw new NoAddressException(string.Format("Failed Credit Enquiry - No Addresses found -  person id: {0}, messageId:{1}", person.PersonId, message.MessageId));

               var addressResidentials = person.AddressDetails.FirstOrDefault(o => o.AddressType.Type == General.AddressType.Residential);

               if (addressResidentials == null)
                 throw new NoAddressException(string.Format("Failed Credit Enquiry - No residential Address found -  person id: {0}, messageId:{1}", person.PersonId, message.MessageId));

               if (person.Contacts == null)
                 throw new Exception(string.Format("Failed Credit Enquiry - No contacts found -  person id: {0}, messageId:{1}", person.PersonId, message.MessageId));

               var homeNo = person.Contacts.FirstOrDefault(o => o.ContactType.Type == General.ContactType.TelNoHome);

               if (homeNo == null)
                 throw new Exception(string.Format("Failed Credit Enquiry - No Home No found -  person id: {0}, messageId:{1}", person.PersonId, message.MessageId));

               var workNo = person.Employer.Contacts.FirstOrDefault(o => o.ContactType.Type == General.ContactType.TelNoWork);

               if (workNo == null)
                 throw new Exception(string.Format("Failed Credit Enquiry - No Work No found -  person id: {0}, messageId:{1}", person.PersonId, message.MessageId));


               var cellNo = person.Contacts.FirstOrDefault(o => o.ContactType.Type == General.ContactType.CellNo);

               if (cellNo == null)
                 throw new Exception(string.Format("Failed Credit Enquiry - No Cell No found -  person id: {0}, messageId:{1}", person.PersonId, message.MessageId));

               _logger.Info(string.Format(":: Start Credit Enquiry - PersonId {0} MessageId {1}", person.PersonId, message.MessageId));

               CreditResponse creditResponse = null;
               long? routeHistoryId = null;

               //creditResponse.Decision = Account.AccountStatus.Approved;
               Application application = new XPQuery<Application>(uow).FirstOrDefault(a => a.ApplicationId == message.ApplicationId);

               creditResponse = svc.Enquiry(message.AccountId, person.Firstname, person.Lastname, person.IdNum, person.Gender, person.DateOfBirth, addressResidentials.Line1,
                                               addressResidentials.Line2, addressResidentials.Line3, addressResidentials.Line4, addressResidentials.PostalCode,
                                               homeNo.Value.Substring(0, 3), homeNo.Value.Substring(3, (homeNo.Value.Length - 3)), workNo.Value.Substring(0, 3),
                                               workNo.Value.Substring(3, (workNo.Value.Length - 3)), cellNo.Value, false, false, "AtlasOnline");
               
               _logger.Info(string.Format(":: End Credit Enquiry - PersonId {0} MessageId {1} AccountId {2}", person.PersonId, message.MessageId, message.AccountId));


               if (creditResponse != null)
               {
                 // TODO: CHECK_BYPASS Override check bypass here
                 var bypassEndpoint = ConfigurationManager.AppSettings["bypass-endpoint"];
                 if (!string.IsNullOrWhiteSpace(bypassEndpoint))
                 {
                   var http = new Http($"{bypassEndpoint}/GetCreditCheck") { Type = Http.PostTypeEnum.Get, TimeOut = 1000 };
                   var bypassResult = http.Post();
                   bool creditCheckBypass;
                   bool.TryParse(bypassResult, out creditCheckBypass);
                   http = null;
                   if (creditCheckBypass)
                   {
                     creditResponse.Decision=Account.AccountStatus.Approved;
                   }
                 }

                 string compiled = string.Empty;
                 Dictionary<string, string> dict = new Dictionary<string, string>();

                 switch (creditResponse.Decision)
                 {
                   case Account.AccountStatus.Technical_Error:
                     // Set the new source of the message
                     message.Source = NodeType.Nodes.CreditVerification;
                     message.MessageId = message.MessageId;

                     // Set Destination of message
                     message.Destination = NodeType.Nodes.Sink;

                     // Add route history.
                     routeHistoryId = this.AddRouteHistory(message);

                     _logger.Warn(string.Format(":: Technical Error - Enquiry [0] Client [1]", message.AccountId, message.ClientId));

                     ServiceLocator.Get<AtlasOnlineServiceBus>()
                  .GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
                     {
                       ActionDate = DateTime.Now,
                       Body = "",
                       CreatedAt = DateTime.Now,
                       From = "noreply@atlasonline.co.za",
                       IsHTML = true,
                       Priority = Notification.NotificationPriority.High,
                       Subject = string.Format("Technical Error Credit Enquiry for Application {0}", application.AccountNo),
                       To = "lee@atcorp.co.za"
                     });

                     new AccountServer.AccountServerClient().Using((pri) =>
                     {
                       pri.UpdateAccountStatus(message.AccountId, Account.AccountStatus.Technical_Error, null, null);
                     });

                     Application.UpdateStatus(uow, message.ApplicationId, Account.AccountStatus.Technical_Error);
                     uow.CommitChanges();
                     break;
                   case Account.AccountStatus.Declined:
                     // Set the new source of the message
                     message.Source = NodeType.Nodes.CreditVerification;
                     message.MessageId = message.MessageId;

                     // Set Destination of message
                     message.Destination = NodeType.Nodes.Sink;

                     // Add route history.
                     routeHistoryId = this.AddRouteHistory(message);

                     /***** WRITE TO NOTIFICATION NODE ON DECLINE ******/
                     _logger.Warn(string.Format(":: Credit Score [{0}] Status [{1}] PersonId [{2}] MessageId [{3}] AccountId [{4}] ", creditResponse.Score, creditResponse.Decision.ToStringEnum(), person.PersonId, message.MessageId, message.AccountId));

                     dict.Clear();

                     dict.Add("{Name}", application.Client.Firstname);
                     dict.Add("{Surname}", application.Client.Surname);
                     dict.Add("{AccountNo}", application.AccountNo);

                     new OrchestrationServiceClient("OrchestrationService.NET").Using(pri =>
                     {
                       compiled = pri.GetCompiledTemplate(Notification.NotificationTemplate.Declined_CreditRisk, dict);
                     });

                     ServiceLocator.Get<AtlasOnlineServiceBus>()
                  .GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
                     {
                       ActionDate = DateTime.Now,
                       Body = compiled,
                       CreatedAt = DateTime.Now,
                       From = "noreply@atlasonline.co.za",
                       IsHTML = true,
                       Priority = Notification.NotificationPriority.High,
                       Subject = string.Format("Your Atlas Online application [{0}] has been Declined", application.AccountNo),
                       To = new XPQuery<UserProfile>(uow).FirstOrDefault(p => p.UserId == application.Client.UserId).Email
                     });

                     Application.UpdateStatus(uow, message.ApplicationId, creditResponse.Decision);

                     uow.CommitChanges();
                     break;
                   case Account.AccountStatus.Review:
                     // Set the new source of the message
                     message.Source = NodeType.Nodes.CreditVerification;
                     message.MessageId = message.MessageId;

                     // Set Destination of message
                     message.Destination = NodeType.Nodes.Sink;

                     // Add route history.
                     routeHistoryId = this.AddRouteHistory(message);

                     /***** WRITE TO NOTIFICATION NODE ON DECLINE ******/
                     _logger.Warn(string.Format(":: Credit Score [{0}] Status [{1}] PersonId [{2}] MessageId [{3}] AccountId [{4}] ", creditResponse.Score, creditResponse.Decision.ToStringEnum(), person.PersonId, message.MessageId, message.AccountId));

                     dict.Clear();

                     dict.Add("{Name}", application.Client.Firstname);
                     dict.Add("{Surname}", application.Client.Surname);
                     dict.Add("{AccountNo}", application.AccountNo);

                     new OrchestrationServiceClient("OrchestrationService.NET").Using(pri =>
                     {
                       compiled = pri.GetCompiledTemplate(Notification.NotificationTemplate.Account_Review, dict);
                     });


                     ServiceLocator.Get<AtlasOnlineServiceBus>()
                  .GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
                     {
                       ActionDate = DateTime.Now,
                       Body = compiled,
                       CreatedAt = DateTime.Now,
                       From = "noreply@atlasonline.co.za",
                       IsHTML = true,
                       Priority = Notification.NotificationPriority.High,
                       Subject = string.Format("Your Atlas Online application [{0}] has been sent for Review", application.AccountNo),
                       To = new XPQuery<UserProfile>(uow).FirstOrDefault(p => p.UserId == application.Client.UserId).Email
                     });

                     Application.UpdateStatus(uow, message.ApplicationId, creditResponse.Decision);

                     uow.CommitChanges();
                     break;
                   case Account.AccountStatus.Approved:
                     // Set the new source of the message
                     message.Source = NodeType.Nodes.CreditVerification;
                     message.MessageId = message.MessageId;

                     // Set Destination of message
                     message.Destination = NodeType.Nodes.Affordability;

                     // Add route history.
                     routeHistoryId = this.AddRouteHistory(message);

                     this.Publish(new AffordabilityMessage(message.CorrelationId)
                     {
                       AccountId = message.AccountId,
                       ApplicationId = message.ApplicationId,
                       ClientId = message.ClientId,
                       CreatedAt = DateTime.Now,
                       RouteHistoryId = routeHistoryId,
                       MessageId = message.MessageId,
                       Source = message.Source,
                       Destination = message.Destination
                     });
                     _logger.Info(string.Format(":: Credit Score [{0}] Status [{1}] PersonId {2} MessageId {3} AccountId {4}", creditResponse.Score, creditResponse.Decision.ToStringEnum(), person.PersonId, message.MessageId, message.AccountId));
                     break;
                 }
               }
               else
               {
                 _logger.Fatal(string.Format(":: Timeout Occurred - Account [0] Client {1}", message.AccountId, message.ClientId));
                 // Set the new source of the message
                 message.Source = NodeType.Nodes.CreditVerification;
                 message.MessageId = message.MessageId;
                 // Set Destination of message
                 message.Destination = NodeType.Nodes.Sink;
                 // Add route history.
                 routeHistoryId = this.AddRouteHistory(message);

                 ServiceLocator.Get<AtlasOnlineServiceBus>()
                  .GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
                 {
                   ActionDate = DateTime.Now,
                   Body = "",
                   CreatedAt = DateTime.Now,
                   From = "noreply@atlasonline.co.za",
                   IsHTML = true,
                   Priority = Notification.NotificationPriority.High,
                   Subject = string.Format("Technical Error Credit Enquiry for Application {0}", application.AccountNo),
                   To = "lee@atcorp.co.za"
                 });

                 new AccountServer.AccountServerClient("AccountServer.NET").Using((pri) =>
                 {
                   pri.UpdateAccountStatus(message.AccountId, Account.AccountStatus.Technical_Error, null, null);
                 });

                 Application.UpdateStatus(uow, message.ApplicationId, Account.AccountStatus.Technical_Error);
                 uow.CommitChanges();
               }
             });
            });
          }
        }
      }
      catch (Exception ex)
      {
        _logger.Fatal(string.Format("Failed Credit Enquiry - messageId:{0}", message.MessageId));
        _logger.Fatal(string.Format("Stack {0} - Message {1}", ex.StackTrace.ToString(), ex.Message));
      }
    }

    public override long AddRouteHistory(dynamic message)
    {
      return base.AddRouteHistory((CreditMessage)message);
    }
  }
}