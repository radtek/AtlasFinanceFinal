using Atlas.Domain.Model;
using Atlas.Workflow.Interface;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atlas.Workflow.Process.Statements.Steps
{
  public class GenerateReport:Step
  {
    public GenerateReport(IJob job)
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

      Console.WriteLine(string.Format("Step 2 - Generate Report {0}", Thread.CurrentThread.ManagedThreadId));

      Complete(null);
    }

    public override void Stop()
    {
      throw new NotImplementedException();
    }
  }
}
