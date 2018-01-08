using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using DevExpress.Xpo;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Policy
{
  public sealed class PolicyProcessor
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(PolicyProcessor));
    public List<Enumerators.Account.Policy> CompanyPolicies(long accountId, UnitOfWork uow)
    {
    }
  }
}
