using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Workflow.Interface;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Workflow.Process.AtlasOnline.NewApplication.Cancel.Steps
{
  public class Cancel : Step
  {
    public Cancel(IJob job)
      : base(job)
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data">This will be ALWAYS the AccountId</param>
    /// <returns></returns>
    public override void Start(dynamic data)
    {
      base.Start();

      using (var uow = new UnitOfWork())
      {
        if (data != null)
        {
          Type dataType = data.GetType();
          var prop = dataType.GetProperty("AccountId");

          if (prop == null)
            throw new Exception("Data does not have AccountId");

          var accountId = (long)data.AccountId;
          var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
          if (account == null)
            throw new Exception("Account does not exist");

          account.Status = new XPQuery<ACC_Status>(uow).FirstOrDefault(s => s.Type == Enumerators.Account.AccountStatus.Cancelled);
          account.StatusChangeDate = DateTime.Now;


          uow.CommitChanges();
        }
      }

      base.Complete(null);
    }
  }
}
