using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Workflow.Interface;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Workflow.Process.AtlasOnline.NewApplication.Payout.Steps
{
  public class RTC : Step
  {
    private ProcessDataExt.Payout.Payout _processData;

    public RTC(IJob job)
      : base(job)
    {
      _processData = new ProcessDataExt.Payout.Payout();
    }

    public override WFL_ProcessStepJobDTO GetCurrentProcessStepJob(dynamic data)
    {
      using (var uow = new UnitOfWork())
      {
        _processData = data;
        var processStepJob = new XPQuery<WFL_ProcessStepJobAccount>(uow).FirstOrDefault(p => p.Account.AccountId == _processData.AccountId
          && p.ProcessStepJob.CompleteDate == null).ProcessStepJob;

        return AutoMapper.Mapper.Map<WFL_ProcessStepJob, WFL_ProcessStepJobDTO>(processStepJob);
      }
    }

    public override void Start(dynamic data)
    {
      base.Start();

    }

    public override void Complete(dynamic data)
    {
      // Open Loan 
      // Add Transactions to GL

      base.Complete((ProcessDataExt.Payout.Payout)data);
    }
  }
}
