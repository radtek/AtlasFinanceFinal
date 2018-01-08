using System;
using System.IO;
using System.Linq;

using Atlas.Evolution.Server.Code.MongoUtils;
using Evolution.Mongo.Entity;
using Atlas.Common.Interface;


namespace Atlas.Evolution.Server.Code.Batch
{
  internal static class BatchUtils
  {
    /// <summary>
    /// Merges files in 'tempFilesPath' and creates single merged file 'batchFilename' and adds to mongo
    /// </summary>
    /// <param name="batchFilename">The batch filename, for reference</param>
    /// <param name="tempFilesPath">Path of temporary .txt files</param>
    /// <param name="uploadBatchId">The DB EVO_UploadBatch.UploadBatchId to link mongo to, for reference</param>
    /// <returns>The Mongo record ID added </returns>
    internal static byte[] MergeFilesAndAddToMongo(ILogging log, string batchFilename, string tempFilesPath, long uploadBatchId)
    {
      var tempFilenames = Directory.GetFiles(tempFilesPath, "*.*", SearchOption.TopDirectoryOnly).ToList();
      var tempMergedFile = Path.Combine(Path.GetTempPath(), string.Format("evolution_{0}.tmp", Guid.NewGuid().ToString("N")));

      log.Information("MergeFilesAndAddToMongo- Found {Files}, merge to {Temp}", tempFilenames.Count, tempMergedFile);
      #region Merge files
      if (tempFilenames.Count > 1)
      {
        using (var output = File.Create(tempMergedFile))
        {
          foreach (var file in tempFilenames)
          {
            using (var input = File.OpenRead(file))
            {
              input.CopyTo(output);
            }
          }
        }
      }
      else if (tempFilenames.Count == 1) // single file- simply use this file
      {
        tempMergedFile = tempFilenames[0];
      }
      else
      {
        return null;
      }
      #endregion

      #region ZIP file- mongodb has limit of 16MB, unless we go gridfs (!!!). Monthly is 33MB uncompressed...
      var data = Atlas.Common.Utils.Compression.InMemoryCompress(File.ReadAllBytes(tempMergedFile));
      #endregion

      #region Add merged file to mongo
      var client = MongoDbUtils.GetMongoClient();
      var db = client.GetDatabase("evolution");
      var files = db.GetCollection<Evolution_Batch_File>("batch_file");
      var batch_file = new Evolution_Batch_File()
      {
        CreatedDT = DateTime.Now,
        File = data,
        UploadBatchId = uploadBatchId,                      // link to db- superfluous?
        Filename = batchFilename
      };
      files.InsertOne(batch_file);
      log.Information("MergeFilesAndAddToMongo- Successfully added to storage, ref: {Reference}", batch_file.Id);
      #endregion

      // Clean up- non-critical
      try
      {
        log.Information("Temp dir: {TempDir}", tempFilesPath);
        log.Information("Merged file: {MergedFile}", tempMergedFile);
        Directory.Delete(tempFilesPath, true);
        if (File.Exists(tempMergedFile))
        {
          File.Delete(tempMergedFile);
        }
      }
      catch { }

      return batch_file.Id.ToByteArray();
    }
  }
}
