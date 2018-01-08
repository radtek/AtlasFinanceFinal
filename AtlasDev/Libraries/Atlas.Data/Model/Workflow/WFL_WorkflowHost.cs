using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_WorkflowHost : XPLiteObject
  {
    private int _workflowHostId;
    [Key(AutoGenerate = true)]
    public int WorkflowHostId
    {
      get
      {
        return _workflowHostId;
      }
      set
      {
        SetPropertyValue("WorkflowHostId", ref _workflowHostId, value);
      }
    }

    private WFL_Workflow _workflow;
    [Persistent("WorkflowId")]
    [Association]
    public WFL_Workflow Workflow
    {
      get
      {
        return _workflow;
      }
      set
      {
        SetPropertyValue("Workflow", ref _workflow, value);
      }
    }

    private Host _host;
    [Persistent("HostId")]
    public Host Host
    {
      get
      {
        return _host;
      }
      set
      {
        SetPropertyValue("Host", ref _host, value);
      }
    }

    #region Constructors

    public WFL_WorkflowHost() : base() { }
    public WFL_WorkflowHost(Session session) : base(session) { }

    #endregion
  }
}
