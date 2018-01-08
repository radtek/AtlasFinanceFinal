﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Atlas.ThirdParty.CS.Bureau
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://webServices/", ConfigurationName="Atlas.ThirdParty.CS.Bureau.NormalSearchService")]
    public interface NormalSearchService
    {
        
        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        Atlas.ThirdParty.CS.Bureau.PingServerResponse PingServer(Atlas.ThirdParty.CS.Bureau.PingServerRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        System.Threading.Tasks.Task<Atlas.ThirdParty.CS.Bureau.PingServerResponse> PingServerAsync(Atlas.ThirdParty.CS.Bureau.PingServerRequest request);
        
        // CODEGEN: Parameter 'TransReplyClass' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        [return: System.ServiceModel.MessageParameterAttribute(Name="TransReplyClass")]
        Atlas.ThirdParty.CS.Bureau.DoNormalEnquiryResponse DoNormalEnquiry(Atlas.ThirdParty.CS.Bureau.DoNormalEnquiryRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        System.Threading.Tasks.Task<Atlas.ThirdParty.CS.Bureau.DoNormalEnquiryResponse> DoNormalEnquiryAsync(Atlas.ThirdParty.CS.Bureau.DoNormalEnquiryRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="PingServer", WrapperNamespace="http://webServices/", IsWrapped=true)]
    public partial class PingServerRequest
    {
        
        public PingServerRequest()
        {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="PingServerResponse", WrapperNamespace="http://webServices/", IsWrapped=true)]
    public partial class PingServerResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webServices/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool @return;
        
        public PingServerResponse()
        {
        }
        
        public PingServerResponse(bool @return)
        {
            this.@return = @return;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webServices/")]
    public partial class NormalEnqRequestParamsType
    {
        
        private string pUsrnmeField;
        
        private string pPasswrdField;
        
        private string pVersionField;
        
        private string pOriginField;
        
        private string pOrigin_VersionField;
        
        private string pInput_FormatField;
        
        private string pTransactionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string pUsrnme
        {
            get
            {
                return this.pUsrnmeField;
            }
            set
            {
                this.pUsrnmeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string pPasswrd
        {
            get
            {
                return this.pPasswrdField;
            }
            set
            {
                this.pPasswrdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string pVersion
        {
            get
            {
                return this.pVersionField;
            }
            set
            {
                this.pVersionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string pOrigin
        {
            get
            {
                return this.pOriginField;
            }
            set
            {
                this.pOriginField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string pOrigin_Version
        {
            get
            {
                return this.pOrigin_VersionField;
            }
            set
            {
                this.pOrigin_VersionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public string pInput_Format
        {
            get
            {
                return this.pInput_FormatField;
            }
            set
            {
                this.pInput_FormatField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public string pTransaction
        {
            get
            {
                return this.pTransactionField;
            }
            set
            {
                this.pTransactionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webServices/")]
    public partial class transReplyClass
    {
        
        private bool transactionCompletedField;
        
        private string errorCodeField;
        
        private string errorStringField;
        
        private string retDataField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public bool transactionCompleted
        {
            get
            {
                return this.transactionCompletedField;
            }
            set
            {
                this.transactionCompletedField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string errorCode
        {
            get
            {
                return this.errorCodeField;
            }
            set
            {
                this.errorCodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string errorString
        {
            get
            {
                return this.errorStringField;
            }
            set
            {
                this.errorStringField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string retData
        {
            get
            {
                return this.retDataField;
            }
            set
            {
                this.retDataField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DoNormalEnquiry", WrapperNamespace="http://webServices/", IsWrapped=true)]
    public partial class DoNormalEnquiryRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webServices/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Atlas.ThirdParty.CS.Bureau.NormalEnqRequestParamsType request;
        
        public DoNormalEnquiryRequest()
        {
        }
        
        public DoNormalEnquiryRequest(Atlas.ThirdParty.CS.Bureau.NormalEnqRequestParamsType request)
        {
            this.request = request;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DoNormalEnquiryResponse", WrapperNamespace="http://webServices/", IsWrapped=true)]
    public partial class DoNormalEnquiryResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webServices/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Atlas.ThirdParty.CS.Bureau.transReplyClass TransReplyClass;
        
        public DoNormalEnquiryResponse()
        {
        }
        
        public DoNormalEnquiryResponse(Atlas.ThirdParty.CS.Bureau.transReplyClass TransReplyClass)
        {
            this.TransReplyClass = TransReplyClass;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface NormalSearchServiceChannel : Atlas.ThirdParty.CS.Bureau.NormalSearchService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class NormalSearchServiceClient : System.ServiceModel.ClientBase<Atlas.ThirdParty.CS.Bureau.NormalSearchService>, Atlas.ThirdParty.CS.Bureau.NormalSearchService
    {
        
        public NormalSearchServiceClient()
        {
        }
        
        public NormalSearchServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public NormalSearchServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public NormalSearchServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public NormalSearchServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Atlas.ThirdParty.CS.Bureau.PingServerResponse Atlas.ThirdParty.CS.Bureau.NormalSearchService.PingServer(Atlas.ThirdParty.CS.Bureau.PingServerRequest request)
        {
            return base.Channel.PingServer(request);
        }
        
        public bool PingServer()
        {
            Atlas.ThirdParty.CS.Bureau.PingServerRequest inValue = new Atlas.ThirdParty.CS.Bureau.PingServerRequest();
            Atlas.ThirdParty.CS.Bureau.PingServerResponse retVal = ((Atlas.ThirdParty.CS.Bureau.NormalSearchService)(this)).PingServer(inValue);
            return retVal.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Atlas.ThirdParty.CS.Bureau.PingServerResponse> Atlas.ThirdParty.CS.Bureau.NormalSearchService.PingServerAsync(Atlas.ThirdParty.CS.Bureau.PingServerRequest request)
        {
            return base.Channel.PingServerAsync(request);
        }
        
        public System.Threading.Tasks.Task<Atlas.ThirdParty.CS.Bureau.PingServerResponse> PingServerAsync()
        {
            Atlas.ThirdParty.CS.Bureau.PingServerRequest inValue = new Atlas.ThirdParty.CS.Bureau.PingServerRequest();
            return ((Atlas.ThirdParty.CS.Bureau.NormalSearchService)(this)).PingServerAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Atlas.ThirdParty.CS.Bureau.DoNormalEnquiryResponse Atlas.ThirdParty.CS.Bureau.NormalSearchService.DoNormalEnquiry(Atlas.ThirdParty.CS.Bureau.DoNormalEnquiryRequest request)
        {
            return base.Channel.DoNormalEnquiry(request);
        }
        
        public Atlas.ThirdParty.CS.Bureau.transReplyClass DoNormalEnquiry(Atlas.ThirdParty.CS.Bureau.NormalEnqRequestParamsType request)
        {
            Atlas.ThirdParty.CS.Bureau.DoNormalEnquiryRequest inValue = new Atlas.ThirdParty.CS.Bureau.DoNormalEnquiryRequest();
            inValue.request = request;
            Atlas.ThirdParty.CS.Bureau.DoNormalEnquiryResponse retVal = ((Atlas.ThirdParty.CS.Bureau.NormalSearchService)(this)).DoNormalEnquiry(inValue);
            return retVal.TransReplyClass;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Atlas.ThirdParty.CS.Bureau.DoNormalEnquiryResponse> Atlas.ThirdParty.CS.Bureau.NormalSearchService.DoNormalEnquiryAsync(Atlas.ThirdParty.CS.Bureau.DoNormalEnquiryRequest request)
        {
            return base.Channel.DoNormalEnquiryAsync(request);
        }
        
        public System.Threading.Tasks.Task<Atlas.ThirdParty.CS.Bureau.DoNormalEnquiryResponse> DoNormalEnquiryAsync(Atlas.ThirdParty.CS.Bureau.NormalEnqRequestParamsType request)
        {
            Atlas.ThirdParty.CS.Bureau.DoNormalEnquiryRequest inValue = new Atlas.ThirdParty.CS.Bureau.DoNormalEnquiryRequest();
            inValue.request = request;
            return ((Atlas.ThirdParty.CS.Bureau.NormalSearchService)(this)).DoNormalEnquiryAsync(inValue);
        }
    }
}
