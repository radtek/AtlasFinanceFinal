using System;


namespace Atlas.Common.ExceptionBase
{
  public class NoUserException : Exception
  {
    public NoUserException() : base() { }
    public NoUserException(string message) : base(message) { }
    public NoUserException(string message, Exception e) : base(message, e) { }
  }
}
