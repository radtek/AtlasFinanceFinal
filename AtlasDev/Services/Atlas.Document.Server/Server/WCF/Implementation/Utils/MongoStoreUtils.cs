using System;
using System.Configuration;
using System.Threading.Tasks;
using Atlas.Common.Utils;
using Atlas.Document.Mongo;
using MongoDB.Driver;


namespace Atlas.DocServer.WCF.Implementation.Utils
{
  internal static class MongoStoreUtils
  {
    /// <summary>
    /// Adds a new document to the mongo document store
    /// </summary>
    /// <param name="document">The document data (uncompressed)</param>
    /// <param name="documentIsCompressed">Is the source document compressed</param>
    /// <returns>The storage document id (OID/PK)</returns>
    internal static byte[] AddDocumentToStore(byte[] document, bool documentIsCompressed)
    {
      var task = Task.Run<byte[]>(async () =>
        {
          var mongoConnStr = ConfigurationManager.ConnectionStrings["MongoDocDb"].ConnectionString;
          var client = new MongoClient(mongoConnStr);
          var db = client.GetDatabase("document");
          var docs = db.GetCollection<FileDocStorage>("file");
          var docFile = new FileDocStorage()
            {              
              Document = documentIsCompressed ? Compression.InMemoryDecompress(document) : document
            };
          await docs.InsertOneAsync(docFile);
          return docFile.Id.ToByteArray();
        });

      task.Wait();
      return task.Result;
    }


    /// <summary>
    /// Gets a document from the store
    /// </summary>
    /// <param name="documentStoreRef"></param>
    /// <returns></returns>
    internal static byte[] GetDocumentFromStore(byte[] documentStoreRef)
    {
      var task = Task.Run<byte[]>(async () =>
        {
          var mongoConnStr = ConfigurationManager.ConnectionStrings["MongoDocDb"].ConnectionString;
          var client = new MongoClient(mongoConnStr);
          var db = client.GetDatabase("document");
          var mongoDocs = db.GetCollection<Atlas.Document.Mongo.FileDocStorage>("file");
          var mongoDoc = await mongoDocs.Find(s => s.Id == new MongoDB.Bson.ObjectId(documentStoreRef)).FirstOrDefaultAsync();

          return (mongoDoc != null) ? mongoDoc.Document : null;
        });

      task.Wait();
      return task.Result;
    }

  }
}
