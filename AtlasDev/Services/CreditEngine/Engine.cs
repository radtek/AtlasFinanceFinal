using System;
using Atlas.Credit.Engine.EasyNetQ;
using Serilog;
using Ninject;
using Quartz;
using Quartz.Impl;
using Atlas.Online.Node.Core;
using Atlas.Credit.Engine.Tasks;
using EasyNetQ;

namespace Atlas.Credit.Engine
{  
  public sealed class Engine : IService
  {
    private static ILogger _logger = Log.Logger.ForContext<Engine>();
    private static IKernel _kernel = null;
    private ISchedulerFactory _schedFact;
	  private IBus _bus;

    public Engine(IKernel kernel)
    {
      _kernel = kernel;
    }


    public void Start()
    {
      _logger.Information("[CreditEngine] - Starting Engine");

      try
      {
        _logger.Information("[CreditEngine] - Engine Started");
        _logger.Information("Setting up queue handler");

				#region Handlers
				var atlasServiceBus = _kernel.Get<AtlasServiceBus>();
	      _bus = atlasServiceBus.GetServiceBus();

				atlasServiceBus.GetServiceBus().Subscribe<RabbitMQ.Messages.Credit.CreditRequest>("queue_CreditRequest", Handle);
				atlasServiceBus.GetServiceBus().Subscribe<RabbitMQ.Messages.Credit.CreditRequestLegacy>("queue_CreditRequestLegacy", HandleLegacy);
				atlasServiceBus.GetServiceBus().Subscribe<RabbitMQ.Messages.Credit.ENQGlobal>("queue_ENQGlobal", EnqGlobal);
				atlasServiceBus.GetServiceBus().Subscribe<RabbitMQ.Messages.Credit.RegisterClient>("queue_RegisterClient", RegisterClient);
				atlasServiceBus.GetServiceBus().Subscribe<RabbitMQ.Messages.Credit.RegisterLoan>("queue_RegisterLoan", RegisterLoan);
				atlasServiceBus.GetServiceBus().Subscribe<RabbitMQ.Messages.Credit.RegisterPayment>("queue_RegisterPayment", RegisterPayment);
				atlasServiceBus.GetServiceBus().Subscribe<RabbitMQ.Messages.Credit.RegisterAddress>("queue_RegisterAddress", RegisterAddress);
				atlasServiceBus.GetServiceBus().Subscribe<RabbitMQ.Messages.Credit.RegisterTelephone>("queue_RegisterTelephone", RegisterTelephone);
				atlasServiceBus.GetServiceBus().Subscribe<RabbitMQ.Messages.Credit.RegisterEmployer>("queue_RegisterEmployer", RegisterEmployer);
				atlasServiceBus.GetServiceBus().Subscribe<RabbitMQ.Messages.Credit.UpdateClient>("queue_UpdateClient", UpdateClient);
				atlasServiceBus.GetServiceBus().Subscribe<RabbitMQ.Messages.Credit.UpdateLoan>("queue_UpdateLoan", UpdateLoan);
				atlasServiceBus.GetServiceBus().Subscribe<RabbitMQ.Messages.Credit.RegisterNLRLoan>("queue_RegisterNLRLoan", NLRRegisterLoan);
				atlasServiceBus.GetServiceBus().Subscribe<RabbitMQ.Messages.Credit.RegisterNLRLoan2>("queue_RegisterNLRLoan2", NLRRegisterLoan2);
				atlasServiceBus.GetServiceBus().Subscribe<RabbitMQ.Messages.Credit.NLRLoanClose>("queue_NLRLoanClose", NLRLoanClose);
				atlasServiceBus.GetServiceBus().Subscribe<RabbitMQ.Messages.Credit.BATB2>("queue_BATB2", NLRBatb2);
				atlasServiceBus.GetServiceBus().Subscribe<RabbitMQ.Messages.Credit.ReportRequest>("queue_ReportRequest", RequestReport);

        #endregion

        _logger.Information("Queue handler setup");
        _logger.Information("Credit Policies Caching..");

        ThirdParty.CompuScan.PolicyCache.BuildCache();

        _logger.Information("Credit Policies Cached");
        _logger.Information("Creating schedulers...");

        _schedFact = new StdSchedulerFactory(/*props*/);

        // Get a scheduler
        var sched = _schedFact.GetScheduler();
        var info = TimeZoneInfo.Local;

        #region Batch Send

        var dateBuilder = DateBuilder.NewDateInTimeZone(info).AtHourMinuteAndSecond(16, 55, 0);
        DateTimeOffset runTime = dateBuilder.Build();

        var taskBatchSend = JobBuilder.Create<DeliverBatch>().WithIdentity("BatchSend", "Tasks").Build();

        //var triggerBatchSend = TriggerBuilder.Create().WithIdentity("BatchSend").StartAt(runTime).WithSimpleSchedule(x => x.RepeatForever().WithIntervalInHours(24)).Build();

        var triggerBatchSend = TriggerBuilder.Create()
          .WithIdentity("BatchSend").WithCronSchedule("0 0/30 7-17 * * ?").StartNow()
            .Build();

        sched.ScheduleJob(taskBatchSend, triggerBatchSend);

        // Removed this - does the same job as the above
        //dateBuilder = DateBuilder.NewDateInTimeZone(info).AtHourMinuteAndSecond(20, 00, 0);
        //runTime = dateBuilder.Build();
        //var taskBatchSendLockIgnore = JobBuilder.Create<DeliverBatchIgnoreLock>().WithIdentity("BatchSendIgnoreLock", "Tasks").Build();
        //var triggerBatchSendLockIgnore = TriggerBuilder.Create().WithIdentity("BatchSendIgnoreLock").StartAt(runTime).WithSimpleSchedule(x => x.RepeatForever().WithIntervalInHours(24)).Build();
        //var triggerBatchSend = (ISimpleTrigger)TriggerBuilder.Create()
        //  .WithIdentity("BatchSend")
        //     .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInMinutes(50).WithMisfireHandlingInstructionFireNow())
        //     .StartNow()
        //    .Build();
        //sched.ScheduleJob(taskBatchSendLockIgnore, triggerBatchSendLockIgnore);

        #endregion

        #region Task Pick Up

        var taskPickUpBatch = JobBuilder.Create<PickUpBatch>().WithIdentity("PickUpBatch", "Tasks").Build();

        //var triggerBPickUpBatch = (ISimpleTrigger)TriggerBuilder.Create()
        //   .WithIdentity("PickUpBatch")
        //   .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInMinutes(50).WithMisfireHandlingInstructionFireNow())
        //   .StartNow()
        //  .Build();

        dateBuilder = DateBuilder.NewDateInTimeZone(info).AtHourMinuteAndSecond(09, 15, 0);
        runTime = dateBuilder.Build();

        var triggerBPickUpBatch = TriggerBuilder.Create().WithIdentity("PickUpBatch").StartAt(runTime).WithSimpleSchedule(x => x.RepeatForever().WithIntervalInHours(24)).Build();

        sched.ScheduleJob(taskPickUpBatch, triggerBPickUpBatch);

        #endregion

        #region Storage Clean Up

        var taskStorageCleanUp = JobBuilder.Create<StorageCleanUp>().WithIdentity("StorageCleanUp", "Tasks").Build();

        //var triggerStorageCleanUp = (ISimpleTrigger)TriggerBuilder.Create()
        //   .WithIdentity("StorageCleanUp")
        //   .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInMinutes(50).WithMisfireHandlingInstructionFireNow())
        //   .StartNow()
        //  .Build();

        dateBuilder = DateBuilder.NewDateInTimeZone(info).AtHourMinuteAndSecond(22, 00, 0);
        runTime = dateBuilder.Build();

        var triggerStorageCleanUp = TriggerBuilder.Create().WithIdentity("StorageCleanUp").StartAt(runTime).WithSimpleSchedule(x => x.RepeatForever().WithIntervalInHours(24)).Build();

        sched.ScheduleJob(taskStorageCleanUp, triggerStorageCleanUp);

        #endregion

        #region NpgSql Pool Claimer

        // Lee: removed this, not sure why there's a pool cleaner. Maybe this solves the daily restart nonsense??
        //var taskNpgSqlRelease = JobBuilder.Create<PoolReleaseTask>().WithIdentity("PoolReleaseTask", "Tasks").Build();

        //dateBuilder = DateBuilder.NewDateInTimeZone(info).AtHourMinuteAndSecond(07, 00, 0);
        //runTime = dateBuilder.Build();

        //var triggerNpgSqlRelease = TriggerBuilder.Create().WithIdentity("PoolReleaseTaskTrigger").StartAt(runTime).WithSimpleSchedule(x => x.RepeatForever().WithIntervalInHours(24)).Build();

        //sched.ScheduleJob(taskNpgSqlRelease, triggerNpgSqlRelease);

        #endregion

        #region CompuScan Error task

        var taskCompuScanErrorTask = JobBuilder.Create<CompuScanErrorTask>().WithIdentity("taskCompuScanErrorTask", "Tasks").Build();

        //var triggerCompuScanErrorTas = (ISimpleTrigger)TriggerBuilder.Create()
        //   .WithIdentity("triggerCompuScanErrorTas")
        //   .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInMinutes(50).WithMisfireHandlingInstructionFireNow())
        //   .StartNow()
        //  .Build();

        dateBuilder = DateBuilder.NewDateInTimeZone(info).AtHourMinuteAndSecond(23, 00, 0);
        runTime = dateBuilder.Build();

        var triggerCompuScanErrorTas = TriggerBuilder.Create().WithIdentity("triggerCompuScanErrorTas").StartAt(runTime).WithSimpleSchedule(x => x.RepeatForever().WithIntervalInHours(24)).Build();

        sched.ScheduleJob(taskCompuScanErrorTask, triggerCompuScanErrorTas);

        #endregion

        _logger.Information("Schedulers created.");
        sched.Start();
        _logger.Information("Start up processes...");

        #region Pre Launch

        // Lee: changed to trigger job manually
        sched.TriggerJob(new JobKey("BatchSend", "Tasks"));
        //var dbatchImpl = new Atlas.ThirdParty.CompuScan.Batch.BatchServletImpl();
        //dbatchImpl.DeliverBatch(true);
        //dbatchImpl = null;

        // Lee: changed to trigger job manually
        sched.TriggerJob(new JobKey("PickUpBatch", "Tasks"));
        //var pbatchImpl = new Atlas.ThirdParty.CompuScan.Batch.BatchServletImpl();
        //pbatchImpl.RetrieveJobStatus();
        //pbatchImpl = null;

        #endregion

        _logger.Information("Start up processes complete.");
      }
      catch (Exception exception)
      {
        _logger.Error(string.Format("[CreditEngine] - Message: {0} Inner Exception: {1} Stack: {2}",
          exception.Message + Environment.NewLine, exception.InnerException + Environment.NewLine, exception.StackTrace + Environment.NewLine));
      }
    }


    #region Handlers

    private void Handle(RabbitMQ.Messages.Credit.CreditRequest req)
    {
      using (var credit = new Core.Credit(_kernel, _bus))
      {
        credit.Do(req);
      }
    }

    private void EnqGlobal(RabbitMQ.Messages.Credit.ENQGlobal req)
    {
      using (var credit = new Core.Credit(_kernel, _bus))
      {
        credit.EnqGlobal(req);
      }
    }

    private void HandleLegacy(RabbitMQ.Messages.Credit.CreditRequestLegacy req)
    {
      using (var credit = new Core.Credit(_kernel, _bus))
      {
        credit.Do(req);
      }
    }

    public void RegisterClient(RabbitMQ.Messages.Credit.RegisterClient req)
    {
      using (var credit = new Core.Credit(_kernel, _bus))
      {
        credit.RegisterClient(req);
      }
    }

    public void RegisterLoan(RabbitMQ.Messages.Credit.RegisterLoan req)
    {
      using (var credit = new Core.Credit(_kernel, _bus))
      {
        credit.RegisterLoan(req);
      }
    }

    public void RegisterPayment(RabbitMQ.Messages.Credit.RegisterPayment req)
    {
      using (var credit = new Core.Credit(_kernel, _bus))
      {
        credit.RegisterPayment(req);
      }
    }

    public void RegisterAddress(RabbitMQ.Messages.Credit.RegisterAddress req)
    {
      using (var credit = new Core.Credit(_kernel, _bus))
      {
        credit.RegisterAddress(req);
      }
    }

    public void RegisterTelephone(RabbitMQ.Messages.Credit.RegisterTelephone req)
    {
      using (var credit = new Core.Credit(_kernel, _bus))
      {
        credit.RegisterTelephone(req);
      }
    }

    public void RegisterEmployer(RabbitMQ.Messages.Credit.RegisterEmployer req)
    {
      using (var credit = new Core.Credit(_kernel, _bus))
      {
        credit.RegisterEmployer(req);
      }
    }

    public void UpdateClient(RabbitMQ.Messages.Credit.UpdateClient req)
    {
      using (var credit = new Core.Credit(_kernel, _bus))
      {
        credit.UpdateClient(req);
      }
    }

    public void UpdateLoan(RabbitMQ.Messages.Credit.UpdateLoan req)
    {
      using (var credit = new Core.Credit(_kernel, _bus))
      {
        credit.UpdateLoan(req);
      }
    }

    public void NLRRegisterLoan(RabbitMQ.Messages.Credit.RegisterNLRLoan req)
    {
      using (var credit = new Core.Credit(_kernel, _bus))
      {
        credit.NLRRegisterLoan(req);
      }
    }

    public void NLRRegisterLoan2(RabbitMQ.Messages.Credit.RegisterNLRLoan2 req)
    {
      using (var credit = new Core.Credit(_kernel, _bus))
      {
        credit.NLRRegisterLoan2(req);
      }
    }

    public void NLRLoanClose(RabbitMQ.Messages.Credit.NLRLoanClose req)
    {
      using (var credit = new Core.Credit(_kernel, _bus))
      {
        credit.NLRLoanClose(req);
      }
    }

    public void NLRBatb2(RabbitMQ.Messages.Credit.BATB2 req)
    {
      using (var credit = new Core.Credit(_kernel, _bus))
      {
        credit.NLRBatb2(req);
      }
    }

    public void RequestReport(RabbitMQ.Messages.Credit.ReportRequest req)
    {
      using (var credit = new Core.Credit(_kernel, _bus))
      {
        credit.RequestReport(req);
      }
    }

		//public void GetSubmission(BATB2 req)
		//{
		//  using (var credit = new Credit(_kernal, _bus))
		//  {
		//    credit.GetSubmissionResult(req);
		//  }
		//}

		#endregion


		public void Stop()
    {
      _logger.Information("[CreditEngine] - Shutting Down.");
    }
  }
}
