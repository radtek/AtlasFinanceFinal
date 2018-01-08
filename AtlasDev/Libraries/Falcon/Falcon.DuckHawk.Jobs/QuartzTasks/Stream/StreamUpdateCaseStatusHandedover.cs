using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Ass.Framework.Repository;
using Autofac;
using Falcon.DuckHawk.Jobs.Attributes;
using Quartz;
using Serilog;
using Stream.Framework.DataContracts.Requests;
using Stream.Framework.Enumerators;
using Stream.Framework.Repository;
using Stream.Framework.Services;

namespace Falcon.DuckHawk.Jobs.QuartzTasks.Stream
{
  [DisableJob]
  [DisallowConcurrentExecution]
  [JobName("StreamUpdateCaseStatusHandedover")]
  [TriggerName("StreamUpdateCaseStatusHandedover")]
  //[CronExpression("0 00 7,19,20 1/1 * ? *")] // run every day @ 07:00 && 15:00 && 19:00
  [CronExpression("0 0/1 * 1/1 * ? *")] // runs every 1 minute for testing
  //[ScheduleBuilder]
  public class StreamUpdateCaseStatusHandedover : IJob
  {
    public IScheduleBuilder Schedule
    {
      get
      {
        return SimpleScheduleBuilder.Create().WithIntervalInSeconds(15).WithRepeatCount(0).WithMisfireHandlingInstructionFireNow();
      }
    }

    private readonly ILifetimeScope _scope;
    private readonly IStreamRepository _streamRepository;
    private readonly IAssStreamRepository _assStreamRepository;
    private readonly IStreamService _streamService;
    private readonly ILogger _logger;

    public StreamUpdateCaseStatusHandedover(ILifetimeScope scope, IStreamRepository streamRepository, IAssStreamRepository assStreamRepository, IStreamService streamService, ILogger logger)
    {
      _scope = scope;
      _streamRepository = streamRepository;
      _assStreamRepository = assStreamRepository;
      _streamService = streamService;
      _logger = logger;
    }

    public void Execute(IJobExecutionContext context)
    {
      using (_scope.BeginLifetimeScope())
      {
        try
        {
          _logger.Information("Started Job [StreamUpdateCaseStatusHandedover]");

          // get all cases
          var cases =
            _streamRepository.GetCasesByStatus(global::Stream.Framework.Enumerators.Stream.GroupType.Collections,
              global::Stream.Framework.Enumerators.CaseStatus.Type.Closed);
          
//          cases  = cases.Where(c => c.CaseId == 4074227).ToList();

          Parallel.ForEach(cases, new ParallelOptions
          {
            MaxDegreeOfParallelism = 15
          }, streamCase =>
          {
            _logger.Debug("Job [StreamUpdateCaseStatusHandedover] start - " + streamCase.CaseId);
            try
            {
              // get all account linked to case
              var accounts = _streamRepository.GetAccountsByCaseId(streamCase.CaseId);

              // get status of accounts from ass db
              var accountStatuses = _assStreamRepository.GetHandedoverAccounts(streamCase.CreateDate, streamCase.LastStatusDate, accounts.Select(a => a.Reference2).ToArray());

              // update case statuses where status == handover
              if (accountStatuses.Any(s => s.Status.ToLower() == "h"))
              {
                _streamRepository.AddOrUpdateCase(new AddOrUpdateCaseRequest
                {
                  BranchId = streamCase.BranchId,
                  AllocatedUserId = streamCase.AllocatedUserId,
                  DebtorId = streamCase.DebtorId,
                  GroupType = streamCase.GroupType,
                  SubCategory = streamCase.SubCategoryType,Priority = streamCase.Priority,
                  Host = streamCase.Host,
                  Reference = streamCase.Reference,
                  CaseId = streamCase.CaseId,
                  CaseStatus = CaseStatus.Type.HandedOver,
                  TotalBalance = streamCase.TotalBalance,
                  TotalArrearsAmount = streamCase.TotalArrearsAmount,
                  TotalRequiredPayment = streamCase.TotalRequiredPayment,
                  LastReceiptDate = streamCase.LastReceiptDate,
                  LastReceiptAmount = streamCase.LastReceiptAmount,
                  TotalInstalmentsOutstanding = streamCase.TotalInstalmentsOutstanding,
                  TotalLoanAmount = streamCase.TotalLoanAmount,
                  SmsCount = streamCase.SmsCount
                });
              }
            }
            catch (Exception ex)
            {
              _logger.Error("Job [StreamUpdateCaseStatusHandedover] end - " + streamCase.CaseId + ", " + ex.Message + "," + ex.StackTrace);
            }
          });

          _logger.Information("Finished Job [StreamUpdateCaseStatusHandedover]");
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("Error in Job [StreamUpdateCaseStatusHandedover]: {0} - {1}", ex.Message, ex.StackTrace));
        }
      }
    }
  }
}
