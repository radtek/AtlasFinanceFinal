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


namespace RebuildTemplate
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        Console.WriteLine("IB Template Re-generate");
        Console.WriteLine("------------------------------");
        Console.WriteLine("  This will regenerate IB templates for a specific person in mongodb and then notify the ID matcher servers");
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

        Console.Write("Regenerate for person {0}- are you sure? ", personId);
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
                           
              var bitmaps = database.GetCollection<FPBitmap2>("fpBitmap2");
              var selBuilder = Builders<FPBitmap2>.Filter;
              var selFilter = selBuilder.Eq("PersonId", personId);
              var bmpList = new List<FPBitmap2>();
              await bitmaps.Find(selFilter).ForEachAsync(s => bmpList.Add(s));
                                               
              var templatesToAdd = new List<FPTemplate2>();

              for (var fingerId = 1; fingerId <= 10; fingerId++)
              {
                var bmps = bmpList.Where(s => s.FingerId == fingerId).ToList();
                if (bmps.Count >= 3)
                {
                  Console.WriteLine("Requesting templates for finger {0}... (found {1} bitmaps)", fingerId, bmps.Count);

                  // Create the templates from the bitmaps
                  var tempBitmapList = bmps.Take(3).Select(s => GZipUtils.DecompressToByte(s.Bitmap)).ToList();
                  var bitmapList = new List<byte[]>();
                  foreach(var bitmapBuff in tempBitmapList)
                  {
                    bitmapList.Add(bitmapBuff[0] == 0x1F && bitmapBuff[1] == 0x8b /* Double GZIPPED */ ? GZipUtils.DecompressToByte(bitmapBuff) : bitmapBuff);              
                  }
                  var template = await bus.RequestAsync<CreateTemplateRequest, CreateTemplateResponse>(new CreateTemplateRequest(bitmapList));

                  Console.WriteLine("Got template");

                  if (!string.IsNullOrEmpty(template.ErrorMessage))
                  {
                    throw new Exception(template.ErrorMessage);
                  }

                  templatesToAdd.Add(new FPTemplate2
                    {
                      CreatedDT = bmps[0].CreatedDT,
                      CreatedPersonId = bmps[0].CreatedPersonId,
                      FingerId = fingerId,
                      Orientation = 0,
                      PersonId = bmps[0].PersonId,
                      TemplateBuffer = template.Template
                    });

                  templatesToAdd.Add(new FPTemplate2
                    {
                      CreatedDT = bmps[0].CreatedDT,
                      CreatedPersonId = bmps[0].CreatedPersonId,
                      FingerId = fingerId,
                      Orientation = 1,
                      PersonId = bmps[0].PersonId,
                      TemplateBuffer = template.ReversedTemplate
                    });
                }
                else
                {
                  Console.WriteLine("Skipping finger {0}- insufficient bitmaps", fingerId);
                }
              }

              Console.WriteLine("Deleting templates...");
              var mongoTemplates = database.GetCollection<FPTemplate2>("fpTemplate2");
              var delBuilder = Builders<FPTemplate2>.Filter;
              var delFilter = delBuilder.Eq("PersonId", personId);
              var result = await mongoTemplates.DeleteManyAsync(delFilter);
              Console.WriteLine("Deleted {0} templates", result.DeletedCount);

              if (templatesToAdd.Any())
              {
                Console.WriteLine("Adding templates...");
                await mongoTemplates.InsertManyAsync(templatesToAdd);                  
                Console.WriteLine("Added {0} templates", templatesToAdd.Count);
              }
             
              // Notify server of a delete
              Console.WriteLine("Notifying ID servers of delete...");                
              var fpDeleteNotification = new FPEventNotification(2, personId);
              bus.Publish<FPEventNotification>(fpDeleteNotification, "pubsub");

              // Notify server of an Add
              Console.WriteLine("Notifying ID servers of add...");
              if (templatesToAdd.Any())
              {                  
                var fpAddNotification = new FPEventNotification(1, personId);
                bus.Publish<FPEventNotification>(fpAddNotification, "pubsub");
              }          
              bus.Dispose();         
            });

          task.Wait();
        }
      }
      catch (Exception err)
      {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine("{0}\r\n{1}\r\n{2}", err.Message, err.StackTrace, err.InnerException != null ?  err.InnerException.Message : null);
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
