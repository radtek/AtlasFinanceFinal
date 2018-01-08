using System;
using System.ServiceModel;

using SimpleInjector;

using Atlas.Integration.Interface;
using Atlas.Servers.Common.Logging;
using Atlas.Common.Interface;
using Atlas.Servers.Common.Config;


namespace Atlas.Server.Implementation
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class Integration : IIntegration
  {    
    static Integration()
    {    
      // DI
      RegisterDependencies();

      _log.Information("Initializing XPO...");

      // XPO
      Servers.Common.Xpo.XpoUtils.CreateXpoDomain(_config, _log, new[] { typeof(Atlas.Domain.Model.Opportunity.OPP_CaseDetail) });

      _log.Information("XPO initialized");
    }


    #region WCF implementation

    public LoginResult Login(SystemLoginRequest system, UserLoginRequest user)
    {
      return Login_Impl.Login(_log, system, user);
    }
    

    public SendSMSResult SendSMS(string loginToken, SendSMSRequest request)
    {
      return SendSMS_Impl.SendSMS(_log, _messageBus, loginToken, request);
    }


    public SendOTPResult SendOTPViaSMS(string loginToken, SendOTPRequest request)
    {
      return SendOTPViaSMS_Impl.SendOTPViaSMS(_log, _messageBus, loginToken, request);
    }
    

    public ClientLastActivityResult GetClientLastActivity(string loginToken, ClientLastActivityRequest request)
    {
      return GetClientLastActivity_Impl.GetClientLastActivity(_log, _config, loginToken, request);
    }


    public ScoreCardResult GetScoreCard(string loginToken, ScoreCardRequest request)
    {
      return GetScoreCard_Impl.GetScoreCard(_log, loginToken, request);
    }


    public AddOpportunityResult AddOpportunity(string loginToken, AddOpportunityRequest request)
    {
      return AddOpportunity_Impl.AddOpportunity(_log, loginToken, request);
    }


    public BranchListResult GetBranchList(string loginToken)
    {
      return GetBranchList_Impl.GetBranchList(_log, loginToken);
    }
    

    public UsersResult GetUsers(string loginToken)
    {
      return GetUsers_Impl.GetUsers(_log, loginToken);
    }
        

    public CheckOpportunityStatusResult CheckOpportunityStatus(string loginToken, CheckOpportunityStatusRequest request)
    {
      return CheckOpportunityStatus_Impl.CheckOpportunityStatus(_log, loginToken, request);
    }

    #endregion


    #region Private methods

    /// <summary>
    /// Register DI dependencies
    /// </summary>
    private static void RegisterDependencies()
    {
      // Infrastructure 
      // ---------------------------------------
      _container.RegisterSingleton(_log);
      _container.RegisterSingleton(_config);

      // Message bus
      _messageBus = new MessageBus.EasyNetQMessageBus(_config, _log);
      _container.RegisterSingleton(_messageBus);
    }

    #endregion


    #region Private fields

    // *Cross-cutting concerns*  we need instances upfront, so create here and register as singletons
    private static readonly ILogging _log = new SerilogLogging("Integration.svc", true);
    private static readonly IConfigSettings _config = new ConfigFileSettings();
    private static Atlas.Server.Implementation.MessageBus.IMessageBusHandler _messageBus;

    // DI
    private static readonly Container _container = new Container();

    #endregion

  }
}
