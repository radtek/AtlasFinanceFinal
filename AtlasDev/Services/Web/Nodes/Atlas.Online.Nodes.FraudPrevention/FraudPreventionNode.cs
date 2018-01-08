using System;
using Atlas.Online.Node.Core;
using MassTransit;
using Ninject.Extensions.Logging;
using Atlas.Online.Node.FraudPreventionNode.FraudServer;
using Ninject;
using System.Linq;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Common.Extensions;
using Magnum;
using Atlas.Enumerators;
using DevExpress.Xpo;
using OrchestrationServer = Atlas.Online.Node.FraudPreventionNode.OrchestrationService;
using AccountServer = Atlas.Online.Node.FraudPreventionNode.AccountServer;
using Atlas.RabbitMQ.Messages.Online;
using Atlas.RabbitMQ.Messages.Notification;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Atlas.Common.Utils;
using Atlas.Online.Node.FraudPrevention.EasyNetQ;
using Atlas.Online.Node.FraudPreventionNode.OrchestrationService;

namespace Atlas.Online.Node.FraudPrevention
{

  public sealed class FraudPreventionServiceNode : AbstractService<FraudPreventionServiceNode, FraudPreventionMessage, SinkMessage>
  {
    private static IKernel _kernel;

    public FraudPreventionServiceNode(ILogger ilogger, IKernel ikernal)
      : base(ilogger)
    {
      _kernel = ikernal;
    }

    public override void Start()
    {
      base.Start();

      var atlasOnlineServiceBus = _kernel.Get<AtlasOnlineServiceBus>();

      // Setup subscription handler for specific message type
      atlasOnlineServiceBus.GetServiceBus().Subscribe<FraudPreventionMessage>("queue_FraudPreventionMessage", Handle);

      ServiceLocator.SetServiceLocator(_kernel);
    }

    public override void Stop()
    {
      base.Stop();
    }

    public override void Publish(SinkMessage message)
    {
      throw new NotImplementedException();
    }

    public override void Handle(FraudPreventionMessage message)
    {
      using (_kernel.BeginBlock())
      {

        if (message.RetryCount == 0)
          message.RetryCount = 1;

        _logger.Info(string.Format(":: Person Lookup - MessageId:{0}, PersonId:{1}", message.MessageId, message.PersonId));

        PER_PersonDTO person = null;


        new OrchestrationServiceClient("OrchestrationService.NET").Using((cli) =>
        {
          person = cli.GetbyPk(message.PersonId);
        });

        if (person == null)
          throw new Exception(string.Format("Unable to locate person in the database - MessageId:{0}, PersonId:{1}", message.MessageId, message.PersonId));

        if (person != null)
        {
          new FraudServerClient("FraudServer.NET").Using((svc) =>
          {
            AddressDTO residentialAddress = person.AddressDetails.FirstOrDefault(_ => _.AddressType.Type == General.AddressType.Residential && _.IsActive);

            BankDetailDTO bankDetail = person.BankDetails.OrderBy(p => p.CreatedDT).FirstOrDefault();

            ContactDTO HomeContact = person.Contacts.FirstOrDefault(_ => _.ContactType.Type == General.ContactType.TelNoHome && _.IsActive);

            ContactDTO WorkContact = person.Employer.Contacts.FirstOrDefault(_ => _.ContactType.Type == General.ContactType.TelNoWork);
            if (WorkContact == null)
            {
              WorkContact = person.Employer.Contacts.FirstOrDefault();
            }

            ContactDTO CellContact = person.Contacts.FirstOrDefault(_ => _.ContactType.Type == General.ContactType.CellNo && _.IsActive);

            CPY_CompanyDTO employer = person.Employer;

            Tuple<string, string> HomeNo = Common.Utils.StringUtils.SplitNo(3, HomeContact.Value);
            Tuple<string, string> WorkNo = WorkContact == null
              ? new Tuple<string, string>("", "")
              : Common.Utils.StringUtils.SplitNo(3, WorkContact.Value);

            _logger.Info(string.Format(":: Enquiry Started - MessageId:{0}, PersonId:{1}", message.MessageId, message.PersonId));

            var fraudEnquiry = svc.GetEnquiryForAccount((long)message.AccountId);
            FraudResult result = null;
            Application application = null;
            long? routeHistoryId = null;
            bool timeOut = false;
            using (var uow = new UnitOfWork())
            {
              if (fraudEnquiry == null)
              {
                try
                {
                  result = svc.FraudEnquiry(message.AccountId, person.IdNum, person.Firstname, person.Lastname, residentialAddress.Line1,
                                        residentialAddress.Line2, residentialAddress.Line3, residentialAddress.Line4,
                                        residentialAddress.PostalCode, residentialAddress.Province.ShortCode, HomeNo.Item1, HomeNo.Item2,
                                        WorkNo.Item1, WorkNo.Item2, CellContact.Value, bankDetail.AccountNum, bankDetail.Bank.Type.ToStringEnum(),
                                        bankDetail.Code, employer.Name);

                  if (result == null)
                    throw new Exception("Technical Error");

                }
                catch (TimeoutException)
                {
                  _logger.Fatal(string.Format(":: Timeout Exception -  MessageId: {0}, PersonId:{1}, AccountId:{2}", message.MessageId, person.PersonId, message.AccountId));
                  timeOut = true;
                }
              }
              else
              {
                result = fraudEnquiry;
              }

              // TODO: CHECK_BYPASS Override check bypass here
              var bypassEndpoint = ConfigurationManager.AppSettings["bypass-endpoint"];
              if (!string.IsNullOrWhiteSpace(bypassEndpoint))
              {
                var http = new Http($"{bypassEndpoint}/GetFraudCheck") { Type = Http.PostTypeEnum.Get, TimeOut = 1000 };
                var bypassResult = http.Post();
                bool fraudCheckBypass;
                bool.TryParse(bypassResult, out fraudCheckBypass);
                http = null;
                if (fraudCheckBypass)
                {
                  result.Status= Account.AccountStatus.Inactive;
                }
              }

              #region Timeout

              if (!timeOut)
              {
                // Set the new source of the message
                message.Source = NodeType.Nodes.FraudPrevention;


                application = new XPQuery<Application>(uow).FirstOrDefault(p => p.ApplicationId == message.ApplicationId);
                if (application == null)
                {
                  _logger.Fatal(string.Format(":: Application Missing Application [{0}] Client [{1}]", message.ApplicationId, message.ClientId));
                  throw new NullReferenceException(string.Format(":: Application Missing Application [{0}] Client [{1}]", message.ApplicationId, message.ClientId));
                }
                _logger.Info(string.Format(":: Enquiry Ended - MessageId:{0}, PersonId:{1}", message.MessageId, message.PersonId));
              }

              #endregion

              #region "Switch"
              switch (result.Status)
              {
                #region Technical Error

                case Account.AccountStatus.Technical_Error:
                  message.RouteHistoryId = routeHistoryId;
                  message.Destination = NodeType.Nodes.FraudPrevention;
                  routeHistoryId = AddRouteHistory(message);

                  _logger.Info(string.Format(":: Technical Error Fraud Error - MessageId:{0}, PersonId:{1}", message.MessageId, message.PersonId));
                  if (message.RetryCount <= 3)
                  {
                    _logger.Info(string.Format(":: Technical Error Fraud Error Retry - MessageId:{0}, PersonId:{1}, RetryCount:{2}", message.MessageId, message.PersonId, message.RetryCount));
                    message.RetryCount++;
                    ServiceLocator.Get<AtlasOnlineServiceBus>()
                  .GetServiceBus().Publish(message);
                  }
                  else
                  {
                    application.Status = Account.AccountStatus.Technical_Error;
                    application.Save();

                    new AccountServer.AccountServerClient("AccountServer.NET").Using((cli) =>
                    {
                      cli.UpdateAccountStatus((long)message.AccountId, Account.AccountStatus.Technical_Error, result.StatusReason, result.SubStatusReason);
                      cli.WorkflowStepUp((long)message.AccountId, null, Workflow.ProcessStep.Technical_Error);
                    });
                  }
                  break;
                #endregion

                #region Inactive
                case Account.AccountStatus.Inactive:
                  // Add route history.
                  message.Destination = NodeType.Nodes.CreditVerification;

                  routeHistoryId = AddRouteHistory(message);

                  _logger.Info(string.Format(":: Passed Fraud - Score {0} MessageId:{1}  PersonId:{2}", result.Rating, message.MessageId, message.PersonId));

                  application.Status = Account.AccountStatus.Inactive;
                  application.Save();

                  ServiceLocator.Get<AtlasOnlineServiceBus>()
                  .GetServiceBus().Publish<CreditMessage>(new CreditMessage(CombGuid.Generate())
                  {
                    AccountId = (long)message.AccountId,
                    ClientId = message.ClientId,
                    CreatedAt = DateTime.Now,
                    RouteHistoryId = routeHistoryId,
                    ApplicationId = message.ApplicationId,
                    Source = NodeType.Nodes.FraudPrevention,
                    Destination = NodeType.Nodes.CreditVerification,
                    MessageId = message.MessageId
                  });
                  break;
                #endregion

                #region Declined
                case Account.AccountStatus.Declined:
                  _logger.Info(string.Format(":: Person Failed Declined - Rating: {0} MessageId: {1} PersonId: {2}", result.Rating, message.MessageId, message.PersonId));
                  routeHistoryId = AddRouteHistory(message);

                  application.Status = Account.AccountStatus.Declined;
                  application.Save();

                  new AccountServer.AccountServerClient("AccountServer.NET").Using((cli) =>
                  {
                    cli.UpdateAccountStatus((long)message.AccountId, Account.AccountStatus.Declined, result.StatusReason, result.SubStatusReason);
                    cli.WorkflowStepUp((long)message.AccountId, null, Workflow.ProcessStep.Fraud);
                    cli.WorkflowComplete((long)message.AccountId);
                  });

                  string compiled = string.Empty;
                  Dictionary<string, string> dict = new Dictionary<string, string>();


                  dict.Add("{Name}", application.Client.Firstname);
                  dict.Add("{Surname}", application.Client.Surname);
                  dict.Add("{AccountNo}", application.AccountNo);

                  new OrchestrationServiceClient("OrchestrationService.NET").Using(clii =>
                  {
                    compiled = clii.GetCompiledTemplate(Notification.NotificationTemplate.Declined_Fraud, dict);
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

                  break;
                #endregion

                #region Review
                case Account.AccountStatus.Review:
                  _logger.Info(string.Format(":: Person Failed Review - Score: {0} MessageId: {1} PersonId: {2}", result.Rating, message.MessageId, message.MessageId));
                  routeHistoryId = AddRouteHistory(message);

                  application.Status = Account.AccountStatus.Review;
                  application.Save();

                  new AccountServer.AccountServerClient("AccountServer.NET").Using((cli) =>
                  {
                    cli.UpdateAccountStatus((long)message.AccountId, Account.AccountStatus.Review, result.StatusReason, result.SubStatusReason);
                    cli.WorkflowStepUp((long)message.AccountId, null, Workflow.ProcessStep.Review);
                  });


                  ServiceLocator.Get<AtlasOnlineServiceBus>()
                  .GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
                  {
                    ActionDate = DateTime.Now,
                    Body = string.Format("Account {0} is under review, to contiue processing account use message id {0}", application.AccountNo),
                    CreatedAt = DateTime.Now,
                    From = "noreply@atlasonline.co.za",
                    IsHTML = true,
                    Priority = Notification.NotificationPriority.High,
                    Subject = string.Format("Your Atlas Online application [{0}] under review", application.AccountNo),
                    To = "review@atlasonline.co.za",
                    Bcc = "lee@atcorp.co.za;fabian@atcorp.co.za"
                  });
                  break;
                #endregion

                default:
                  break;
              }
              uow.CommitChanges();
            }
              #endregion
          });
        }
      }
    }

    public override long AddRouteHistory(dynamic message)
    {
      return base.AddRouteHistory((FraudPreventionMessage)message);
    }
  }
}