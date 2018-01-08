using Atlas.Workflow.Interface;
using Atlas.Workflow.Process.Statements.Steps;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Workflow.Process.Test
{
  class Program
  {
    static void Main(string[] args)
    {
      var connStr = ConfigurationManager.ConnectionStrings["Atlas"].ConnectionString;
      var dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.None);
      using (var dataLayer = new SimpleDataLayer(dataStore))
      {
        using (var session = new Session(dataLayer))
        {
          //session.CreateObjectTypeRecords();
          XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
        }
      }
      XpoDefault.Session = null;

      A a = new B();
      a.Start();

      Console.ReadLine();
    }
  }

  public class A
  {
    public virtual void Start()
    {
      Console.WriteLine("A - Start");
    }
  }

  public class B:A
  {
    public override void Start()
    {
      base.Start();
      Console.WriteLine("B - Start");
    }
  }
}
