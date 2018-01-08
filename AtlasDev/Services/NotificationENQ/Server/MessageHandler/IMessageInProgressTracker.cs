namespace NotificationServerENQ.MessageHandler
{
  /// <summary>
  /// Handles messages currently being processed by various processes, to avoid overlapped sending
  /// </summary>
  internal interface IMessageInProgressTracker
  {
    void SetInProgress(long recId, bool inProgress);

    bool IsInProgress(long recId);

  }
}
