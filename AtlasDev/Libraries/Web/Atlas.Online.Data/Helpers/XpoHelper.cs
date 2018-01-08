using System;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Atlas.Online.Data.Models.Definitions;
using System.Configuration;

public static class XpoHelper
{
  public static Session GetNewSession()
  {
    return new Session(DataLayer);
  }

  public static UnitOfWork GetNewUnitOfWork()
  {
    return new UnitOfWork(DataLayer);
  }

  private static readonly object lockObject = new object();

  static volatile IDataLayer fDataLayer;

  static IDataLayer DataLayer
  {
    get
   {
      if (fDataLayer == null)
      {
        lock (lockObject)
        {
          if (fDataLayer == null)
          {
            fDataLayer = GetDataLayer();
          }
        }
      }
      return fDataLayer;
    }
  }

  private static IDataLayer GetDataLayer()
  {
    //var a = AutoCreateOption.DatabaseAndSchema;
    var a = AutoCreateOption.None;

    IDataStore store =
      XpoDefault.GetConnectionProvider(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString,
        AutoCreateOption.DatabaseAndSchema);

    var dataLayer = new SimpleDataLayer(store);

    using (var session = new Session(dataLayer))
    {
      session.UpdateSchema();
      session.UpdateSchema(typeof (Address));
      session.CreateObjectTypeRecords();

      XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, store);
    }

    return XpoDefault.DataLayer;
  }
}