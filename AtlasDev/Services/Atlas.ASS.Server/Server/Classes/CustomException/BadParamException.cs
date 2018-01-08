using System;
using Atlas.Enumerators;

namespace Atlas.Server.Classes.CustomException
{
  /// <summary>
  /// 'Bad parameter' exception
  /// </summary>
  internal class BadParamException : Exception
  {   
    public BadParamException(string message) : base(message)
    {
    }

    public BadParamException(string message, Exception inner) : base(message, inner)
    {
    }

  }

}
