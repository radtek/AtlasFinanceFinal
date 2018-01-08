using Atlas.Common.Utils;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Workflow.Interface;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Workflow.DecisionGate
{
  public static class DecisionGate
  {
    //public static void BuildTestExpression()
    //{
    //  //using (var uow = new UnitOfWork())
    //  //{
    //  //  var data = new ProcessDataExt.QuickQuote.QuickQuote()
    //  //    {
    //  //      AccountId = 1,
    //  //      ScoreCardId = 0,
    //  //      HostId = 1
    //  //    };

    //  //  var processData = new XPQuery<Atlas.Domain.Model.WFL_ProcessDataExt>(uow).FirstOrDefault(p => p.ProcessJob.ProcessJobId == 24
    //  //    && p.DataExtType.Type == Enumerators.Workflow.WorkflowDataExtType.QuickQuote);

    //  //  if (processData == null)
    //  //  {
    //  //    processData = new WFL_ProcessDataExt(uow);
    //  //    processData.ProcessJob = new XPQuery<WFL_ProcessJob>(uow).FirstOrDefault(c => c.ProcessJobId == 24);
    //  //    processData.DataExtType = new XPQuery<WFL_DataExtType>(uow).FirstOrDefault(c => c.Type == Enumerators.Workflow.WorkflowDataExtType.QuickQuote);
    //  //  }

    //  //  processData.Data = Xml.Serialize(data.GetType(), data, true);

    //  //  uow.CommitChanges();
    //  //}

    //  WFL_ProcessJobDTO processJobDTO = null;
    //  WFL_ConditionGroupDTO conditionGroupDTO = null;
    //  using (var uow = new UnitOfWork())
    //  {
    //    var condtionGroup = new XPQuery<WFL_ConditionGroup>(uow).FirstOrDefault(c => c.ConditionGroupId == 1);
    //    conditionGroupDTO = AutoMapper.Mapper.Map<WFL_ConditionGroup, WFL_ConditionGroupDTO>(condtionGroup);

    //    var processJob = new XPQuery<WFL_ProcessJob>(uow).FirstOrDefault(c => c.ProcessJobId == 24);
    //    processJobDTO = AutoMapper.Mapper.Map<WFL_ProcessJob, WFL_ProcessJobDTO>(processJob);
    //  }

    //  var expression = Atlas.Workflow.Condition.Expression.BuildExpression(conditionGroupDTO, processJobDTO);

    //}

    /// <summary>
    /// Returns the next ProcessStepId
    /// </summary>
    /// <param name="currentStep"></param>
    /// <returns></returns>
    public static WFL_ProcessStepDTO GetNextStep(IStep currentStep)
    {
      using (var uow = new UnitOfWork())
      {
        // GetDecision Stuffs with ActiveStep
        var decisions = new XPQuery<WFL_Decision>(uow).Where(d => d.ProcessStep.ProcessStepId == currentStep.ProcessStep.ProcessStepId).OrderBy(r => r.Rank);

        foreach (var decision in decisions)
        {
          WFL_ProcessStep processStep = GetNext(uow, decision.ConditionGroup, currentStep.ProcessStepJob.ProcessJob);

          return AutoMapper.Mapper.Map<WFL_ProcessStep, WFL_ProcessStepDTO>(processStep);
        }
      }

      return null;
    }

    private static WFL_ProcessStep GetNext(UnitOfWork uow, WFL_ConditionGroup conditionGroup, WFL_ProcessJobDTO processjob)
    {
      WFL_ConditionGroupResult conditionGroupResult = null;
      if (string.IsNullOrEmpty(conditionGroup.Expression))
      {
        // No need to check expression - just get the outcome process/ConditionGroup
        conditionGroupResult = new XPQuery<WFL_ConditionGroupResult>(uow).FirstOrDefault(r => r.ConditionGroup.ConditionGroupId == conditionGroup.ConditionGroupId
          && r.Result);
      }
      else
      {
        // Build Expression
        var expression = Condition.Expression.BuildExpression(AutoMapper.Mapper.Map<WFL_ConditionGroup, WFL_ConditionGroupDTO>(conditionGroup), processjob);

        var result = Dynamic.Compiler.GetExpressionResult(expression);

        conditionGroupResult = new XPQuery<WFL_ConditionGroupResult>(uow).FirstOrDefault(r => r.ConditionGroup.ConditionGroupId == conditionGroup.ConditionGroupId
          && r.Result == result);
      }

      if (conditionGroupResult.NextConditionGroup == null)
      {
        // Get outcome Step
        return conditionGroupResult.OutcomeProcessStep;
      }
      else
      {
        return GetNext(uow, conditionGroupResult.NextConditionGroup, processjob);
      }
    }
  }
}