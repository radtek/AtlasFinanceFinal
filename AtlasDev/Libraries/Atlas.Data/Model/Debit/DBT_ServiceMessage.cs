using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class DBT_ServiceMessage : XPLiteObject
  {
    private int _serviceMessageId;
    [Key(AutoGenerate = false)]
    public int ServiceMessageId
    {
      get
      {
        return _serviceMessageId;
      }
      set
      {
        SetPropertyValue("ServiceMessageId", ref _serviceMessageId, value);
      }
    }

    private string _code;
    [Persistent, Size(20)]
    public string Code
    {
      get
      {
        return _code;
      }
      set
      {
        SetPropertyValue("Code", ref _code, value);
      }
    }

    private string _description;
    [Persistent, Size(120)]
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

    public DBT_ServiceMessage() : base() { }
    public DBT_ServiceMessage(Session session) : base(session) { }
  }
}