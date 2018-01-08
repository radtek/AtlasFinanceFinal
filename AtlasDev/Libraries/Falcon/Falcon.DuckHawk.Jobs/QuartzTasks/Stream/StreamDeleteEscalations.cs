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
  [JobName("StreamDeleteEscalations")]
  [TriggerName("StreamDeleteEscalations")]
  //[CronExpression("0 00 7,19,20 1/1 * ? *")] // run every day @ 07:00 && 15:00 && 19:00
  [CronExpression("0 0/1 * 1/1 * ? *")] // runs every 1 minute for testing
  public  class StreamDeleteEscalations:IJob
  {
    private readonly ILifetimeScope _scope;
    private readonly IStreamRepository _streamRepository;
    private readonly ILogger _logger;
    private readonly IConfigService _configService;
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;

    public StreamDeleteEscalations(ILifetimeScope scope, IStreamRepository streamRepository, ILogger logger, IConfigService configService, IUserRepository userRepository, ICompanyRepository companyRepository)
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
          _logger.Information("Started Job [StreamDeleteEscalations]");

          // get all branches
          var branches = _companyRepository.GetAllBranches();

          Parallel.ForEach(branches, new ParallelOptions
          {
            MaxDegreeOfParallelism = 15
          }, branch =>
          {
            _logger.Information("Job [StreamDeleteEscalations] - " + branch.BranchId);
            try
            {
              var caseStreamsToUpdate = new RawSql().ExecuteObject<CaseStreamToUpdate>(
                string.Format(
                  "SELECT CST.\"CaseStreamId\", CSE.\"CaseStreamEscalationId\", CSA.\"CaseStreamAllocationId\" " +
                  "FROM \"STR_CaseStream\" CST " +
                  "LEFT JOIN \"STR_Case\" CS ON CST.\"CaseId\" = CS.\"CaseId\" " +
                  "LEFT JOIN \"STR_CaseStreamEscalation\" CSE ON CST.\"CaseStreamId\" = CSE.\"CaseStreamId\"" +
                  " 	AND CSE.\"EscalationId\" > 1 " +
                  "Left JOIN \"STR_CaseStreamAllocation\" CSA ON CST.\"CaseStreamId\" = CSA.\"CaseStreamId\"" +
                  " 	AND CSA.\"EscalationId\" > 1 " +
                  "WHERE CS.\"BranchId\" IN ({0}) " +
                  "   AND (CSE.\"CaseStreamId\" IS NOT NULL OR CSA.\"CaseStreamId\" IS NOT NULL)", branch.BranchId), _configService.AtlasCoreConnection)
                .Distinct()
                .ToList();

              foreach (var caseStreamToUpdate in caseStreamsToUpdate)
              {
                new RawSql().ExecuteScalar(
                  string.Format(
                    "DELETE FROM \"STR_CaseStreamAllocation\" " +
                    "WHERE \"CaseStreamAllocationId\" IN ({0}); " +
                    "DELETE FROM \"STR_CaseStreamEscalation\" " +
                    "WHERE \"CaseStreamEscalationId\" IN ({1}); " +
                    "UPDATE \"STR_CaseStream\" " +
                    "SET \"EscalationId\" = 1 " +
                    "WHERE \"CaseStreamId\" IN ({2}); ",
                    caseStreamToUpdate.CaseStreamAllocationId, caseStreamToUpdate.CaseStreamEscalationId,
                    caseStreamToUpdate.CaseStreamId), _configService.AtlasCoreConnection);
              }
            }
            catch (Exception ex)
            {
              _logger.Error("Job [StreamDeleteEscalations] end - " + branch.BranchId + ", " + ex.Message + "," + ex.StackTrace);
            }
            _logger.Information("Job [StreamDeleteEscalations] end - " + branch.BranchId);
          });
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("Error in Job [StreamDeleteEscalations]: {0} - {1}", ex.Message, ex.StackTrace));
        }
      }
    }
  }

  public class CaseStreamToUpdate
  {
    public long CaseStreamId { get; set; }
    public long CaseStreamEscalationId { get; set; }
    public long CaseStreamAllocationId { get; set; }
  }
}
