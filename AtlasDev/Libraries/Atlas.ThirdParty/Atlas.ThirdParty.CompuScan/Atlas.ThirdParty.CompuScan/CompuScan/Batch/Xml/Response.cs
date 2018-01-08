using System;
using System.Xml.Serialization;
using System.Xml.Schema;


namespace Atlas.ThirdParty.CompuScan.Batch.XML.Response
{
  /// <remarks/>
  [Serializable]
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false)]
  public partial class TRANS_DATA
  {
    private TRANS_DATARECORD[] itemsField;

    /// <remarks/>
    [XmlElement("RECORD", Form = XmlSchemaForm.Unqualified)]
    public TRANS_DATARECORD[] Items
    {
      get
      {
        return this.itemsField;
      }
      set
      {
        this.itemsField = value;
      }
    }
  }

  /// <remarks/>
  [Serializable]
  [XmlType(AnonymousType = true)]
  public partial class TRANS_DATARECORD
  {
    private string jOB_IDField;

    private string tRANS_TYPEField;

    private string tRANS_SUB_TYPEField;

    private string rEFERENCE_NOField;

    private string tRANS_STATUSField;

    private string eRROR_CODEField;

    private string mESSAGEField;

    private string mISC_DATAField;

    private string fILENAMEField;

    private string nLR_ENQ_REF_NOField;

    private string nLR_ENQ_START_DATEField;

    private string sUMM_FILENAMEField;

    private string cC_FILENAMEField;

    private string nLR_FILENAMEField;

    private string nUMField;

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string JOB_ID
    {
      get
      {
        return this.jOB_IDField;
      }
      set
      {
        this.jOB_IDField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string TRANS_TYPE
    {
      get
      {
        return this.tRANS_TYPEField;
      }
      set
      {
        this.tRANS_TYPEField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string TRANS_SUB_TYPE
    {
      get
      {
        return this.tRANS_SUB_TYPEField;
      }
      set
      {
        this.tRANS_SUB_TYPEField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string REFERENCE_NO
    {
      get
      {
        return this.rEFERENCE_NOField;
      }
      set
      {
        this.rEFERENCE_NOField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string TRANS_STATUS
    {
      get
      {
        return this.tRANS_STATUSField;
      }
      set
      {
        this.tRANS_STATUSField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string ERROR_CODE
    {
      get
      {
        return this.eRROR_CODEField;
      }
      set
      {
        this.eRROR_CODEField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string MESSAGE
    {
      get
      {
        return this.mESSAGEField;
      }
      set
      {
        this.mESSAGEField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string MISC_DATA
    {
      get
      {
        return this.mISC_DATAField;
      }
      set
      {
        this.mISC_DATAField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string FILENAME
    {
      get
      {
        return this.fILENAMEField;
      }
      set
      {
        this.fILENAMEField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string NLR_ENQ_REF_NO
    {
      get
      {
        return this.nLR_ENQ_REF_NOField;
      }
      set
      {
        this.nLR_ENQ_REF_NOField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string NLR_ENQ_START_DATE
    {
      get
      {
        return this.nLR_ENQ_START_DATEField;
      }
      set
      {
        this.nLR_ENQ_START_DATEField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string SUMM_FILENAME
    {
      get
      {
        return this.sUMM_FILENAMEField;
      }
      set
      {
        this.sUMM_FILENAMEField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string CC_FILENAME
    {
      get
      {
        return this.cC_FILENAMEField;
      }
      set
      {
        this.cC_FILENAMEField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string NLR_FILENAME
    {
      get
      {
        return this.nLR_FILENAMEField;
      }
      set
      {
        this.nLR_FILENAMEField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string NUM
    {
      get
      {
        return this.nUMField;
      }
      set
      {
        this.nUMField = value;
      }
    }
  }
}