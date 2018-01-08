﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Atlas.ThirdParty.EADONupayReportService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.nupay.co.za/", ConfigurationName="EADONupayReportService.wsNupayReportSoap")]
    public interface wsNupayReportSoap {
        
        // CODEGEN: Generating message contract since element name merchant_number from namespace http://www.nupay.co.za/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://www.nupay.co.za/getReport", ReplyAction="*")]
        Atlas.ThirdParty.EADONupayReportService.getReportResponse getReport(Atlas.ThirdParty.EADONupayReportService.getReportRequest request);
        
        // CODEGEN: Generating message contract since element name merchant_number from namespace http://www.nupay.co.za/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://www.nupay.co.za/getReportSPF", ReplyAction="*")]
        Atlas.ThirdParty.EADONupayReportService.getReportSPFResponse getReportSPF(Atlas.ThirdParty.EADONupayReportService.getReportSPFRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class getReportRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="getReport", Namespace="http://www.nupay.co.za/", Order=0)]
        public Atlas.ThirdParty.EADONupayReportService.getReportRequestBody Body;
        
        public getReportRequest() {
        }
        
        public getReportRequest(Atlas.ThirdParty.EADONupayReportService.getReportRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.nupay.co.za/")]
    public partial class getReportRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string merchant_number;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string usertype;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string report_type;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=4)]
        public System.DateTime date_from;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=5)]
        public System.DateTime date_to;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string token_id;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=7)]
        public string block_id;
        
        public getReportRequestBody() {
        }
        
        public getReportRequestBody(string merchant_number, string password, string usertype, string report_type, System.DateTime date_from, System.DateTime date_to, string token_id, string block_id) {
            this.merchant_number = merchant_number;
            this.password = password;
            this.usertype = usertype;
            this.report_type = report_type;
            this.date_from = date_from;
            this.date_to = date_to;
            this.token_id = token_id;
            this.block_id = block_id;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class getReportResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="getReportResponse", Namespace="http://www.nupay.co.za/", Order=0)]
        public Atlas.ThirdParty.EADONupayReportService.getReportResponseBody Body;
        
        public getReportResponse() {
        }
        
        public getReportResponse(Atlas.ThirdParty.EADONupayReportService.getReportResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.nupay.co.za/")]
    public partial class getReportResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public System.Xml.Linq.XElement getReportResult;
        
        public getReportResponseBody() {
        }
        
        public getReportResponseBody(System.Xml.Linq.XElement getReportResult) {
            this.getReportResult = getReportResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class getReportSPFRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="getReportSPF", Namespace="http://www.nupay.co.za/", Order=0)]
        public Atlas.ThirdParty.EADONupayReportService.getReportSPFRequestBody Body;
        
        public getReportSPFRequest() {
        }
        
        public getReportSPFRequest(Atlas.ThirdParty.EADONupayReportService.getReportSPFRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.nupay.co.za/")]
    public partial class getReportSPFRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string merchant_number;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string usertype;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string report_type;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=4)]
        public System.DateTime date_from;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=5)]
        public System.DateTime date_to;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string token_id;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=7)]
        public string block_id;
        
        public getReportSPFRequestBody() {
        }
        
        public getReportSPFRequestBody(string merchant_number, string password, string usertype, string report_type, System.DateTime date_from, System.DateTime date_to, string token_id, string block_id) {
            this.merchant_number = merchant_number;
            this.password = password;
            this.usertype = usertype;
            this.report_type = report_type;
            this.date_from = date_from;
            this.date_to = date_to;
            this.token_id = token_id;
            this.block_id = block_id;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class getReportSPFResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="getReportSPFResponse", Namespace="http://www.nupay.co.za/", Order=0)]
        public Atlas.ThirdParty.EADONupayReportService.getReportSPFResponseBody Body;
        
        public getReportSPFResponse() {
        }
        
        public getReportSPFResponse(Atlas.ThirdParty.EADONupayReportService.getReportSPFResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.nupay.co.za/")]
    public partial class getReportSPFResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public System.Xml.Linq.XElement getReportSPFResult;
        
        public getReportSPFResponseBody() {
        }
        
        public getReportSPFResponseBody(System.Xml.Linq.XElement getReportSPFResult) {
            this.getReportSPFResult = getReportSPFResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface wsNupayReportSoapChannel : Atlas.ThirdParty.EADONupayReportService.wsNupayReportSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class wsNupayReportSoapClient : System.ServiceModel.ClientBase<Atlas.ThirdParty.EADONupayReportService.wsNupayReportSoap>, Atlas.ThirdParty.EADONupayReportService.wsNupayReportSoap {
        
        public wsNupayReportSoapClient() {
        }
        
        public wsNupayReportSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public wsNupayReportSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public wsNupayReportSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public wsNupayReportSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Atlas.ThirdParty.EADONupayReportService.getReportResponse Atlas.ThirdParty.EADONupayReportService.wsNupayReportSoap.getReport(Atlas.ThirdParty.EADONupayReportService.getReportRequest request) {
            return base.Channel.getReport(request);
        }
        
        public System.Xml.Linq.XElement getReport(string merchant_number, string password, string usertype, string report_type, System.DateTime date_from, System.DateTime date_to, string token_id, string block_id) {
            Atlas.ThirdParty.EADONupayReportService.getReportRequest inValue = new Atlas.ThirdParty.EADONupayReportService.getReportRequest();
            inValue.Body = new Atlas.ThirdParty.EADONupayReportService.getReportRequestBody();
            inValue.Body.merchant_number = merchant_number;
            inValue.Body.password = password;
            inValue.Body.usertype = usertype;
            inValue.Body.report_type = report_type;
            inValue.Body.date_from = date_from;
            inValue.Body.date_to = date_to;
            inValue.Body.token_id = token_id;
            inValue.Body.block_id = block_id;
            Atlas.ThirdParty.EADONupayReportService.getReportResponse retVal = ((Atlas.ThirdParty.EADONupayReportService.wsNupayReportSoap)(this)).getReport(inValue);
            return retVal.Body.getReportResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Atlas.ThirdParty.EADONupayReportService.getReportSPFResponse Atlas.ThirdParty.EADONupayReportService.wsNupayReportSoap.getReportSPF(Atlas.ThirdParty.EADONupayReportService.getReportSPFRequest request) {
            return base.Channel.getReportSPF(request);
        }
        
        public System.Xml.Linq.XElement getReportSPF(string merchant_number, string password, string usertype, string report_type, System.DateTime date_from, System.DateTime date_to, string token_id, string block_id) {
            Atlas.ThirdParty.EADONupayReportService.getReportSPFRequest inValue = new Atlas.ThirdParty.EADONupayReportService.getReportSPFRequest();
            inValue.Body = new Atlas.ThirdParty.EADONupayReportService.getReportSPFRequestBody();
            inValue.Body.merchant_number = merchant_number;
            inValue.Body.password = password;
            inValue.Body.usertype = usertype;
            inValue.Body.report_type = report_type;
            inValue.Body.date_from = date_from;
            inValue.Body.date_to = date_to;
            inValue.Body.token_id = token_id;
            inValue.Body.block_id = block_id;
            Atlas.ThirdParty.EADONupayReportService.getReportSPFResponse retVal = ((Atlas.ThirdParty.EADONupayReportService.wsNupayReportSoap)(this)).getReportSPF(inValue);
            return retVal.Body.getReportSPFResult;
        }
    }
}
