using System;


namespace Atlas.Common.Attributes
{
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
  public class FieldNameAttribute : Attribute
  {
    private readonly string _FieldName;
    public string FieldName
    {
      get { return _FieldName; }
    }

    public FieldNameAttribute(string FieldName)
    {
      _FieldName = FieldName;
    }
  }
}
