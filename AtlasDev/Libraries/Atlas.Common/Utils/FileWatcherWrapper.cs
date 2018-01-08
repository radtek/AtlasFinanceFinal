using System;
using System.IO;
using System.Linq;


namespace Atlas.Common.Utils.FileWatcher
{
  public enum FileWatcherEvents
  {
    Changed = 1,
    Created = 2,
    Deleted = 3,
    Renamed = 4
  }

  public enum FileWatcherParams
  {
    ChangeType = 1,
    FullPath = 2
  }

  public class FileWatcherWrapper
  {
    public bool ForceFire = false; // fire the delegated method even if fire does not exist
    public bool BufferSleep = false; // sleep the event for 5 seconds before executing the delegate

    private readonly FileSystemWatcher _fileWatcher;
    private readonly Delegate _doWork;
    private readonly FileWatcherParams[] _fileWatcherParams;
    private readonly object[] _doWorkParams;
    private readonly object _lock;


    public FileWatcherWrapper(string pathToWatch, FileWatcherEvents[] fileWatcherEvents, string filter, Delegate doWork, FileWatcherParams[] fileWatcherParams, params object[] doWorkParams)
    {
      _fileWatcher = new FileSystemWatcher(pathToWatch)
      {
        EnableRaisingEvents = false,
        IncludeSubdirectories = false
      };

      if (string.IsNullOrEmpty(filter))
        _fileWatcher.Filter = filter;

      foreach (var fileWatcherEvent in fileWatcherEvents)
      {
        switch (fileWatcherEvent)
        {
          case FileWatcherEvents.Changed:
            _fileWatcher.Changed += fileWatcher_Event;
            break;
          case FileWatcherEvents.Created:
            _fileWatcher.Created += fileWatcher_Event;
            break;
          case FileWatcherEvents.Deleted:
            _fileWatcher.Deleted += fileWatcher_Event;
            break;
          case FileWatcherEvents.Renamed:
            _fileWatcher.Renamed += fileWatcher_Event;
            break;
        }
      }

      _lock = new object();
      _doWork = doWork;
      _fileWatcherParams = fileWatcherParams;
      _doWorkParams = doWorkParams;
      _fileWatcher.EnableRaisingEvents = true;
    }


    void fileWatcher_Event(object sender, FileSystemEventArgs e)
    {
      lock (_lock)
      {
        if (_doWork != null)
        {
          if (File.Exists(e.FullPath) || ForceFire)
          {
            if (BufferSleep)
              System.Threading.Thread.Sleep(5000);

            var objects = 0;
            if (_fileWatcherParams != null && _fileWatcherParams.Count() > 0)
              objects += _fileWatcherParams.Count();

            var doWorkParams = new object[objects];

            if (_fileWatcherParams != null)
            {
              for (var i = 0; i < _fileWatcherParams.Count(); i++)
              {
                switch (_fileWatcherParams[i])
                {
                  case FileWatcherParams.ChangeType:
                    doWorkParams[i] = e.ChangeType;
                    break;
                  case FileWatcherParams.FullPath:
                    doWorkParams[i] = e.FullPath;
                    break;
                }
              }
            }

            if (_doWorkParams != null)
              doWorkParams = doWorkParams.Concat(_doWorkParams).ToArray();

            if (doWorkParams.Count() > 0)
              _doWork.DynamicInvoke(doWorkParams);
            else
              _doWork.DynamicInvoke();
          }
        }
      }
    }


    public void Dispose()
    {
      _fileWatcher.Dispose();
    }
  }
}
