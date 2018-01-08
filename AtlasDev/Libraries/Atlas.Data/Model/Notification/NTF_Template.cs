using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class NTF_Template : XPLiteObject
  {
    private int _templateId;
    [Key(AutoGenerate = true)]
    public int TemplateId
    {
      get
      {
        return _templateId;
      }
      set
      {
        SetPropertyValue("TemplateId", ref _templateId, value);
      }
    }

    private NTF_TemplateType _templateType;
    [Persistent("TemplateTypeId")]
    public NTF_TemplateType TemplateType
    {
      get
      {
        return _templateType;
      }
      set
      {
        SetPropertyValue("TemplateType", ref _templateType, value);
      }
    }

    private string _template;
    [Persistent, Size(Int32.MaxValue)]
    public string Template
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

    private int _version;
    [Persistent]
    public int Version
    {
      get
      {
        return _version;
      }
      set
      {
        SetPropertyValue("Version", ref _version, value);
      }
    }

    private DateTime _createDate;
    [Persistent]
    public DateTime CreateDate
    {
      get
      {
        return _createDate;
      }
      set
      {
        SetPropertyValue("CreateDate", ref _createDate, value);
      }
    }


    #region Constructors

    public NTF_Template() : base() { }
    public NTF_Template(Session session) : base(session) { }

    #endregion
  }
}