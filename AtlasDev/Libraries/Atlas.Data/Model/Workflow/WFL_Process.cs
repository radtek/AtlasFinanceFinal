using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_Process : XPLiteObject
  {
    private int _processId;
    [Key]
    public int ProcessId
    {
      get
      {
        return _processId;
      }
      set
      {
        SetPropertyValue("ProcessId", ref _processId, value);
      }
    }

    private WFL_Workflow _workflow;
    [Persistent("WorkflowId")]
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
  
    private string _name;
    [Persistent,Size(50)]    
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

    private string _assembly;
    [Persistent, Size(100)]
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

    private bool _locked;
    [Persistent]
    public bool Locked
    {
      get
      {
        return _locked;
      }
      set
      {
        SetPropertyValue("Locked", ref _locked, value);
      }
    }

    private bool _enabled;
    [Persistent]
    public bool Enabled
    {
      get
      {
        return _enabled;
      }
      set
      {
        SetPropertyValue("Enabled", ref _enabled, value);
      }
    }

    private DateTime? _disableDate;
    [Persistent]
    public DateTime? DisableDate
    {
      get
      {
        return _disableDate;
      }
      set
      {
        SetPropertyValue("DisableDate", ref _disableDate, value);
      }
    }

    private int _rank;
    [Persistent]
    public int Rank
    {
      get
      {

        return _rank;
      }
      set
      {
        SetPropertyValue("Rank", ref _rank, value);
      }
    }

    [Association]
    public XPCollection<WFL_ProcessStep> ProcessSteps { get { return GetCollection<WFL_ProcessStep>("ProcessSteps"); } }

    #region Constructors

    public WFL_Process() : base() { }
    public WFL_Process(Session session) : base(session) { }

    #endregion
  }
}
