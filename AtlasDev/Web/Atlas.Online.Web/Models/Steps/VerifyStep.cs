using Atlas.Enumerators;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Online.Web.Models.Steps
{
  public class VerifyStep : ApplicationStepBase
  {
    public override int Id
    {
      get { return 5; }
    }

    public override void Populate(Application application) 
    {
    }

    public override void Save(ref Application applicatio, HttpRequestBase request) { }
  }
}