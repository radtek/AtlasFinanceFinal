using System;


namespace Atlas.Common.ExceptionBase
{
  public class NoAddressException : Exception
  {
    public NoAddressException() : base() { }
    public NoAddressException(string message) : base(message) { }
    public NoAddressException(string message, Exception e) : base(message, e) { }
  }
}
