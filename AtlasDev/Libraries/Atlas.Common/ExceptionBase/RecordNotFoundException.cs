using System;


namespace Atlas.Common.ExceptionBase
{
  public class RecordNotFoundException : Exception
  {
    public RecordNotFoundException() : base() { }
    public RecordNotFoundException(string message) : base(message) { }
    public RecordNotFoundException(string message, Exception e) : base(message, e) { }
  }
}
