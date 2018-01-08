using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class NTF_TemplateType : XPLiteObject
  {

    private int _templateTypeId;
    [Key(AutoGenerate = false)]
    public int TemplateTypeId
    {
      get
      {
        return _templateTypeId;
      }
      set
      {
        SetPropertyValue("TemplateTypeId", ref _templateTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Notification.NotificationTemplate Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Notification.NotificationTemplate>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Notification.NotificationTemplate>();
      }
    }

    private string _description;
    [Persistent, Size(40)]
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


    #region Constructors

    public NTF_TemplateType() : base() { }
    public NTF_TemplateType(Session session) : base(session) { }

    #endregion
  }
}
