using System;
using System.Configuration;

using MongoDB.Driver;


namespace Atlas.Evolution.Server.Code.MongoUtils
{
  internal class MongoDbUtils
  {
    public static MongoClient GetMongoClient()
    {
      var host = ConfigurationManager.AppSettings["mongodb-host"] ?? "10.0.0.249";
      var port = ConfigurationManager.AppSettings["mongodb-port"] ?? "27017";
      var user = ConfigurationManager.AppSettings["mongodb-user"] ?? "evolution-server";
      var pass = ConfigurationManager.AppSettings["mongodb-pass"] ?? "grt6y65hiuoodfsetbv";
      var dbName = ConfigurationManager.AppSettings["mongodb-db"] ?? "evolution";

      return new MongoClient(new MongoClientSettings
      {
        Credentials = new[] { MongoCredential.CreateCredential(dbName, user, pass) },
        ConnectionMode = ConnectionMode.Direct,
        //ConnectTimeout = TimeSpan.FromSeconds(5),
        //ReadPreference = ReadPreference.Nearest,
        Server = new MongoServerAddress(host, int.Parse(port)),
        UseSsl = false,
        ConnectTimeout = TimeSpan.FromSeconds(10),
        ReplicaSetName = "rs1",
        //Servers = new[] { new MongoServerAddress(host) },
        ServerSelectionTimeout = TimeSpan.FromSeconds(5)        
      });
    }
  }
}
