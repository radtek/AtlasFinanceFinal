using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.ThirdParty.Xds
{

  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
  public partial class Profile
  {

    private object[] itemsField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Branches", typeof(ProfileBranches), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlElementAttribute("Purposes", typeof(ProfilePurposes), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlElementAttribute("SubscriberProfile", typeof(ProfileSubscriberProfile), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public object[] Items
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
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class ProfileBranches
  {

    private string subscriberIDField;

    private string branchCodeField;

    private string branchNameField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string SubscriberID
    {
      get
      {
        return this.subscriberIDField;
      }
      set
      {
        this.subscriberIDField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string BranchCode
    {
      get
      {
        return this.branchCodeField;
      }
      set
      {
        this.branchCodeField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string BranchName
    {
      get
      {
        return this.branchNameField;
      }
      set
      {
        this.branchNameField = value;
      }
    }
  }

  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class ProfilePurposes
  {

    private string subscriberIDField;

    private string purposeIDField;

    private string purposeField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string SubscriberID
    {
      get
      {
        return this.subscriberIDField;
      }
      set
      {
        this.subscriberIDField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string PurposeID
    {
      get
      {
        return this.purposeIDField;
      }
      set
      {
        this.purposeIDField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Purpose
    {
      get
      {
        return this.purposeField;
      }
      set
      {
        this.purposeField = value;
      }
    }
  }

  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class ProfileSubscriberProfile
  {

    private string subscriberIDField;

    private string authenticationSearchOnIDNoYNField;

    private string authenticationSearchOnCellPhoneNoYNField;

    private string authenticationSearchOnAccountNoYNField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string SubscriberID
    {
      get
      {
        return this.subscriberIDField;
      }
      set
      {
        this.subscriberIDField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string AuthenticationSearchOnIDNoYN
    {
      get
      {
        return this.authenticationSearchOnIDNoYNField;
      }
      set
      {
        this.authenticationSearchOnIDNoYNField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string AuthenticationSearchOnCellPhoneNoYN
    {
      get
      {
        return this.authenticationSearchOnCellPhoneNoYNField;
      }
      set
      {
        this.authenticationSearchOnCellPhoneNoYNField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string AuthenticationSearchOnAccountNoYN
    {
      get
      {
        return this.authenticationSearchOnAccountNoYNField;
      }
      set
      {
        this.authenticationSearchOnAccountNoYNField = value;
      }
    }
  }
}