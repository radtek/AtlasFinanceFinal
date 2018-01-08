using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_DataExtType : XPLiteObject
  {
    private int _dataExtTypeId;
    [Key(AutoGenerate = false)]
    public int DataExtTypeId
    {
      get
      {
        return _dataExtTypeId;
      }
      set
      {
        SetPropertyValue("DataExtTypeId", ref _dataExtTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Workflow.WorkflowDataExtType Type
    {
      get
      {
        return Namespace.FromStringToEnum<Enumerators.Workflow.WorkflowDataExtType>();
      }
      set
      {
        value = Namespace.FromStringToEnum<Enumerators.Workflow.WorkflowDataExtType>();
      }
    }

    private string _namespace;
    [Persistent, Size(500)]
    public string Namespace
    {
      get
      {
        return _namespace;
      }
      set
      {
        SetPropertyValue("Namespace", ref _namespace, value);
      }
    }

    private string _assembly;
    [Persistent, Size(150)]
    public string Assembly
    {
      get
      {
        return _assembly;
      }
      set
      {
        SetPropertyValue("Assembly", ref _assembly, value);
      }
    }

    #region Constructors

    public WFL_DataExtType() : base() { }
    public WFL_DataExtType(Session session) : base(session) { }

    #endregion
  }
}
