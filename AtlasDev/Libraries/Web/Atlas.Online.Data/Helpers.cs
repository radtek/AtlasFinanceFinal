using Atlas.Enumerators;
using Atlas.Online.Data.Models.Definitions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data
{
  public sealed class Helpers : IDisposable
  {

    private UnitOfWork _uow = null;

    public Helpers(UnitOfWork uow)
    {
      _uow = uow;
    }

    public void Dispose()
    {
      _uow.CommitChanges();
      _uow = null;
    }

    public void UpdateAccountStatus(long applicationId, Account.AccountStatus status)
    {
      var application = new XPQuery<Application>(_uow).FirstOrDefault(a => a.ApplicationId == applicationId);
      if (application != null)
      {
        application.Status = status;
        application.Save();
      }
    }
  }
}
