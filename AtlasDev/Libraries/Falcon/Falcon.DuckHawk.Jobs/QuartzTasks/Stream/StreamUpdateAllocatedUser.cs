using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Common.Utils;
using Autofac;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Services;
using Falcon.DuckHawk.Jobs.Attributes;
using MassTransit;
using Quartz;
using Serilog;
using Stream.Framework.Repository;

namespace Falcon.DuckHawk.Jobs.QuartzTasks.Stream
{
  [DisableJob]
  [DisallowConcurrentExecution]
  [JobName("StreamUpdateAllocatedUser")]
  [TriggerName("StreamUpdateAllocatedUser")]
  //[CronExpression("0 00 7,19,20 1/1 * ? *")] // run every day @ 07:00 && 15:00 && 19:00
  [CronExpression("0 0/1 * 1/1 * ? *")] // runs every 1 minute for testing
  public  class StreamUpdateAllocatedUser:IJob
  {
    private readonly ILifetimeScope _scope;
    private readonly IStreamRepository _streamRepository;
    private readonly ILogger _logger;
    private readonly IConfigService _configService;
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;

    public StreamUpdateAllocatedUser(ILifetimeScope scope, IStreamRepository streamRepository, ILogger logger, IConfigService configService, IUserRepository userRepository, ICompanyRepository companyRepository)
    {
      _scope = scope;
      _streamRepository = streamRepository;
      _logger = logger;
      _configService = configService;
      _userRepository = userRepository;
      _companyRepository = companyRepository;
    }

    public void Execute(IJobExecutionContext context)
    {
      using (_scope.BeginLifetimeScope())
      {
        try
        {
          _logger.Information("Started Job [StreamUpdateAllocatedUser]");

          // get all cases
          var cases =
            _streamRepository.GetCasesByStatus(global::Stream.Framework.Enumerators.Stream.GroupType.Collections,
              global::Stream.Framework.Enumerators.CaseStatus.Type.New,
              global::Stream.Framework.Enumerators.CaseStatus.Type.InProgress,
              global::Stream.Framework.Enumerators.CaseStatus.Type.OnHold);

          var branches = _companyRepository.GetAllBranches();
          var lee = _userRepository.GetPerson("2594bfdb-0b54-421e-81ea-818bac8f12b0");

          Parallel.ForEach(cases, new ParallelOptions
          {
            MaxDegreeOfParallelism = 15
          }, streamCase =>
          {
            _logger.Information("Job [StreamUpdateAllocatedUser] collections - " +streamCase.CaseId);
            try
            {
              var accounts = _streamRepository.GetAccountsByCaseId(streamCase.CaseId);

              var operatorCodes = new RawSql().ExecuteObjectString(
                string.Format("SELECT oper FROM company.loans where recid IN ({0})",
                  string.Join(",", accounts.Select(a => a.Reference2))), _configService.AssConnection)
                .Distinct()
                .ToList();

              foreach (var operatorCode in operatorCodes)
              {
                var allocatedUser = _userRepository.GetUserByOperatorCode(operatorCode,
                  branches.FirstOrDefault(b => b.BranchId == streamCase.BranchId).LegacyBranchNum);

                if (allocatedUser != null && streamCase.AllocatedUserId != allocatedUser.PersonId)
                {

                  var caseStream = _streamRepository.GetOpenCaseStreamForCase(streamCase.CaseId);
                  if (caseStream != null)
                  {
                    _streamRepository.ChangeCaseStreamAllocatedUser(caseStream.CaseStreamId, lee.WebReference,
                      streamCase.AllocatedUserId,allocatedUser.PersonId);
                  }
                }
              }
            }
            catch (Exception ex)
            {
              _logger.Error("Job [StreamUpdateAllocatedUser] collections end - " + streamCase.CaseId + ", " + ex.Message + "," + ex.StackTrace);
            }
            _logger.Information("Job [StreamUpdateAllocatedUser] collections end - " + streamCase.CaseId);
          });


          _logger.Information(" Job [StreamUpdateAllocatedUser] - sales");

          // get all cases
           cases =
            _streamRepository.GetCasesByStatus(global::Stream.Framework.Enumerators.Stream.GroupType.Collections,
              global::Stream.Framework.Enumerators.CaseStatus.Type.New,
              global::Stream.Framework.Enumerators.CaseStatus.Type.InProgress,
              global::Stream.Framework.Enumerators.CaseStatus.Type.OnHold);

          Parallel.ForEach(cases, streamCase =>
          {
            _logger.Information("Started Job [StreamUpdateAllocatedUser] sales - " + streamCase.CaseId);
            var accounts = _streamRepository.GetAccountsByCaseId(streamCase.CaseId);

            var operatorCodes = new RawSql().ExecuteObjectString(
              string.Format("SELECT oper FROM company.loans where recid IN ({0})",
                string.Join(",", accounts.Select(a => a.Reference2))), _configService.AssConnection).Distinct().ToList();

            foreach (var operatorCode in operatorCodes)
            {
              var allocatedUser = _userRepository.GetUserByOperatorCode(operatorCode,
                branches.FirstOrDefault(b => b.BranchId == streamCase.BranchId).LegacyBranchNum);

              if (allocatedUser != null && streamCase.AllocatedUserId != allocatedUser.PersonId)
              {

                var caseStream = _streamRepository.GetOpenCaseStreamForCase(streamCase.CaseId);
                if (caseStream != null)
                {
                  _streamRepository.ChangeCaseStreamAllocatedUser(caseStream.CaseStreamId, lee.WebReference, streamCase.AllocatedUserId,allocatedUser.PersonId);
                }
              }
            }
          });

          _logger.Information("Finished Job [StreamUpdateAllocatedUser]");
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("Error in Job [StreamUpdateAllocatedUser]: {0} - {1}", ex.Message, ex.StackTrace));
        }
      }
    }
  }
}
