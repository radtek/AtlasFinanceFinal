using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;

using MongoDB.Driver;
using MongoDB.Driver.Linq;
using FileHelpers;

using Atlas.Evolution.Server.Code.MongoUtils;
using Evolution.Mongo.Entity;
using Atlas.Evolution.Server.Layout;


namespace GetDoc
{
  class Program
  {
    [HandleProcessCorruptedStateExceptions]
    static void Main(string[] args)
    {
      try
      {
        var x = new blah();
        x.Do();
      }
      catch (Exception err)
      {

      }

      Console.ReadLine();
    }


  }

  class blah
  {
    public async void Do()
    {
      try
      {
        var lookup = new Dictionary<string, string>();
        using (var conn = new Npgsql.NpgsqlConnection("Server=172.31.91.165;Port=5432;Database=ass;User Id=postgres;Password=s1DT81ChqlVkPZMlRO8b;CommandTimeout=20;"))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = "SELECT cs_branch, brnum FROM company.asbranch WHERE cs_branch IS NOT NULL";
            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                lookup[rdr.GetString(0).Trim()] = rdr.GetString(1).Trim();
              }
            }
          }
        }

        var client = MongoDbUtils.GetMongoClient();
        var db = client.GetDatabase("evolution");
        var collection = db.GetCollection<Evolution_Batch_File>("batch_file");
        var xx = collection.AsQueryable<Evolution_Batch_File>();
        var tyt = xx.Where(s => s.Filename == "AT0001_ALL_T702_M_20160930_1_1.txt").FirstOrDefault(); // works now!?!
        
        // Get users
        //var command = new BsonDocument("usersInfo", 1);
        //var result = db.RunCommand<BsonDocument>(command);

        var filter = Builders<Evolution_Batch_File>.Filter.Eq(s => s.Filename, "AT0001_ALL_T702_M_20160930_1_1.txt");
        //var filter2 = Builders<Evolution_Batch_File>.Filter.Eq("Id", new ObjectId("577962f567b0000a184a840d"));

        //ObjectId("57a2421867b0002b3c4a91a0")
        var cursor = collection.Find(filter).ToCursor();
        foreach (var doc in cursor.ToEnumerable())
        {
          Console.WriteLine($"dc.Id- {doc.Filename}");

          var filename = $"D:\\temp\\{doc.Filename}";
          using (var fd = File.Create(filename))
          using (var fs = new MemoryStream(doc.File))
          using (var csStream = new GZipStream(fs, CompressionMode.Decompress))
          {
            byte[] buffer = new byte[1024];
            int nRead;
            while ((nRead = csStream.Read(buffer, 0, buffer.Length)) > 0)
            {
              fd.Write(buffer, 0, nRead);
            }
          }

          var engine = new FileHelperEngine<Monthly_Data>();
          var recs = engine.ReadFile(filename).ToList();
          recs.ForEach(s => s.BRANCH_CODE = lookup.ContainsKey(s.BRANCH_CODE.Trim()) ? lookup[s.BRANCH_CODE.Trim()] : $"ERR-{s.BRANCH_CODE.Trim()}");
          var problems = recs.Where(s => s.CURRENT_BALANCE == 0 && string.IsNullOrWhiteSpace(s.STATUS_CODE)).ToList();

          var lines = new StringBuilder();
          foreach (var problem in problems)
          {
            lines.Append($"{problem.BRANCH_CODE}-{problem.ACCOUNT_NO.Trim()}-{problem.SUB_ACCOUNT_NO.Trim()}: Balance: {problem.CURRENT_BALANCE:0.00}, Overdue: {problem.AMOUNT_OVERDUE:0.00}\r\n");
          }
          File.WriteAllText(@"D:\temp\errors.txt", lines.ToString());
        }


        //creates a filter to find Bson Document
        //var filter2 = Builders<Evolution_Batch_File>.Filter.Eq(c => c.Filename, "AT0001_ALL_T702_M_20160731_1_1.txt");
        ////runs the query to find it
        //var query = await collection.Find(filter2).ToListAsync(); // app shutdown!?!
        ////gets the customer
        //var customer = query.FirstOrDefault();

        //var x = await files.Find(filter).FirstOrDefaultAsync(); // app shutdown!?!

        //var count = files.Count(new BsonDocument("_id", "577962f567b0000a184a840d"));//  new BsonDocument());
        //var find = new BsonDocument("_id", "577962f567b0000a184a840d");

        //using (var cursor = await files.FindAsync(find)) // app shutdown!?!
        //{
        //  while (await cursor.MoveNextAsync())
        //  {
        //    var batch = cursor.Current;
        //    foreach (var document in batch)
        //    {
        //      // process document
        //      count++;
        //    }
        //  }
        //}
      }
      catch (Exception err)
      {
        Console.WriteLine(err);        
      }

      Console.WriteLine("Press a key...");
      Console.ReadLine();
    }
    
  }
}
