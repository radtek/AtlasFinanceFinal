using System;


namespace Atlas.Common.ExceptionBase
{
  public class HardwareTamperException : Exception
  {
    public HardwareTamperException() : base() { }
    public HardwareTamperException(string message) : base(message) { }
    public HardwareTamperException(string message, Exception e) : base(message, e) { }
  }
}
