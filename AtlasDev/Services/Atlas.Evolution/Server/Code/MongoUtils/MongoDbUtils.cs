using System;
using System.Configuration;

using MongoDB.Driver;


namespace Atlas.Evolution.Server.Code.MongoUtils
{
  internal class MongoDbUtils
  {
    public static MongoClient GetMongoClient()
    {
      var host = ConfigurationManager.AppSettings["mongodb-host"] ?? "169.254.0.2";
      var port = ConfigurationManager.AppSettings["mongodb-port"] ?? "27017";
      var user = ConfigurationManager.AppSettings["mongodb-user"];
      var pass = ConfigurationManager.AppSettings["mongodb-pass"];
      var dbName = ConfigurationManager.AppSettings["mongodb-db"] ?? "evolution";

      return new MongoClient(new MongoClientSettings
      {
        Credentials = new[] { MongoCredential.CreateCredential(dbName, user, pass) },
        ConnectionMode = ConnectionMode.Automatic,
        ConnectTimeout = TimeSpan.FromSeconds(5),
        ReadPreference =  ReadPreference.Nearest,       
        Server = new MongoServerAddress(host, int.Parse(port)),
        UseSsl = false
      });
    }
  }
}
