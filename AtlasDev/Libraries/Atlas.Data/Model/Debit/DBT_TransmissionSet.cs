using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class DBT_TransmissionSet : XPLiteObject
  {
    private Int64 _transmissionSetId;
    [Key(AutoGenerate = true)]
    public Int64 TransmissionSetId
    {
      get
      {
        return _transmissionSetId;
      }
      set
      {
        SetPropertyValue("TransmissionSetId", ref _transmissionSetId, value);
      }
    }

    private DBT_Transmission _transmission;
    [Persistent("TransmissionId")]
    public DBT_Transmission Transmission
    {
      get
      {
        return _transmission;
      }
      set
      {
        SetPropertyValue("Transmission", ref _transmission, value);
      }
    }

    private int _generationNo;
    [Persistent]
    public int GenerationNo
    {
      get
      {
        return _generationNo;
      }
      set
      {
        SetPropertyValue("GenerationNo", ref _generationNo, value);
      }
    }

    private bool? _accepted;
    [Persistent]
    public bool? Accepted
    {
      get
      {
        return _accepted;
      }
      set
      {
        SetPropertyValue("Accepted", ref _accepted, value);
      }
    }

    private DateTime? _createDate;
    [Persistent]
    public DateTime? CreateDate
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


    public DBT_TransmissionSet() : base() { }
    public DBT_TransmissionSet(Session session) : base(session) { }
  }
}