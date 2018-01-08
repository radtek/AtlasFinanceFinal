using System;

using DevExpress.Xpo;

using Atlas.Domain.Model;


namespace Atlas.LoanEngine.Account
{
  public static class Log
  {
    /// <summary>
    /// Logs a message for historical purposes.
    /// </summary>
    public static void Save(UnitOfWork uow, ACC_Account account, string entry)
    {
      var history = new ACC_History(uow)
      {
        Account = account,
        Entry = entry,
        CreatedDate = DateTime.Now
      };
      history.Save();
      uow.CommitChanges();
    }
  }
}