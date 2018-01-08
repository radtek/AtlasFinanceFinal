using System;
using System.Diagnostics;
using System.Threading;


namespace Atlas.RabbitMQ.Messages
{
  public class ReplyMessage<TMessage>
  {
    private readonly Stopwatch _elapsed = Stopwatch.StartNew();
    private readonly ManualResetEvent _received = new ManualResetEvent(false);

    public TMessage Message { get; private set; }

    public void Set(TMessage message)
    {
      _elapsed.Stop();

      Trace.WriteLine("Message Received After " + _elapsed.Elapsed);

      Message = message;
      _received.Set();
    }


    public bool IsAvailable(TimeSpan timeout)
    {
      return _received.WaitOne(timeout, true);
    }

  }
}