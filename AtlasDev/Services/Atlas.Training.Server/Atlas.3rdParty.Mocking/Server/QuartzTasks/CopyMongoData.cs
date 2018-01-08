using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using Quartz;
using Serilog;

using Atlas.MongoDB.Entities;


namespace Atlas.Server.Training.QuartzTasks
{
  [DisallowConcurrentExecution]
  internal class CopyMongoData : IJob
  {
    public void Execute(IJobExecutionContext context)
    {
      var methodName = "CopyMongoData.Execute";
      _log.Information("{MethodName}- Starting", methodName);
      try
      {
        //CopyIdPhotos().Wait();
        CopyFingerprints().Wait();
      }
      catch (Exception err)
      {
        _log.Error(err, "{MethodName}", methodName);
      }

      _log.Information("{MethodName}- Completed", methodName);
    }


    ///// <summary>
    ///// Copy ID Photos using mongodb driver v2
    ///// </summary>
    ///// <returns></returns>
    //private Task CopyIdPhotos()
    //{      
    //  return Task.Run(async () =>
    //    {         
    //      string methodName = "CopyIdPhotos";
    //      var dest = new MongoClient(); // defaults to localhost          
    //      await dest.DropDatabaseAsync("compuScan").ConfigureAwait(false);
    //      var destIdDb = dest.GetDatabase("compuScan");

    //      #region Create indexes for ID Photo
    //      var destIdPhotos = destIdDb.GetCollection<IDPhoto>("idPhoto");
    //      var idPhotoKeys = Builders<IDPhoto>.IndexKeys.Ascending(x => x.IdOrPassport);
    //      await destIdPhotos.Indexes.CreateOneAsync(idPhotoKeys, new CreateIndexOptions() { Unique = true }).ConfigureAwait(false);
    //      #endregion

    //      #region Copy across some recent pictures
    //      var sourceConnectionString = ConfigurationManager.AppSettings["MongoSourceDB"];
    //      var sourceDb = new MongoClient(sourceConnectionString);
    //      var sourceIDDb = sourceDb.GetDatabase("compuScan");

    //      var filterBuilder = Builders<IDPhoto>.Filter;
    //      var filter = filterBuilder.Gt(s => s.CreatedDT, DateTime.Now.Subtract(TimeSpan.FromDays(5)));
    //      var idCollection = sourceIDDb.GetCollection<IDPhoto>("idPhoto");
    //      var sourceIdPhotos = idCollection.Find(new BsonDocument()).SortByDescending(s => s.CreatedDT).Limit(10);
    //      await sourceIdPhotos.ForEachAsync(async s =>
    //        {
    //          await destIdPhotos.InsertOneAsync(s).ConfigureAwait(false);
    //          _log.Information("{MethodName}- Copied ID Photo: {@IdPhoto}", methodName, s);
    //        }).ConfigureAwait(false);
    //      #endregion     
    //    });
    //}


    /// <summary>
    /// Copy Celia's thumbprints
    /// </summary>
    /// <returns></returns>
    private Task CopyFingerprints()
    {
      return Task.Run(async () =>
        {
          string methodName = "CopyFingerprints";
          var destClient = new MongoClient();
          await destClient.DropDatabaseAsync("fingerprint").ConfigureAwait(false);
          var destFPDb = destClient.GetDatabase("fingerprint");

          #region Create table and indexes for fingeprint Templates
          var destTemplates = destFPDb.GetCollection<FPTemplate>("fpTemplate");
          var personKey = Builders<FPTemplate>.IndexKeys.Ascending(s => s.PersonId);          
          var options = new CreateIndexOptions() { Name = "idxPersonId", Unique = false };
          await destTemplates.Indexes.CreateOneAsync(personKey, options).ConfigureAwait(false);

          var fingerKey = Builders<FPTemplate>.IndexKeys.Ascending(s => s.PersonId).Ascending(s => s.FingerId);//.Ascending(s => s.ImageOrientation);
          options = new CreateIndexOptions() { Name = "idxPersonIdFingerId", Unique = false };
          await destTemplates.Indexes.CreateOneAsync(fingerKey, options).ConfigureAwait(false);
          #endregion
                    
          #region Copy across just Celia's + Keith's thumbs
          var sourceConnectionString = ConfigurationManager.AppSettings["MongoSourceDB"];
          var sourceClient = new MongoClient(sourceConnectionString);
          var sourceFPDb = sourceClient.GetDatabase("fingerprint");
          var fpCeliasThumbs = sourceFPDb.GetCollection<FPTemplate>("fpTemplate").Find(s => (s.FingerId == 1|| s.FingerId == 6) && _copyPersonIds.Contains(s.PersonId));

          await fpCeliasThumbs.ForEachAsync(async (s) =>
            {
              await destTemplates.InsertOneAsync(s).ConfigureAwait(false);
              _log.Information("{MethodName}- Copied FP template: {@FPTemplate}", methodName, s);
            }).ConfigureAwait(false);
          
          #endregion
        });     
    }


    #region Private fields

    /// <summary>
    /// Logging
    /// </summary>
    private static readonly ILogger _log = Log.ForContext<CopyMongoData>();

    /// <summary>
    /// PersonId's to copy across
    /// </summary>
    private static readonly List<Int64> _copyPersonIds = new List<long> { 105146 /* Celia */, 105107 /* Keith */ };

    #endregion

  }
}
