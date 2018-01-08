using System;
using System.Configuration;
using System.Linq;

using DevExpress.Xpo;
using DevExpress.Xpo.DB;

using Atlas.Domain.Model;


namespace Atlas.Data.UpgradeDomain
{
  class Program
  {
    static void Main()
    {
      var connStr = ConfigurationManager.ConnectionStrings["Atlas"] != null ? ConfigurationManager.ConnectionStrings["Atlas"].ConnectionString : null;
      if (connStr == null)
      {
        Console.WriteLine("..<connectionStrings><add name=\"Atlas\">...  ----> no value set in config file!");
        Console.WriteLine("Press key to exit...");
        Console.ReadKey();
        return;
      }

      Console.WriteLine("This will update the following database with the current domain:\r\n'{0}'", connStr);
      Console.BackgroundColor = ConsoleColor.Yellow;
      Console.ForegroundColor = ConsoleColor.Red;
      Console.Write("Are you sure (Y/N)?");
      Console.ForegroundColor = ConsoleColor.White;
      Console.BackgroundColor = ConsoleColor.Black;
      var answer = Console.ReadKey();
      Console.WriteLine();

      if (answer.KeyChar == 'Y' || answer.KeyChar == 'y')
      {
        Console.Write("Update in progress...");
        try
        {
          var dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.SchemaOnly);
          using (var dataLayer = new SimpleDataLayer(dataStore))
          {
            using (var session = new Session(dataLayer))
            {
              session.UpdateSchema();
              session.CreateObjectTypeRecords();
              XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
            }
          }

          // Pull in the domain classes...
          var person = new PER_Person();

          // Testing
          using (var unitOfWork = new UnitOfWork())
          { 
            var fraud = unitOfWork.Query<FRD_Case>().FirstOrDefault();// new FRD_Case(unitOfWork);
            if (fraud != null)
            {
              foreach (var fraudPerson in fraud.CasePersons)
              {
                Console.WriteLine(fraudPerson.PersonId.Lastname);
              }
            }

            //var x = unitOfWork.Query<ALT_LogEDOInstalmentCancel>().FirstOrDefault();
            //if (x == null)
            //{
            //  x = new ALT_LogEDOInstalmentCancel(unitOfWork);
            //  x.CreatedDT = DateTime.Now;
            //  x.InstalmentNum = 1;
            // 
            //  unitOfWork.CommitChanges();
            //  if (x.LogEDOInstalmentCancelId != 1)
            //  {

            //  }
            //  x.Delete();
            //  unitOfWork.CommitChanges();
            //}
            //var fraud =  new FRD_Case(unitOfWork);

            /*
            fraud.CreatedDT = DateTime.Now;
            fraud.Comment = "Comment";
            fraud.CaseReference = "Pieterville Dec 2015";
                        
            var newPerson = new FRD_Person(unitOfWork);
            newPerson.Person = person;
            newPerson.Case = fraud;
            newPerson.CreatedBy = person;
            newPerson.CreatedDT = DateTime.Now;

            unitOfWork.CommitChanges();
             * */
          }
          
          Console.WriteLine();
          Console.WriteLine("   Upgrade completed successfully");
        }
        catch (Exception err)
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine("Error: {0}", err);
        }
      }
      else
      {
        Console.BackgroundColor = ConsoleColor.Yellow;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine("Upgrade aborted!");
      }

      Console.BackgroundColor = ConsoleColor.Black;
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine("Press a key to exit...");
      Console.ReadKey();
    }
  }
}
