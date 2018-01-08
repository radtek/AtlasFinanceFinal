using Quartz;


namespace Atlas.Server.QuartzTasks
{
  internal class Test : IJob
  {
    public void Execute(IJobExecutionContext context)
    {
      System.Diagnostics.Debug.WriteLine("OK");     
    }
  }
}
