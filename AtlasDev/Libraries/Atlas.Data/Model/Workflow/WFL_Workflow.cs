using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_Workflow : XPLiteObject
  {
    private int _workflowId;
    [Key]
    public int WorkflowId
    {
      get
      {
        return _workflowId;
      }
      set
      {
        SetPropertyValue("WorkflowId", ref _workflowId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Workflow.WorkflowProcess Type
    {
      get
      {
        return Name.FromStringToEnum<Enumerators.Workflow.WorkflowProcess>();
      }
      set
      {
        value = Name.FromStringToEnum<Enumerators.Workflow.WorkflowProcess>();
      }
    }

    private string _name;
    [Persistent, Size(50)]
    public string Name
    {
      get
      {
        return _name;
      }
      set
      {
        SetPropertyValue("Name", ref _name, value);
      }
    }

    [Association]
    public XPCollection<WFL_WorkflowHost> WorkflowHosts { get { return GetCollection<WFL_WorkflowHost>("WorkflowHosts"); } }

    #region Constructors

    public WFL_Workflow() : base() { }
    public WFL_Workflow(Session session) : base(session) { }

    #endregion
  }
}
