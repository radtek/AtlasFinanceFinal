using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.ThirdParty.Xds
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
  public partial class ListOfConsumers
  {

    private ListOfConsumersConsumerDetails consumerDetailsField;

    /// <remarks/>
    public ListOfConsumersConsumerDetails ConsumerDetails
    {
      get
      {
        return this.consumerDetailsField;
      }
      set
      {
        this.consumerDetailsField = value;
      }
    }
  }

  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class ListOfConsumersConsumerDetails
  {

    private uint consumerIDField;

    private bool consumerIDFieldSpecified;

    private string firstNameField;

    private string secondNameField;

    private string surnameField;

    private ulong iDNoField;

    private bool iDNoFieldSpecified;

    private System.DateTime birthDateField;

    private bool birthDateFieldSpecified;

    private string genderIndField;

    private string recordStatusIndField;

    private string titleCodeField;

    private string createdByUserField;

    private System.DateTime createdOnDateField;

    private bool createdOnDateFieldSpecified;

    private string firstInitialField;

    private string secondInitialField;

    private System.DateTime lastUpdatedDateField;

    private bool lastUpdatedDateFieldSpecified;

    private uint mergedConsumerIDField;

    private bool mergedConsumerIDFieldSpecified;

    private ulong iDNo10Field;

    private bool iDNo10FieldSpecified;

    private object bonusXMLField;

    private object tempReferenceField;

    private uint enquiryIDField;

    private bool enquiryIDFieldSpecified;

    private uint enquiryResultIDField;

    private bool enquiryResultIDFieldSpecified;

    private string referenceField;

    /// <remarks/>
    public uint ConsumerID
    {
      get
      {
        return this.consumerIDField;
      }
      set
      {
        this.consumerIDField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool ConsumerIDSpecified
    {
      get
      {
        return this.consumerIDFieldSpecified;
      }
      set
      {
        this.consumerIDFieldSpecified = value;
      }
    }

    /// <remarks/>
    public string FirstName
    {
      get
      {
        return this.firstNameField;
      }
      set
      {
        this.firstNameField = value;
      }
    }

    /// <remarks/>
    public string SecondName
    {
      get
      {
        return this.secondNameField;
      }
      set
      {
        this.secondNameField = value;
      }
    }

    /// <remarks/>
    public string Surname
    {
      get
      {
        return this.surnameField;
      }
      set
      {
        this.surnameField = value;
      }
    }

    /// <remarks/>
    public ulong IDNo
    {
      get
      {
        return this.iDNoField;
      }
      set
      {
        this.iDNoField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool IDNoSpecified
    {
      get
      {
        return this.iDNoFieldSpecified;
      }
      set
      {
        this.iDNoFieldSpecified = value;
      }
    }

    /// <remarks/>
    public System.DateTime BirthDate
    {
      get
      {
        return this.birthDateField;
      }
      set
      {
        this.birthDateField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool BirthDateSpecified
    {
      get
      {
        return this.birthDateFieldSpecified;
      }
      set
      {
        this.birthDateFieldSpecified = value;
      }
    }

    /// <remarks/>
    public string GenderInd
    {
      get
      {
        return this.genderIndField;
      }
      set
      {
        this.genderIndField = value;
      }
    }

    /// <remarks/>
    public string RecordStatusInd
    {
      get
      {
        return this.recordStatusIndField;
      }
      set
      {
        this.recordStatusIndField = value;
      }
    }

    /// <remarks/>
    public string TitleCode
    {
      get
      {
        return this.titleCodeField;
      }
      set
      {
        this.titleCodeField = value;
      }
    }

    /// <remarks/>
    public string CreatedByUser
    {
      get
      {
        return this.createdByUserField;
      }
      set
      {
        this.createdByUserField = value;
      }
    }

    /// <remarks/>
    public System.DateTime CreatedOnDate
    {
      get
      {
        return this.createdOnDateField;
      }
      set
      {
        this.createdOnDateField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool CreatedOnDateSpecified
    {
      get
      {
        return this.createdOnDateFieldSpecified;
      }
      set
      {
        this.createdOnDateFieldSpecified = value;
      }
    }

    /// <remarks/>
    public string FirstInitial
    {
      get
      {
        return this.firstInitialField;
      }
      set
      {
        this.firstInitialField = value;
      }
    }

    /// <remarks/>
    public string SecondInitial
    {
      get
      {
        return this.secondInitialField;
      }
      set
      {
        this.secondInitialField = value;
      }
    }

    /// <remarks/>
    public System.DateTime LastUpdatedDate
    {
      get
      {
        return this.lastUpdatedDateField;
      }
      set
      {
        this.lastUpdatedDateField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool LastUpdatedDateSpecified
    {
      get
      { 
        return this.lastUpdatedDateFieldSpecified;
      }
      set
      {
        this.lastUpdatedDateFieldSpecified = value;
      }
    }

    /// <remarks/>
    public uint MergedConsumerID
    {
      get
      {
        return this.mergedConsumerIDField;
      }
      set
      {
        this.mergedConsumerIDField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool MergedConsumerIDSpecified
    {
      get
      {
        return this.mergedConsumerIDFieldSpecified;
      }
      set
      {
        this.mergedConsumerIDFieldSpecified = value;
      }
    }

    /// <remarks/>
    public ulong IDNo10
    {
      get
      {
        return this.iDNo10Field;
      }
      set
      {
        this.iDNo10Field = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool IDNo10Specified
    {
      get
      {
        return this.iDNo10FieldSpecified;
      }
      set
      {
        this.iDNo10FieldSpecified = value;
      }
    }

    /// <remarks/>
    public object BonusXML
    {
      get
      {
        return this.bonusXMLField;
      }
      set
      {
        this.bonusXMLField = value;
      }
    }

    /// <remarks/>
    public object TempReference
    {
      get
      {
        return this.tempReferenceField;
      }
      set
      {
        this.tempReferenceField = value;
      }
    }

    /// <remarks/>
    public uint EnquiryID
    {
      get
      {
        return this.enquiryIDField;
      }
      set
      {
        this.enquiryIDField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool EnquiryIDSpecified
    {
      get
      {
        return this.enquiryIDFieldSpecified;
      }
      set
      {
        this.enquiryIDFieldSpecified = value;
      }
    }

    /// <remarks/>
    public uint EnquiryResultID
    {
      get
      {
        return this.enquiryResultIDField;
      }
      set
      {
        this.enquiryResultIDField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool EnquiryResultIDSpecified
    {
      get
      {
        return this.enquiryResultIDFieldSpecified;
      }
      set
      {
        this.enquiryResultIDFieldSpecified = value;
      }
    }

    /// <remarks/>
    public string Reference
    {
      get
      {
        return this.referenceField;
      }
      set
      {
        this.referenceField = value;
      }
    }
  }
}