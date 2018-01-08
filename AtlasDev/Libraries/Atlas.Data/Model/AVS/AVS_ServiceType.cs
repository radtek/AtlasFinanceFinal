using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class AVS_ServiceType : XPLiteObject
  {
    private Int32 _serviceTypeId;
    [Key]
    public Int32 ServiceTypeId
    {
      get
      {
        return _serviceTypeId;
      }
      set
      {
        SetPropertyValue("ServiceTypeId", ref _serviceTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.AVS.ServiceType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.AVS.ServiceType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.AVS.ServiceType>();
      }
    }

    private string _description;
    [Persistent, Size(20)]
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

    public AVS_ServiceType() : base() { }
    public AVS_ServiceType(Session session) : base(session) { }

    #endregion
  }
}
 