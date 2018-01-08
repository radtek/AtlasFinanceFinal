using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Service.OrchestrationService;
using DevExpress.Xpo;
using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;
using Atlas.Domain.Structures;
using Atlas.Online.Web.Service.AccountService;
using Atlas.RabbitMQ.Messages.Notification;
using Magnum;
using Atlas.Enumerators;
using Atlas.Online.Web.Service.EasyNetQ;

namespace Atlas.Online.Web.Service.Tasks
{
  [DisallowConcurrentExecution]
  public class AVSCheckTask : IJob
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(NotificationOfInactiveAccountTask));
    private int DELAY = 0;
    public void Execute(IJobExecutionContext context)
    {
      _log.Info(string.Format(":: [Tasks] {0} Executing...", context.JobDetail.Key.Name));

      using (var uow = new UnitOfWork())
      {
        List<Enumerators.Account.AccountStatus> statuses = new List<Enumerators.Account.AccountStatus>();
        statuses.Add(Enumerators.Account.AccountStatus.PreApproved);
        statuses.Add(Enumerators.Account.AccountStatus.Approved);
        statuses.Add(Enumerators.Account.AccountStatus.Inactive);

        var applicationCollection = new XPQuery<Application>(uow).Where(p => statuses.Contains(p.Status) && p.IsCurrent).ToList();

        foreach (var application in applicationCollection)
        {
          if (application.Client.PersonId != null)
          {
            var bankDetail = application.BankDetail;
            AccountVerification accountVerification = null;
            string body = string.Empty;
            string subject = string.Empty;
            Dictionary<string, string> dict = new Dictionary<string, string>();

            if (bankDetail != null && bankDetail.IsEnabled && !bankDetail.IsVerified)
            {
              if (bankDetail.TransactionId == null)
              {
                List<Atlas.Online.Web.Service.OrchestrationService.BankDetailDTO> bankDetails = null;

                new OrchestrationServiceClient("OrchestrationService.NET").Using(client =>
                {
                  bankDetails = client.GetBankDetails(application.Client.PersonId);

                  if (bankDetails.Count > 0)
                  {
                    var currentBankDetails = bankDetails.FirstOrDefault(o => o.AccountNum == application.BankDetail.AccountNo && o.Bank.Description == application.BankDetail.Bank.Description);
                        //Edited By Prashant
                        if (currentBankDetails != null)
                        {
                            var avsResult = client.GetAccountVerification(application.Client.IDNumber, currentBankDetails.AccountNum, currentBankDetails.Bank.Type, DELAY);
                            bankDetail.TransactionId = avsResult.TransactionId;
                            bankDetail.Save();
                            uow.CommitChanges();
                        }
                  }
                });
              }

              if (bankDetail.TransactionId != null)
              {
                new OrchestrationServiceClient("OrchestrationService.NET").Using(client =>
                {
                  accountVerification = client.GetAccountVerificationById((long)bankDetail.TransactionId);
                });

                if (accountVerification != null)
                {
                  if (accountVerification.Transaction == Enumerators.Orchestration.AVSTransaction.AVS_Current)
                  {
                    application.BankDetail.IsActive = true;
                    application.BankDetail.IsVerified = true;
                    application.BankDetail.Save();

                    if (application.Status == Account.AccountStatus.PreApproved)
                    {
                      dict.Clear();

                      dict.Add("{Name}", application.Client.Firstname);
                      dict.Add("{Surname}", application.Client.Surname);

                      string compiled = string.Empty;

                      new OrchestrationServiceClient("OrchestrationService.NET").Using(cli =>
                      {
                        body = cli.GetCompiledTemplate(Notification.NotificationTemplate.AVSPassed_AcceptQuote, dict);
                      });

                      ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
                      {
                        ActionDate = DateTime.Now,
                        Body = body,
                        CreatedAt = DateTime.Now,
                        From = "noreply@atlasonline.co.za",
                        IsHTML = true,
                        Priority = Notification.NotificationPriority.High,
                        Subject = "Bank account verified!",
                        To = new XPQuery<UserProfile>(uow).FirstOrDefault(p => p.UserId == application.Client.UserId).Email
                      });
                    }
                  }
                  else if (accountVerification.Transaction == Enumerators.Orchestration.AVSTransaction.AVS_Failed)
                  {
                    int failedCount = 0;

                    new AccountServerClient("AccountServer.NET").Using(cli =>
                    {
                      failedCount = cli.GetAVSFailureCount((long)application.Client.PersonId);
                    });

                    if (failedCount >= 2)
                    {
                      dict.Clear();

                      dict.Add("{Name}", application.Client.Firstname);
                      dict.Add("{Surname}", application.Client.Surname);
                      dict.Add("{AccountNo}", application.AccountNo);

                      string compiled = string.Empty;

                      new OrchestrationServiceClient("OrchestrationService.NET").Using(cli =>
                      {
                        body = cli.GetCompiledTemplate(Notification.NotificationTemplate.Declined_BankDetails, dict);
                      });

                      subject = "Bank Account Validation Failed";

                      Application.UpdateIsCurrent(uow, application.ApplicationId, false);
                      Application.UpdateStatus(uow, application.ApplicationId, Account.AccountStatus.Declined);

                      uow.CommitChanges();

                      if (application.AccountId != null)
                      {
                        new AccountServerClient("AccountServer.NET").Using(cli =>
                        {
                          cli.UpdateAccountStatus((long)application.AccountId, AccountAccountStatus.Declined, AccountAccountStatusReason.CompanyPolicy, AccountAccountStatusSubReason.SalaryBankAccount);
                        });
                      }
                    }
                    else if (failedCount < 2)
                    {
                      dict.Clear();

                      dict.Add("{Name}", application.Client.Firstname);
                      dict.Add("{Surname}", application.Client.Surname);

                      string compiled = string.Empty;

                      new OrchestrationServiceClient("OrchestrationService.NET").Using(cli =>
                      {
                        body = cli.GetCompiledTemplate(Notification.NotificationTemplate.BankDetails_Failed, dict);
                      });

                      subject = "Failed to validate your bank details";
                    }

                    ServiceLocator.Get<AtlasOnlineServiceBus>().GetServiceBus().Publish<EmailNotifyMessage>(new EmailNotifyMessage(CombGuid.Generate())
                    {
                      ActionDate = DateTime.Now,
                      Body = body,
                      CreatedAt = DateTime.Now,
                      From = "noreply@atlasonline.co.za",
                      IsHTML = true,
                      Priority = Notification.NotificationPriority.High,
                      Subject = subject,
                      To = new XPQuery<UserProfile>(uow).FirstOrDefault(p => p.UserId == application.Client.UserId).Email
                    });

                    application.BankDetail.IsActive = false;
                    application.BankDetail.IsVerified = true;
                    application.BankDetail.Save();
                  }
                }
              }
            }
            uow.CommitChanges();
          }
        }
      }
      _log.Info(string.Format(":: [Tasks] {0} Finished Executing.", context.JobDetail.Key.Name));
    }
  }
}