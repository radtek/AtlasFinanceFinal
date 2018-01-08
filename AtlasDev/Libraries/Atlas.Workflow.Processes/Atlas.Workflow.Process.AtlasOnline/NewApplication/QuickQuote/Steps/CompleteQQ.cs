using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Workflow.Interface;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Workflow.Process.AtlasOnline.NewApplication.QuickQuote.Steps
{
  public class CompleteQQ : Step
  {
    private ProcessDataExt.QuickQuote.QuickQuote _processData;

    public CompleteQQ(IJob job)
      : base(job)
    {
      _processData = new ProcessDataExt.QuickQuote.QuickQuote();
    }

    public override WFL_ProcessStepJobDTO GetCurrentProcessStepJob(dynamic data)
    {
      using (var uow = new UnitOfWork())
      {
        _processData = (ProcessDataExt.QuickQuote.QuickQuote)data;
        var processStepJob = new XPQuery<WFL_ProcessStepJobAccount>(uow).FirstOrDefault(p => p.Account.AccountId == _processData.AccountId
          && p.ProcessStepJob.CompleteDate == null).ProcessStepJob;

        return AutoMapper.Mapper.Map<WFL_ProcessStepJob, WFL_ProcessStepJobDTO>(processStepJob);
      }
    }

    public override void Start(dynamic data)
    {
      base.Start();

      //_processData = (ProcessDataExt.QuickQuote.QuickQuote)base.GetProcessData(Enumerators.Workflow.WorkflowDataExtType.QuickQuote);

      //base.CreateProcessStepJobAccount(_processData.AccountId);

      Complete(data);
    }
  }
}