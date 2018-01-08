using System;
using System.Linq;
using System.IO;
using System.Diagnostics;

using Quartz;
using DevExpress.Xpo;
using MongoDB.Driver;
using MongoDB.Bson;
using Renci.SshNet;

using Evolution.Mongo.Entity;
using Atlas.Domain.Model.Evolution;
using Atlas.Evolution.Server.Code.MongoUtils;
using Atlas.Evolution.Server.Code.PGP;
using Atlas.Common.Interface;


namespace Atlas.Evolution.Server.QuartzTasks
{
  internal class UploadPendingBatches : IJob
  {
    public UploadPendingBatches(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Execute(IJobExecutionContext context)
    {
      return; /// TODO: Remove when ready

      var methodName = $"{nameof(UploadPendingBatches)}.{nameof(Execute)}";
      try
      {
        _log.Information("[{MethodName}] Starting", methodName);
        using (var uow = new UnitOfWork())
        {
          var expiresBefore = DateTime.Now.Subtract(TimeSpan.FromDays(21));

          var pending = uow.Query<EVO_UploadBatch>()
            .Where(s => s.UploadedDate == null && s.CollateDate > expiresBefore && s.StorageSystemRef != null)
            .OrderBy(s => s.LastUploadAttempt)
            .ToList();
          _log.Information("[{MethodName}] Found {Pending} new", methodName, pending.Count);

          if (pending.Any())
          {
            var client = MongoDbUtils.GetMongoClient();
            var db = client.GetDatabase("evolution");
            var files = db.GetCollection<Evolution_Batch_File>("batch_file");

            var selBuilder = Builders<Evolution_Batch_File>.Filter;
            foreach (var batch in pending)
            {
              batch.LastUploadAttempt = DateTime.Now;
              batch.UploadAttemptCount = batch.UploadAttemptCount + 1;
              uow.CommitChanges();

              // Get file from mongo
              var id = new ObjectId(batch.StorageSystemRef);
              var selFilter = selBuilder.Eq("Id", id);
              var batchFiles = files.FindSync(selFilter);
              var batchFile = batchFiles.FirstOrDefault();

              if (batchFile != null)
              {
                var txtFilename = Path.Combine(Path.GetTempPath(), batch.StorageFileName);
                var pgpFilename = Path.ChangeExtension(txtFilename, ".txt.pgp");
                var uploaded = false;
                try
                {
                  try
                  {
                    _log.Information("[{MethodName}] Getting file...", methodName);
                    var fileData = batchFile.File.IsGZipCompressed() ? Atlas.Common.Utils.Compression.InMemoryDecompress(batchFile.File) : batchFile.File;
                    File.WriteAllBytes(txtFilename, fileData);

                    _log.Information("[{MethodName}] Encrypting file...", methodName);
                    var encrypted = PgpEncrypt.EncryptFile(_log, _config.GetCustomSetting("", "PgpPublicKeyFolder", false), txtFilename, pgpFilename);
                    if (encrypted)
                    {
                      var pgpFileInfo = new FileInfo(pgpFilename);
                      _log.Information("[{MethodName}] Uploading PGP file {@PGPFile}...", methodName, new { pgpFileInfo.FullName, pgpFileInfo.Length });

                      var timer = Stopwatch.StartNew();
                      using (var ftpClient = new SftpClient(GetSftpConnection))
                      {
                        ftpClient.Connect();
                        var rootDir = _config.GetCustomSetting("", "SFTP_RootPath", false);
                        if (!string.IsNullOrEmpty(rootDir))
                        {
                          ftpClient.ChangeDirectory(rootDir);
                        }
                        using (var source = File.OpenRead(pgpFilename))
                        {
                          ftpClient.UploadFile(source, Path.GetFileName(pgpFilename));
                        }
                      }
                      timer.Stop();
                      uploaded = true;

                      _log.Information("[{MethodName}] Uploaded {File} in {Timer}ms", methodName, Path.GetFileName(pgpFilename), timer.ElapsedMilliseconds);
                    }
                    else
                    {
                      _log.Error("Failed to encrypt file- no file could be uploaded (HINT: check public keys are available and up-to-date)!");
                    }
                  }
                  catch (Exception err)
                  {
                    _log.Error(err, "Failed to upload file");
                  }
                }
                finally
                {
                  if (File.Exists(txtFilename))
                  {
                    File.Delete(txtFilename);
                  }
                  if (File.Exists(pgpFilename))
                  {
                    File.Delete(pgpFilename);
                  }
                }
                if (uploaded)
                {
                  batch.UploadedDate = DateTime.Now;
                  uow.CommitChanges();
                }
              }
              else
              {
                _log.Error("[{MethodName}] Failed to locate file in storage: Name: {Name}, ID: {ID}", methodName, batch.StorageFileName, batch.UploadBatchId);
              }
            }
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, methodName);
      }

      _log.Information("[{MethodName}] Task complete", methodName);
    }


    /// <summary>
    /// SFTP connection parameters
    /// </summary>
    private ConnectionInfo GetSftpConnection
    {
      get
      {
        var host = _config.GetCustomSetting("", "Sftp_Host", false);
        var port = _config.GetCustomSettingInt("", "Sftp_Port", false, 22);
        var userName = _config.GetCustomSetting("", "Sftp_UserName", false);
        var password = _config.GetCustomSetting("", "Sftp_Password", false);
        return new ConnectionInfo(host, port, userName, new AuthenticationMethod[] { new PasswordAuthenticationMethod(userName, password) });
      }
    }


    private readonly ILogging _log;
    private readonly IConfigSettings _config;

  }


  static class GZipUtils
  {
    public static bool IsGZipCompressed(this byte[] data)
    {
      return data != null && data.Length > 3 && data[0] == 0x1f && data[1] == 0x8b;
    }
  }
}
