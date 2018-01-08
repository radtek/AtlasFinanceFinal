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
  public class PerformQQ : Step
  {
    private ProcessDataExt.QuickQuote.QuickQuote _processData;

    public PerformQQ(IJob job)
      : base(job)
    {
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

      if (_processData == null)
      {
        //_processData = base.GetProcessData(Enumerators.Workflow.WorkflowDataExtType.QuickQuote);
      }

      if (_processData != null)
        base.CreateProcessStepJobAccount(_processData.AccountId);

      PerformQuickQuote();
    }

    public void PerformQuickQuote()
    {
      // This is where the QQ gets done
      System.Threading.Thread.Sleep(10000);


      Complete(_processData);
    }

    //public new void Complete(dynamic data)
    //{
    //  // Update Process Data Ext with relevant info
    //  //base.Complete();
    //}
  }
}