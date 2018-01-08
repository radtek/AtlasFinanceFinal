using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.RabbitMQ.Messages.Workflow;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Workflow.Handlers
{
  public class RabbitHandler
  {

    public RabbitHandler()
    {
    }

    public void StepWorkflowProcessMessageHandler(StepWorkflowProcessMessage stepWorkflowProcessMessage)
    {
      WFL_ProcessStepDTO currentProcessStepDTO = null;
      WFL_ProcessStepDTO jumpToProcessStepDTO = null;
      using (var uow = new UnitOfWork())
      {
        var currentProcessStep = new XPQuery<WFL_ProcessStep>(uow).FirstOrDefault(p => p.ProcessStepId == stepWorkflowProcessMessage.CurrentProcessStepId);
        currentProcessStepDTO = AutoMapper.Mapper.Map<WFL_ProcessStep, WFL_ProcessStepDTO>(currentProcessStep);
        if (stepWorkflowProcessMessage.JumpToProcessStepId != null)
        {
          var jumpToProcessStep = new XPQuery<WFL_ProcessStep>(uow).FirstOrDefault(p => p.ProcessStepId == (int)stepWorkflowProcessMessage.JumpToProcessStepId);
          jumpToProcessStepDTO = AutoMapper.Mapper.Map<WFL_ProcessStep, WFL_ProcessStepDTO>(jumpToProcessStep);
        }
      }
      var job = new Job(currentProcessStepDTO.Process);

      dynamic step = Common.Step.CreateDynamicStepInstance(currentProcessStepDTO.Process.Assembly, currentProcessStepDTO.Namespace, new object[] { job });

      step.Rank = currentProcessStepDTO.Rank;

      job.CompleteStepAndMove(step, stepWorkflowProcessMessage.Data, stepWorkflowProcessMessage.Direction, jumpToProcessStepDTO);
    }

    public void StartWorflowProcessMessageHandler(StartWorflowProcessMessage startWorflowProcessMessage)
    {
      WFL_ProcessStepDTO firstProcessStepDTO = null;
      // Get the first process in the workflow
      using (var uow = new UnitOfWork())
      {
        var firstProcessStep = new XPQuery<WFL_ProcessStep>(uow).Where(p => p.Process.Workflow.Type == startWorflowProcessMessage.Workflow
          && p.Process.Enabled
          && p.Enabled).OrderBy(p => p.Process.Rank).OrderBy(ps => ps.Rank).FirstOrDefault();
        if (firstProcessStep == null)
        {
          // Unable to proceed, no available processes to start with
          throw new NotImplementedException();
        }
        firstProcessStepDTO = AutoMapper.Mapper.Map<WFL_ProcessStep, WFL_ProcessStepDTO>(firstProcessStep);
      }

      // start job for the processStep
      var job = new Job(firstProcessStepDTO.Process);

      dynamic step = Common.Step.CreateDynamicStepInstance(firstProcessStepDTO.Process.Assembly, firstProcessStepDTO.Namespace, new object[] { job });

      // TODO: Add logger
      //step.Logger = _logger;

      step.UseDecisionGate = true;
      step.ProcessStep = firstProcessStepDTO;
      step.Rank = firstProcessStepDTO.Rank;

      job.StartAtStep(step, startWorflowProcessMessage.Data);
    }
  }
}
