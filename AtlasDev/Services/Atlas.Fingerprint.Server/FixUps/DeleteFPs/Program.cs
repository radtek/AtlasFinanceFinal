using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using EasyNetQ;
using MongoDB.Driver;

using Atlas.MongoDB.Entities;
using Atlas.FP.Identifier.MessageTypes.PubSub;
using Atlas.FP.Identifier.MessageTypes.RequestResponse;
using Atlas.Common.Utils;


namespace DeleteFPs
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        Console.WriteLine("IB Delete");
        Console.WriteLine("------------------------------");
        Console.WriteLine("  This will remove invalid bitmaps and templates for a specific person in mongodb and then notify ID matchers");
        Console.WriteLine("");

        Console.Write("SA ID number (leave blank if you know PersonId):");
        var id = Console.ReadLine();

        long personId = 0;
        if (string.IsNullOrEmpty(id))
        {
          Console.Write("PersonId (required): ");
          var person = Console.ReadLine();
          
          if (!Int64.TryParse(person, out personId))
          {
            throw new Exception("Invalid person entry");
          }
        }

        if (personId == 0)
        {
          using (var conn = new Npgsql.NpgsqlConnection("Server=172.31.91.165;User ID=postgres;Password=s1DT81ChqlVkPZMlRO8b;Database=atlas_core"))
          {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
              cmd.CommandText = "SELECT \"PersonId\" FROM \"PER_Person\" WHERE \"IdNum\" = @IdNum LIMIT 1";
              cmd.Parameters.AddWithValue("IdNum", id);
              cmd.CommandType = System.Data.CommandType.Text;
              var dbPersonId = cmd.ExecuteScalar();
              if (dbPersonId != null && dbPersonId is Int64)
              {
                personId = (Int64)dbPersonId;
              }
              else
              {
                throw new Exception("Unable to locate ID");
              }
            }
          }
        }

        Console.Write("Delete all fingerprint details for person {0}- are you sure? ", personId);
        var key = Console.ReadKey();
        Console.WriteLine();

        if (key.KeyChar == 'Y' || key.KeyChar == 'y')
        {
          Console.WriteLine("Connecting to mongo...");
          var client = GetMongoClient();
          var database = client.GetDatabase("fingerprint");
          var task = Task.Run(async () =>
          {
            var address = ConfigurationManager.AppSettings["fp-rabbitmq-address"];
            var virtualHost = ConfigurationManager.AppSettings["fp-rabbitmq-virtualhost"];
            var userName = ConfigurationManager.AppSettings["fp-rabbitmq-username"];
            var password = ConfigurationManager.AppSettings["fp-rabbitmq-password"];

            var connectionString = string.Format("host={0};virtualHost={1};username={2};password={3};persistentMessages=false;timeout=30;product=fp.server", address, virtualHost, userName, password);
            var bus = RabbitHutch.CreateBus(connectionString);

            Console.WriteLine("Deleting bitmaps...");
            var bitmaps = database.GetCollection<FPBitmap2>("fpBitmap2");
            var delBitmapBuilder = Builders<FPBitmap2>.Filter;
            var delBitmapFilter = delBitmapBuilder.Eq("PersonId", personId);
            var delBitmaps = await bitmaps.DeleteManyAsync(delBitmapFilter);
            Console.WriteLine("Deleted {0} bitmaps", delBitmaps.DeletedCount);
     
            Console.WriteLine("Deleting templates...");
            var mongoTemplates = database.GetCollection<FPTemplate2>("fpTemplate2");
            var delTemplateBuilder = Builders<FPTemplate2>.Filter;
            var delTemplateFilter = delTemplateBuilder.Eq("PersonId", personId);
            var delTemplates = await mongoTemplates.DeleteManyAsync(delTemplateFilter);
            Console.WriteLine("Deleted {0} templates", delTemplates.DeletedCount);
                       
            // Notify server of a delete
            Console.WriteLine("Notifying ID servers of delete...");
            var fpDeleteNotification = new FPEventNotification(2, personId);
            bus.Publish<FPEventNotification>(fpDeleteNotification, "pubsub");
                  
            bus.Dispose();           
          });

          task.Wait();
        }
      }
      catch (Exception err)
      {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine("{0}\r\n{1}\r\n{2}", err.Message, err.StackTrace, err.InnerException != null ? err.InnerException.Message : null);
        Console.BackgroundColor = ConsoleColor.Black;
      }

      Console.WriteLine("Press a key...");
      Console.ReadKey();
    }
    

    public static MongoClient GetMongoClient()
    {
      var host = ConfigurationManager.AppSettings["mongodb-host"] ?? "172.31.75.38";
      var port = ConfigurationManager.AppSettings["mongodb-port"] ?? "27017";
      var user = ConfigurationManager.AppSettings["mongodb-user"];
      var pass = ConfigurationManager.AppSettings["mongodb-pass"];
      var dbName = ConfigurationManager.AppSettings["mongodb-db"] ?? "fingerprint";

      return new MongoClient(new MongoClientSettings
      {
        Credentials = new[] { MongoCredential.CreateCredential(dbName, user, pass) },
        ConnectionMode = ConnectionMode.Automatic,
        ConnectTimeout = TimeSpan.FromSeconds(5),
        ReadPreference = ReadPreference.Nearest,
        Server = new MongoServerAddress(host, int.Parse(port)),
        UseSsl = false
      });
    }


  }
}
