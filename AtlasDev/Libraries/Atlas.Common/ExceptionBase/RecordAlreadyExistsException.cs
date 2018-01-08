using System;


namespace Atlas.Common.ExceptionBase
{
  public class RecordAlreadyExistsException : Exception
  {
    public RecordAlreadyExistsException() : base() { }
    public RecordAlreadyExistsException(string message) : base(message) { }
    public RecordAlreadyExistsException(string message, Exception e) : base(message, e) { }
  }
}
