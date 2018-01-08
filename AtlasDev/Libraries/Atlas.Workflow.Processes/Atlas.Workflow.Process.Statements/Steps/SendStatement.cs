using Atlas.Workflow.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atlas.Workflow.Process.Statements.Steps
{
  public class SendStatement : Step
  {
    public SendStatement(IJob job)
      : base(job)
    {
    }

    public override void Start()
    {
      base.Start();

      Console.WriteLine(string.Format("Step 3 - SendStatement {0}", Thread.CurrentThread.ManagedThreadId));

      Complete(null);
    }
  }
}
