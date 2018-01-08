using System;
using System.Configuration;
using System.Threading.Tasks;
using Atlas.Common.Utils;
using Stream.Upgrade.Properties;

namespace Stream.Upgrade
{
  class Program
  {
    static void Main(string[] args)
    {
      var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

      var tasks = new Task[8];

      tasks[0] = new Task(() =>
      {
        Upgrade("Debtor", connectionString, Resources.CountCasesWithoutDebtor, Resources.UpdateCasesWithoutDebtor);
      });
      tasks[1] = new Task(() =>
      {
      Upgrade("Host", connectionString, Resources.CountCasesWithoutHost, Resources.UpdateCasesWithoutHost);
      });
      tasks[2] = new Task(() =>
      {
      Upgrade("Branch", connectionString, Resources.CountCasesWithoutBranch, Resources.UpdateCasesWithoutBranch);
      });
      tasks[3] = new Task(() =>
      {
      Upgrade("Contact", connectionString, Resources.CountCasesWithoutContact, Resources.UpdateCasesWithoutContact);
      });
      tasks[4] = new Task(() =>
      {
        Upgrade("Note", connectionString, Resources.CountNotes, Resources.UpdateNote);
      });
      tasks[5] = new Task(() =>
      {
        Upgrade("Delete Transaction Notes", connectionString, Resources.CountTransactionNotesRemaining, Resources.DeleteTransactionNotes);
      });
      tasks[6] = new Task(() =>
      {
        Upgrade("Case Reference", connectionString, Resources.CountCasesWithoutReference, Resources.UpdateCasesWithoutReference);
      });
      tasks[7] = new Task(() =>
      {
        Upgrade("Account Last Import Reference", connectionString, Resources.CountCasesWithLastImportReference, Resources.UpdateCasesWithLastImportReference);
      });

      foreach (var task in tasks)
      {
        task.Start();
      }
      Task.WaitAll(tasks);
    }

    private static void Upgrade(string title, string connectionString, string countQuery, string updateQuery)
    {
      var queryUtil = new RawSql();
      var tempObject = queryUtil.ExecuteScalar(countQuery,connectionString);
      if (tempObject == null)
        return;
      var count = int.Parse(tempObject.ToString());
      Console.WriteLine(Resources.Program_Upgrade__0___remaining_records____1_, DateTime.Now, title, count);

      while (count > 0)
      {
        var result = queryUtil.ExecuteScalar(updateQuery, connectionString);

        tempObject = queryUtil.ExecuteScalar(countQuery, connectionString);
        if (tempObject == null)
          return;
        count = int.Parse(tempObject.ToString());
        Console.WriteLine(Resources.Program_Upgrade__0___remaining_records____1_, DateTime.Now, title, count);
      }

    }
  }
}
