using Atlas.Domain.Model;
using Atlas.Workflow.Interface;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atlas.Workflow.Process.Statements.Steps
{
  public class GetInfo : Step
  {
    public GetInfo(IJob job)
      : base(job)
    {
    }

    public override bool Validate()
    {
      throw new NotImplementedException();
    }

    public override void Start()
    {
      base.Start();

      base.Log(string.Format("Step 1 - GetInfo {0}", Thread.CurrentThread.ManagedThreadId), Enumerators.General.Log4NetType.Info);

      using (var uow = new UnitOfWork())
      {
        var test = new XPQuery<WFL_ScheduleProcess>(uow).ToList();

      }

      var processData = (ProcessDataExt.SendStatement.SendStatement)base.GetProcessData(Enumerators.Workflow.WorkflowDataExtType.SendStatment);

      base.Complete(null);
    }
  }
}
