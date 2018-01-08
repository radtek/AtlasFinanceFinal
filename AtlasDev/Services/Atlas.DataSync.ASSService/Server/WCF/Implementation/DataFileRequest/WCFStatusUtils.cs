using Atlas.DataSync.WCF.Interface;


namespace ASSServer.WCF.Implementation.DataFileRequest
{
  internal static class WCFStatusUtils
  {
    /// <summary>
    /// Converts internal status to WCF status
    /// </summary>
    /// <param name="status">Internal status</param>
    /// <returns>WCF status</returns>
    internal static ProcessStatus.CurrentStatus StatusToWCFStatus(ProcessTracking.CurrentStatus status)
    {
      switch (status)
      {
        case ProcessTracking.CurrentStatus.Started:
          return ProcessStatus.CurrentStatus.Started;

        case ProcessTracking.CurrentStatus.Completed:
          return ProcessStatus.CurrentStatus.Completed;

        case ProcessTracking.CurrentStatus.Failed:
          return ProcessStatus.CurrentStatus.Failed;

        default:
          return ProcessStatus.CurrentStatus.NotSet;
      }
    }

  }
}
