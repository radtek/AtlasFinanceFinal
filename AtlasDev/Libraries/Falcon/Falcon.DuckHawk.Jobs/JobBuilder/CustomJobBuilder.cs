using System;
using System.Collections.Generic;
using Falcon.DuckHawk.Jobs.Attributes;
using Quartz;
using Quartz.Impl;


namespace Falcon.DuckHawk.Jobs.JobBuilder
{
  public static class CustomJobBuilder
  {
    public static List<Tuple<JobDetailImpl, ITrigger>> BuildJob(IEnumerable<Type> types)
    {
      var jobs = new List<Tuple<JobDetailImpl, ITrigger>>();

      foreach (var type in types)
      {
        var cronExpression = string.Empty;
        var jobName = string.Empty;
        var triggerName = string.Empty;
        IScheduleBuilder scheduleBuilder = null;
        var disableJob = false;

        foreach (var attribute in type.GetCustomAttributes(true))
        {
          var _type = attribute.GetType();
          if (_type == typeof(CronExpressionAttribute))
            cronExpression = ((CronExpressionAttribute)attribute).Value;
          if (_type == typeof(JobNameAttribute))
            jobName = ((JobNameAttribute)attribute).Value;
          if (_type == typeof(TriggerNameAttribute))
            triggerName = ((TriggerNameAttribute)attribute).Value;
          if (_type == typeof(ScheduleBuilderAttribute))
            scheduleBuilder = (IScheduleBuilder)type.GetProperty("Schedule").GetValue(Activator.CreateInstance(type));
          if (_type == typeof(DisableJobAttribute))
            disableJob = true;

        }

        if (!disableJob)
        {
          var jobDetail = new JobDetailImpl(jobName, type);

          ITrigger trigger = scheduleBuilder == null ?
            TriggerBuilder.Create().WithIdentity(triggerName).WithCronSchedule(cronExpression).StartNow().Build() :
            TriggerBuilder.Create().WithIdentity(triggerName).WithSchedule(scheduleBuilder).StartNow().Build();

          jobs.Add(new Tuple<JobDetailImpl, ITrigger>(jobDetail, trigger));
        }
      }
      return jobs;
    }
  }
}