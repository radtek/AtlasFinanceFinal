using System;
using System.Diagnostics;
using System.Linq;
using System.Configuration;
using System.Threading.Tasks;

using DevExpress.Xpo;
using MongoDB.Driver;

using Atlas.Domain.Model;
using Atlas.Common.Utils;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Admin
{
  class GetStorageFile_Impl
  {
    internal static byte[] Execute(ILogging log, long storageId, bool destCompress)
    {
      var methodName = "GetStorageFile";
      log.Information("{MethodName} started: {StorageId}", methodName, storageId);

      if (storageId <= 0)
      {
        log.Warning("{MethodName}- 'storageId' parameter invalid", methodName);
        return null;
      }

      byte[] result = null;
      var timer = Stopwatch.StartNew();

      using (var unitOfWork = new UnitOfWork())
      {
        var docStorage = unitOfWork.Query<DOC_FileStore>().FirstOrDefault(s => s.StorageId == storageId);
        if (docStorage != null)
        {
          if (docStorage.StorageSystemRef != null) // Only prepared, no document loaded yet...
          {
            var task = Task.Run<byte[]>(async () =>
              {
                var mongoConnStr = ConfigurationManager.ConnectionStrings["MongoDocDb"].ConnectionString;
                var mongoClient = new MongoClient(mongoConnStr);

                var mongoDb = mongoClient.GetDatabase("document");
                var mongoDocs = mongoDb.GetCollection<Atlas.Document.Mongo.FileDocStorage>("file");
                var mongoDoc = await mongoDocs.Find(s => s.Id == new MongoDB.Bson.ObjectId(docStorage.StorageSystemRef)).FirstOrDefaultAsync();
                return (mongoDoc != null) ? mongoDoc.Document : null;

              });
            task.Wait();

            result = task.Result;
            if (task.Result == null)
            {
              log.Error("{MethodName}- 'StorageId': {StorageId} could not be located in document storage", methodName, storageId);
            }
          }
          else
          {
            log.Error("{MethodName}- 'documentId': {StorageId} not linked to a document in the store", methodName, storageId);
          }
        }
        else
        {
          log.Warning("{MethodName}- 'documentId': {StorageId} could not be located in database", methodName, storageId);
        }
      }

      log.Information("{MethodName}- {ResultLength} bytes, time: {ElapsedMS}ms", methodName, result != null ? result.Length : 0, timer.ElapsedMilliseconds);

      return (destCompress && result != null && result.Length > 0) ? Compression.InMemoryCompress(result) : result;
    }
  }
}
