using System;

using Ninject;
using Serilog;

using Atlas.RabbitMQ.Messages.Credit;
using Atlas.ThirdParty.CompuScan;
using EasyNetQ;


namespace Atlas.Credit.Engine.Core
{
  public sealed class Credit : IDisposable
  {
    #region Static Members

    private static readonly ILogger _log = Log.Logger.ForContext<Credit>();

    #endregion

    #region Private Properties

    private IKernel _kernel = null;
	  private readonly IBus _bus;

	  #endregion

    #region Constructor

    public Credit(IKernel kernel, IBus bus)
    {
	    _kernel = kernel;
	    _bus = bus;
    }

	  #endregion

    #region Public Methods

    public void Do(CreditRequest req)
    {
      _log.Information("[Credit.Do] - Performing enquiry for Account {AccountId}", req.AccountId);

      using (var func = new Functions(_kernel, _bus))
      {
        func.Perform(req);
      }

      _log.Information("[Credit.Do] - Ended performing enquiry for Account {AccountId}", req.AccountId);

    }

    public void Do(CreditRequestLegacy req)
    {
      _log.Information("[Credit.Do] - Performing enquiry for Account {AccountId}", req.AccountId);

      using (var func = new Functions(_kernel, _bus))
      {
        func.Perform(req);
      }

      _log.Information("[Credit.Do] - Ended performing enquiry for Account {AccountId}", req.AccountId);

    }

    public void RegisterClient(RegisterClient req)
    {
      _log.Information("[Credit.RegisterClient] - Registering client...");

      using (var func = new Functions(_kernel, _bus))
      {
        func.RegisterClient(req);
      }

      _log.Information("[Credit.RegisterClient] - Ended registering client");

    }

    public void RegisterLoan(RegisterLoan req)
    {
      _log.Information("[Credit.RegisterLoan] - Registering loan...");

      using (var func = new Functions(_kernel, _bus))
      {
        func.RegisterLoan(req);
      }

      _log.Information("[Credit.RegisterLoan] - Ended registering loan");

    }

    public void EnqGlobal(ENQGlobal req)
    {
      _log.Information("[Credit.ENQGlobal] - ENQGlobal...");

      using (var func = new Functions(_kernel, _bus))
      {
        func.EnqGlobal(req);
      }

      _log.Information("[Credit.ENQGlobal] - Ended ENQGlobal");

    }

    public void RegisterPayment(RegisterPayment req)
    {
      _log.Information("[Credit.RegisterPayment] - Registering payment...");

      using (var func = new Functions(_kernel, _bus))
      {
        func.RegisterPayment(req);
      }

      _log.Information("[Credit.RegisterPayment] - Ended registering payment");

    }

    public void RegisterAddress(RegisterAddress req)
    {
      _log.Information("[Credit.RegisterAddress] - Register addresss...");

      using (var func = new Functions(_kernel, _bus))
      {
        func.RegisterAddress(req);
      }

      _log.Information("[Credit.RegisterAddress] - Ended performing enquiry");

    }

    public void RegisterTelephone(RegisterTelephone req)
    {
      _log.Information("[Credit.RegisterTelephone] - Registering telephone...");

      using (var func = new Functions(_kernel, _bus))
      {
        func.RegisterTelephone(req);
      }

      _log.Information("[Credit.RegisterTelephone] - Ended registering telephone");

    }

    public void RegisterEmployer(RegisterEmployer req)
    {
      _log.Information("[Credit.RegisterEmployer] - Register employer...");

      using (var func = new Functions(_kernel, _bus))
      {
        func.RegisterEmployer(req);
      }

      _log.Information("[Credit.Do] - Ended performing enquiry");

    }

    public void UpdateClient(UpdateClient req)
    {
      _log.Information("[Credit.UpdateClient] - Updating client...");

      using (var func = new Functions(_kernel, _bus))
      {
        func.UpdateClient(req);
      }

      _log.Information("[Credit.Do] - Ended updating client");

    }

    public void UpdateLoan(UpdateLoan req)
    {
      _log.Information("[Credit.UpdateLoan] - Updating loan...");

      using (var func = new Functions(_kernel, _bus))
      {
        func.UpdateLoan(req);
      }

      _log.Information("[Credit.UpdateLoan] - Ended updating loan");

    }

    public void NLRRegisterLoan(RegisterNLRLoan req)
    {
      _log.Information("[Credit.NLRRegisterLoan] - Registering loan on NLR");

      using (var func = new Functions(_kernel, _bus))
      {
        func.NLRRegisterLoan(req);
      }

      _log.Information("[Credit.NLRRegisterLoan] - Ended registering loan on NLR");

      }

    public void NLRRegisterLoan2(RegisterNLRLoan2 req)
    {
      _log.Information("[Credit.NLRRegisterLoan2] - Registering loan2 on NLR");

      using (var func = new Functions(_kernel, _bus))
      {
        func.NLRRegisterLoan2(req);
      }

      _log.Information("[Credit.NLRRegisterLoan] - Ended registering loan2 on NLR");

    }

    public void NLRLoanClose(NLRLoanClose req)
    {
      _log.Information("[Credit.NLRLoanClose] - Closing loan on NLR");

      using (var func = new Functions(_kernel, _bus))
      {
        func.NLRLoanClose(req);
      }

      _log.Information("[Credit.NLRLoanClose] - Ended closing loan on NLR");

    }

    public void NLRBatb2(BATB2 req)
    {
      _log.Information("[Credit.Do] - Performing BATB2");

      using (var func = new Functions(_kernel, _bus))
      {
        func.NLRBatb2(req);
      }

      _log.Information("[Credit.Do] - Ended performing BATB2");

    }

    public void RequestReport(ReportRequest req)
    {
      _log.Information("[Credit.RequestReport] - Performing RequestReport");
      using (var func = new Functions(_kernel, _bus))
      {
        func.RequestReport(req);
      }
      _log.Information("[Credit.RequestReport] - Ended performing RequestReport");
    }

		//public void GetSubmissionResult(BATB2 req)
		//{
		//  _log.Info(string.Format("[Credit.Do] - Performing BATB2"));

		//  using (var func = new Functions(_kernel, _bus))
		//  {
		//    func.NLRBatb2(req);
		//  }

		//  _log.Info(string.Format("[Credit.Do] - Ended performing BATB2"));

		//}


		#endregion

		public void Dispose()
    {
      _kernel = null;
    }
  }
}
