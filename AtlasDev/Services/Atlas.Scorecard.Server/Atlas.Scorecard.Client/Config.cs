using System;

namespace Atlas.ThirdParty.CS.Bureau.Client
{
  static internal class Config
  {
    static internal string ScorecardServerAddress
    {
      get { return System.Configuration.ConfigurationManager.AppSettings["ScorecardServerWCFAddress"]; }
    }
  }
}
