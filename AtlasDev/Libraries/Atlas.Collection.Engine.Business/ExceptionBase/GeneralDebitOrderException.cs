using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Collection.Engine.Business.ExceptionBase
{
  public class GeneralDebitOrderException : Exception
  {
    public GeneralDebitOrderException() : base() { }
    public GeneralDebitOrderException(string message) : base(message) { }
    public GeneralDebitOrderException(string message, Exception e) : base(message, e) { }
  }
}
