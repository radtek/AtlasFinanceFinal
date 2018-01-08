
using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class WFL_ConditionClass : XPLiteObject
  {
    private long _conditionClassId;
    [Key(AutoGenerate = false)]
    public long ConditionClassId
    {
      get
      {
        return _conditionClassId;
      }
      set
      {
        SetPropertyValue("ConditionClassId", ref _conditionClassId, value);
      }
    }       
    
    private string _description;
    [Persistent]    
    public string Description
    {
        get
        {
            return _description;
        }
        set
        {
            SetPropertyValue("Description", ref _description, value);
        }
    }

    [NonPersistent]
    public Enumerators.Workflow.ConditionClass Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Workflow.ConditionClass>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Workflow.ConditionClass>();
      }
    } 

    private string _assembly;
    [Persistent]    
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

    private string _namespace;
    [Persistent]
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


    #region Constructors

    public WFL_ConditionClass() : base() { }
    public WFL_ConditionClass(Session session) : base(session) { }

    #endregion
  }
}
