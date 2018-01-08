using System;


namespace Atlas.Common.Interface
{
  public interface ILogging
  {
    void Fatal(string messageTemplate, params object[] propertyValues);
    void Fatal(Exception exception, string messageTemplate, params object[] propertyValues);

    void Error(string messageTemplate, params object[] propertyValues);
    void Error(Exception exception, string messageTemplate, params object[] propertyValues);

    void Warning(string messageTemplate, params object[] propertyValues);
    void Warning(Exception exception, string messageTemplate, params object[] propertyValues);

    void Information(string messageTemplate, params object[] propertyValues);
    void Information(Exception exception, string messageTemplate, params object[] propertyValues);

  }
}
