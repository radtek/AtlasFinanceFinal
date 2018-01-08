using System;
using System.IO;

using Quartz;

using Atlas.Common.Interface;
using Atlas.DocServer.WCF.Implementation.Utils;


namespace Atlas.DocServer.QuartzTasks
{
  /// <summary>
  /// Job to delete old chunked uploaded files
  /// </summary>
  public class DeleteOldChunkedFiles : IJob
  {
    public DeleteOldChunkedFiles(ILogging log)
    {
      _log = log;
    }

    public void Execute(IJobExecutionContext context)
    {
      try
      {
        var files = Directory.GetFiles(ChunkedFileUtils.ChunkTempDir());
        foreach (var fileName in files)
        {
          var fi = new FileInfo(fileName);
          if (DateTime.Now.Subtract(fi.CreationTime).TotalHours > 48)
          {
            File.Delete(fileName);
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute");
      }
    }


    private static ILogging _log;

  }
}
