using Atlas.Common.Utils.FileWatcher;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.PayoutEngine.Business;
using RTC = Atlas.PayoutEngine.Business.RTC;
using DevExpress.Xpo;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Atlas.Payout.Engine.EasyNetQ;
using Atlas.RabbitMQ.Messages.Push;
using EasyNetQ;
using Ninject;

namespace Atlas.Payout.Engine.FileWatchers
{
  public class PayoutFileWatcher
  {
	  private readonly IKernel _kernel;

	  private delegate void Work(string filePath, PYT_ServiceDTO serviceDto);
    private delegate void Work1(string filePath, PYT_ServiceDTO serviceDto);
    private HashSet<FileWatcherWrapper> _watcherCollection;
    private static ILogger _logger;
		private IBus _bus;

		public PayoutFileWatcher(IKernel kernel)
    {
	    _kernel = kernel;
      _watcherCollection = new HashSet<FileWatcherWrapper>();
    }

    public void StartWatching(ILogger ilogger)
    {
      _logger = ilogger;

      using (var uoW = new UnitOfWork())
      {
        // Get Enabled Payout Services

        _logger.Info("Retrieving Payout Service(s)");

        var services = new XPQuery<PYT_Service>(uoW).Where(s => s.Enabled);
        if (!services.Any())
        {
          _logger.Warn("Retrieved 0 Service(s)");
          return;
        }

        _logger.Info($"Retrieved {services.Count()} Service(s)");

        // Get FilePaths to watch
        foreach (var service in services)
        {
          var serviceDto = AutoMapper.Mapper.Map<PYT_Service, PYT_ServiceDTO>(service);

          if (!string.IsNullOrEmpty(service.IncomingPath))
          {
            Work1 work = ImportRtcFile;

	          var watcher = new FileWatcherWrapper(service.IncomingPath,
		          new[]
		          {
			          FileWatcherEvents.Changed,
			          FileWatcherEvents.Created,
			          FileWatcherEvents.Renamed
		          },
		          string.Empty,
		          work,
		          new[]
		          {
			          FileWatcherParams.FullPath
		          }, serviceDto) {BufferSleep = true};

	          // Add to _watcherCollection
            _watcherCollection.Add(watcher);

            _logger.Info($"Watching folder: {service.IncomingPath}");
          }

          if (!string.IsNullOrEmpty(service.ArchivePath))
          {
            Work work = Archivefile;

	          var watcher = new FileWatcherWrapper(service.ArchivePath,
		          new[]
		          {
			          FileWatcherEvents.Changed,
			          FileWatcherEvents.Created,
			          FileWatcherEvents.Renamed
		          },
		          string.Empty,
		          work,
		          new[]
		          {
			          FileWatcherParams.FullPath
		          }, serviceDto) {BufferSleep = true};

	          // Add to _watcherCollection
            _watcherCollection.Add(watcher);

            _logger.Info($"Watching folder: {service.ArchivePath}");
          }

          if (!string.IsNullOrEmpty(service.OutgoingPath))
          {
            Work work = FilePickup;

	          var watcher = new FileWatcherWrapper(service.OutgoingPath,
		          new[]
		          {
			          FileWatcherEvents.Deleted
		          },
		          string.Empty,
		          work,
		          new[]
		          {
			          FileWatcherParams.FullPath
		          }, serviceDto)
	          {
		          BufferSleep = true,
		          ForceFire = true
	          };

	          // Add to _watcherCollection
            _watcherCollection.Add(watcher);

            _logger.Info($"Watching folder: {service.OutgoingPath}");
          }
        }
      }
    }

    void Archivefile(string filePath, PYT_ServiceDTO service)
    {
      // Archive
      _logger.Info("Started Archiving File");
      try
      {
        var fileInfo = new FileInfo(filePath);
        var archiveFile = CreateArchiveSubDirectories(service.ArchivePath) + fileInfo.Name;
        var inc = 1;
        while (File.Exists(archiveFile))
        {
          archiveFile =
	          $"{CreateArchiveSubDirectories(service.ArchivePath)}{fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf(".", StringComparison.Ordinal))}_{inc}{fileInfo.Extension}";
          inc++;
        }
        fileInfo.MoveTo(archiveFile);

        _logger.Info("Finished Archiving File");
      }
      catch (Exception exception)
      {
        _logger.Error($"Error Archiving File: {exception.Message} {exception.InnerException}");
      }
    }

    void ImportRtcFile(string filePath, PYT_ServiceDTO service)
    {
      // Import
      _logger.Info("Started Importing File");
      try
      {
        var responses = RTC.Helper.Import(filePath);
        var transmissionIds = responses.Select(r => r.TransmissionId).Distinct().ToArray();

        var utility = new Utility(service.ServiceId);
        foreach (var transmissionId in transmissionIds)
        {
          utility.UpdateTransmissionReply(transmissionId, true, string.Empty, filePath, false, true);
        }

        var transmissionReply = new Dictionary<long, Tuple<long, bool, string>>();
        responses.ForEach(p =>
          {
            transmissionReply.Add(p.PayoutId, new Tuple<long, bool, string>(p.TransmissionId, p.Accepted, p.ReplyCode));
          });

        utility.UpdateTransmissionTransactionReplies(transmissionReply);

        var payoutReplies = new Dictionary<long, Tuple<string, string>>();
        responses.ForEach(p =>
        {
          payoutReplies.Add(p.PayoutId, new Tuple<string, string>(p.ReplyCode, p.Reason));
        });

        var batchResults = utility.ImportPayoutReplies(payoutReplies);

        foreach (var batchResult in batchResults)
        {
          utility.CloseBatch(batchResult.Key);
        }

        Archivefile(filePath, service);

        var accounts = utility.GetAccountsWithPayRules(payoutReplies.Select(p => p.Key).ToArray());
        foreach (var account in accounts)
        {
          var pushMessage = new PushMessage(Magnum.CombGuid.Generate())
            {
              Type = PushMessage.PushType.Payout
            };

          pushMessage.Parameters.Add("AccountId", account.Key.AccountId);

					var atlasServiceBus = _kernel.Get<AtlasServiceBus>();
					_bus = atlasServiceBus.GetServiceBus();
					_bus.Publish(pushMessage);
        }

	      _logger.Info("Finished Importing File");
      }
      catch (Exception exception)
      {
        _logger.Error($"Error Importing File: {exception.Message} {exception.InnerException}");
      }
    }

    void FilePickup(string filePath, PYT_ServiceDTO service)
    {
      // Update
      _logger.Info("Started Importing File");
      try
      {
        if (!File.Exists(filePath))
        {
          var batchId = long.Parse(filePath.Substring(filePath.LastIndexOf('_') + 1, filePath.LastIndexOf('.') - (filePath.LastIndexOf('_') + 1)));

          var utility = new Utility(service.ServiceId);
          utility.UpdateBatch_FilePickedUp(batchId);
        }

        _logger.Info("Finished Importing File");
      }
      catch (Exception exception)
      {
        _logger.Error($"Error Importing File: {exception.Message} {exception.InnerException}");
      }
    }

    private string CreateArchiveSubDirectories(string path)
    {
      var archivePath = path + DateTime.Today.ToString("/yyyy/MM/dd/");
      if (!Directory.Exists(archivePath))
      {
        Directory.CreateDirectory(archivePath);
      }
      return archivePath;
    }

    ~PayoutFileWatcher()
    {
      // Dispose ALL Watchers
      foreach (var watchers in _watcherCollection)
      {
        watchers.Dispose();
      }

      _watcherCollection.Clear();
      _watcherCollection = null;
    }
  }
}