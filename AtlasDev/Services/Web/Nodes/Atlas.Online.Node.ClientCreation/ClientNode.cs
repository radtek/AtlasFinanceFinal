using System;
using Atlas.Online.Node.Core;
using Ninject.Extensions.Logging;
using MassTransit;
using Ninject;
using Atlas.Common.Utils;
using System.Collections.Generic;
using System.Linq;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Common.Extensions;
using Atlas.Domain.DTO;
using Atlas.Enumerators;
using DevExpress.Xpo;
using System.Text;
using System.Security.Cryptography;
using Atlas.Online.Node.ClientNode.OrchestrationService;
using Atlas.RabbitMQ.Messages.Online;
using System.Threading.Tasks;
using Atlas.Online.Node.ClientNode.EasyNetQ;

namespace Atlas.Online.Node.ClientNode
{
  public sealed class ClientServiceNode : AbstractService<ClientServiceNode, ClientMessage, AccountCreationMessage>
  {
    private IKernel _kernel;
    private static object obj = new object();
    public ClientServiceNode(ILogger ilogger, IKernel ikernal)
      : base(ilogger)
    {
      _kernel = ikernal;
    }

    public override void Start()
    {
      base.Start();

      var atlasOnlineServiceBus = _kernel.Get<AtlasOnlineServiceBus>();

      // Setup subscription handler for specific message type
      atlasOnlineServiceBus.GetServiceBus().Subscribe<ClientMessage>("queue_ClientMessage", Handle);

      ServiceLocator.SetServiceLocator(_kernel);

    }

    public override void Stop()
    {
      base.Stop();
    }

    private string GetSHA256(string strPlain)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(strPlain);
      byte[] buffer = new SHA256Managed().ComputeHash(bytes);
      string str = string.Empty;
      foreach (byte num in buffer)
      {
        str = str + string.Format("{0:x2}", num);
      }
      return str;
    }

    public override void Publish(AccountCreationMessage message)
    {
      ServiceLocator.Get<AtlasOnlineServiceBus>()
                  .GetServiceBus().Publish(message);
    }

    /// <summary>
    /// Creates the core data required to begin the loan application process
    /// </summary>
    public override void Handle(ClientMessage message)
    {
      try
      {
        bool success = false;

        using (_kernel.BeginBlock())
        {
          using (var uow = new UnitOfWork())
          {
            var client = new XPQuery<Client>(uow).FirstOrDefault(c => c.ClientId == message.ClientId);
            var application = client.Applications.OfType<Application>().FirstOrDefault(p => p.IsCurrent);

            long? personId = null;

            #region Save Message

            // Create initial message insert.
            _logger.Info(string.Format(":: Msg - ClientId:{0}", message.ClientId));


            var msg = new Message(uow)
            {
              Client = client,
              CorrelationId = message.CorrelationId,
              MsgData = Compression.Compress(Xml.Serialize<ClientMessage>(message)),
              CreateDate = DateTime.Now
            };

            msg.Save();
            uow.CommitChanges();


            _logger.Info(string.Format(":: Msg End - ClientId:{0} , MessageId:{1}", message.ClientId, msg.MessageId));

            #endregion

            new OrchestrationServiceClient("OrchestrationService.NET").Using((cli) =>
            {
              var person = cli.GetByIdNo(client.IDNumber);

              _logger.Info(string.Format(":: Person Lookup - ClientId:{0}, MessageId:{1}", message.ClientId, msg.MessageId));

              if (person != null)
              {
                _logger.Info(string.Format(":: Person Found - ClientId:{0}, MessageId:{1}, PersonId:{2}", message.ClientId, message.MessageId, person.PersonId));
                personId = person.PersonId;

                success = true;

                if (client.PersonId != personId)
                {
                  if (person.CreatedDT <= new DateTime(2013, 08, 25))
                  {
                    _logger.Info(string.Format(":: Branch Client, Linking to Online Platform - ClientId:{0}, MessageId:{1}, PersonId:{2}", message.ClientId, message.MessageId, person.PersonId));
                    client.PersonId = person.PersonId;
                    client.Save();

                    uow.CommitChanges();

                    _logger.Info(string.Format(":: Branch Client, Linking to Online Platform End - ClientId:{0}, MessageId:{1}, PersonId:{2}", message.ClientId, message.MessageId, person.PersonId));
                  }
                  else if (client.PersonId != null && client.PersonId != personId)
                  {
                    success = false;
                    #region Dupe Check

                    _logger.Warn(string.Format(":: !!Dupe warning!! - ClientId:{0}, Online PersonId:{1},  CoreDB PersonId:{2}, MsgId:{3}", client.ClientId, client.PersonId, personId, msg.MessageId));
                    // Publis message to email node to notify of us of the potential issue.
                    ServiceLocator.Get<AtlasOnlineServiceBus>()
                  .GetServiceBus().Publish(new EmailMessage(message.CorrelationId)
                    {
                      CreatedAt = DateTime.Now,
                      Source = NodeType.Nodes.Client,
                      Destination = NodeType.Nodes.Email,
                      MessageId = msg.MessageId,
                      IsBodyHTML = false,
                      Body = string.Format("Possible duplication - ClientId:{0}, Online PersonId:{1},  CoreDB PersonId:{2}, messageId:{3}, Date:{4}",
                                                             client.ClientId, client.PersonId, personId, msg.MessageId, DateTime.Now.ToLongTimeString()),
                      To = "lee@atcorp.co.za;fabian@atcorp.co.za",
                      From = "watch@atlasonline.co.za",
                      Priority = System.Net.Mail.MailPriority.High
                    });

                    #endregion
                  }
                  else
                  {
                    #region ReLink lost client link

                    _logger.Warn(string.Format(":: Person {0} Found, No link {1} , Re-linking", personId, client.ClientId));

                    client.PersonId = person.PersonId;
                    client.Save();

                    uow.CommitChanges();

                    #endregion
                  }
                }
                else
                {
                  var result = Save(cli, client, application);

                  // Update Web DB with Core DB BankDetailId
                  var bankDetail = new XPQuery<Data.Models.Definitions.BankDetail>(uow).FirstOrDefault(p => p.BankDetailId == application.BankDetail.BankDetailId);
                  bankDetail.ReferenceId = result.m_Item2;
                  bankDetail.Save();

                  uow.CommitChanges();

                  success = true;
                }
              }
              else
              {
                _logger.Info(string.Format(":: New Client - ClientId:{0}, MessageId:{1}", message.ClientId, msg.MessageId));

                var result = Save(cli, client, application);

                var bankDetail = new XPQuery<Data.Models.Definitions.BankDetail>(uow).FirstOrDefault(p => p.BankDetailId == application.BankDetail.BankDetailId);
                bankDetail.ReferenceId = result.m_Item2;
                bankDetail.Save();

                _logger.Info(string.Format(":: New Client End - ClientId:{0}, MessageId:{1} , PersonId:{2}", message.ClientId, message.MessageId, personId));

                _logger.Info(string.Format(":: Link Create - ClientId:{0}, MessageId:{1}, PersonId:{2}", message.ClientId, message.MessageId, personId));

                client.PersonId = result.m_Item1;
                client.Save();

                uow.CommitChanges();

                success = true;

                _logger.Info(string.Format(":: Link End - ClientId:{0}, MessageId:{1}, PersonId:{2}", message.ClientId, message.MessageId, personId));
              }
            });

            if (success)
            {
              // Set the new source of the message
              message.Source = NodeType.Nodes.Client;
              message.MessageId = msg.MessageId;

              // Set Destination of message
              message.Destination = NodeType.Nodes.AccountCreation;

              // Add route history.
              var routeHistoryId = AddRouteHistory(message);

              // Route to next node
              Publish(new AccountCreationMessage(message.CorrelationId)
              {
                CreatedAt = DateTime.Now,
                Source = NodeType.Nodes.Client,
                MessageId = message.MessageId,
                ClientId = message.ClientId,
                RouteHistoryId = routeHistoryId
              });
            }
            else
            {
              // Notify Error otherwise -- TODO
            }
          }
        }
      }
      catch (Exception ex)
      {
        _logger.Fatal(string.Format(":: Failed - ClientId: {0}, MessageId: {1}, Stack: {2}, Message:{3}", message.ClientId, message.MessageId, ex.StackTrace.ToString(), ex.Message));
      }
    }

    internal TupleOflonglong Save(OrchestrationServiceClient service, Client client, Application application)
    {
      string cellNo = string.Empty;
      string homeNo = string.Empty;
      string email = string.Empty;

      var contactCell = client.Contacts.OfType<Data.Models.Definitions.Contact>().FirstOrDefault(_ => _.ContactType.ContactTypeId == General.ContactType.CellNo.ToInt());
      var contactHome = client.Contacts.OfType<Data.Models.Definitions.Contact>().FirstOrDefault(_ => _.ContactType.ContactTypeId == General.ContactType.TelNoHome.ToInt());
      var employer = application.Employer;

      if (contactCell != null)
        cellNo = contactCell.Value;

      if (contactHome == null && contactCell != null)
        homeNo = cellNo;

      if (contactHome != null)
        homeNo = contactHome.Value;

      var IdValidation = new IDValidator(client.IDNumber);

      using (var uow = new UnitOfWork())
      {
        var userProfile = new XPQuery<UserProfile>(uow).FirstOrDefault(p => p.UserId == client.UserId);

        if (userProfile == null)
          throw new Exception(string.Format("UserProfile is missing for Client {0}", client.ClientId));

        email = userProfile.Email;
      }

      Person _client = new Person();

      try
      {
        #region Client

        _client.FirstName = client.Firstname;
        _client.LastName = client.Surname;
        _client.IdNo = client.IDNumber;
        _client.Gender = IdValidation.IsFemale() ? General.Gender.Female : General.Gender.Male;
        _client.Host = General.Host.Atlas_Online;
        _client.Email = email;
        _client.DateOfBirth = client.DateOfBirth == null ? IdValidation.GetDateOfBirthAsDateTime() : (DateTime)client.DateOfBirth;
        _client.Title = client.Title;

        _client.Contacts = new List<OrchestrationService.Contact>();

        _client.Contacts.Add(new OrchestrationService.Contact()
        {
          ContactType = General.ContactType.CellNo,
          No = cellNo
        });


        _client.Contacts.Add(new OrchestrationService.Contact()
        {
          ContactType = General.ContactType.TelNoHome,
          No = homeNo
        });

        #endregion

        #region Employer

        _client.Employer = new OrchestrationService.Employer();
        _client.Employer.Name = employer.Name;
        _client.Employer.Addresses = new List<OrchestrationService.Address>();


        _client.Employer.Addresses.Add(new OrchestrationService.Address()
        {
          Line1 = employer.Address.AddressLine1,
          Line2 = employer.Address.AddressLine2,
          Line3 = employer.Address.AddressLine3,
          Line4 = employer.Address.AddressLine4,
          AddressType = employer.Address.AddressType.Type,
          Code = employer.Address.PostalCode,
          Province = (General.Province)employer.Address.Province.ProvinceId
        });

        _client.Employer.Contacts = new List<OrchestrationService.Contact>();
        _client.Employer.Contacts.Add(new OrchestrationService.Contact()
        {
          ContactType = General.ContactType.TelNoWork,
          No = employer.ContactNo
        });

        #endregion

        #region Relation

        _client.Relation = new Relation();
        _client.Relation.FirstName = application.NextOfKin.FirstName;
        _client.Relation.LastName = application.NextOfKin.Surname;
        _client.Relation.RelationType = application.NextOfKin.Relation;
        _client.Relation.Contacts = new List<OrchestrationService.Contact>();
        _client.Relation.Contacts.Add(new OrchestrationService.Contact()
        {
          ContactType = General.ContactType.CellNo,
          No = application.NextOfKin.ContactNo
        });


        #endregion

        #region Address

        _client.Addresses = new List<OrchestrationService.Address>();

        _client.Addresses.Add(new OrchestrationService.Address()
        {
          Line1 = application.ResidentialAddress.AddressLine1,
          Line2 = application.ResidentialAddress.AddressLine2,
          Line3 = application.ResidentialAddress.AddressLine3,
          Line4 = application.ResidentialAddress.AddressLine4,
          Code = application.ResidentialAddress.PostalCode,
          AddressType = General.AddressType.Residential,
          Province = (General.Province)application.ResidentialAddress.Province.ProvinceId
        });


        #endregion

        #region Bank Detail

        _client.BankDetail = new OrchestrationService.BankDetail();
        _client.BankDetail.AccountName = application.BankDetail.AccountName;
        _client.BankDetail.AccountNo = application.BankDetail.AccountNo;
        _client.BankDetail.AccountType = application.BankDetail.AccountType.Type;
        _client.BankDetail.Bank = application.BankDetail.Bank.Type;
        _client.BankDetail.Period = application.BankDetail.Period.Type;

        #endregion
      }
      catch (Exception ex)
      {
        _logger.FatalException("Msg", ex);
      }

      return service.Save(_client);
    }

    public override long AddRouteHistory(dynamic message)
    {
      return base.AddRouteHistory((ClientMessage)message);
    }
  }
}