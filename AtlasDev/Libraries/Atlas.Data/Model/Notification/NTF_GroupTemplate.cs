using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.Model
{
  public class NTF_GroupTemplate : XPLiteObject
  {
    private Int64 _groupTemplateId;
    [Key(AutoGenerate = true)]
    public Int64 GroupTemplateId
    {
      get
      {
        return _groupTemplateId;
      }
      set
      {
        SetPropertyValue("GroupTemplateId", ref _groupTemplateId, value);
      }
    }

    private NTF_Group _group;
    [Persistent("GroupId")]
    public NTF_Group Group
    {
      get
      {
        return _group;
      }
      set
      {
        SetPropertyValue("Group", ref _group, value);
      }
    }

    private NTF_Template _template;
    [Persistent("TemplateId")]
    public NTF_Template Template
    {
      get
      {
        return _template;
      }
      set
      {
        SetPropertyValue("Template", ref _template, value);
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

    #region Constructors

    public NTF_GroupTemplate() : base() { }
    public NTF_GroupTemplate(Session session) : base(session) { }

    #endregion
  }
}