﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Atlas.Server.NuCard.TutukaNuCardService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="TutukaNuCardService.wsNucardSoap")]
    public interface wsNucardSoap {
        
        // CODEGEN: Generating message contract since element name username from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetProfileTransactionDataForPrevDay", ReplyAction="*")]
        Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataForPrevDayResponse GetProfileTransactionDataForPrevDay(Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataForPrevDayRequest request);
        
        // CODEGEN: Generating message contract since element name username from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetProfileTransactionData", ReplyAction="*")]
        Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataResponse GetProfileTransactionData(Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetProfileTransactionDataForPrevDayRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetProfileTransactionDataForPrevDay", Namespace="http://tempuri.org/", Order=0)]
        public Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataForPrevDayRequestBody Body;
        
        public GetProfileTransactionDataForPrevDayRequest() {
        }
        
        public GetProfileTransactionDataForPrevDayRequest(Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataForPrevDayRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class GetProfileTransactionDataForPrevDayRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string username;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string profile_number;
        
        public GetProfileTransactionDataForPrevDayRequestBody() {
        }
        
        public GetProfileTransactionDataForPrevDayRequestBody(string username, string password, string profile_number) {
            this.username = username;
            this.password = password;
            this.profile_number = profile_number;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetProfileTransactionDataForPrevDayResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetProfileTransactionDataForPrevDayResponse", Namespace="http://tempuri.org/", Order=0)]
        public Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataForPrevDayResponseBody Body;
        
        public GetProfileTransactionDataForPrevDayResponse() {
        }
        
        public GetProfileTransactionDataForPrevDayResponse(Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataForPrevDayResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class GetProfileTransactionDataForPrevDayResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public System.Xml.XmlElement GetProfileTransactionDataForPrevDayResult;
        
        public GetProfileTransactionDataForPrevDayResponseBody() {
        }
        
        public GetProfileTransactionDataForPrevDayResponseBody(System.Xml.XmlElement GetProfileTransactionDataForPrevDayResult) {
            this.GetProfileTransactionDataForPrevDayResult = GetProfileTransactionDataForPrevDayResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetProfileTransactionDataRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetProfileTransactionData", Namespace="http://tempuri.org/", Order=0)]
        public Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataRequestBody Body;
        
        public GetProfileTransactionDataRequest() {
        }
        
        public GetProfileTransactionDataRequest(Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class GetProfileTransactionDataRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string username;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string profile_number;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string datefrom;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string dateto;
        
        public GetProfileTransactionDataRequestBody() {
        }
        
        public GetProfileTransactionDataRequestBody(string username, string password, string profile_number, string datefrom, string dateto) {
            this.username = username;
            this.password = password;
            this.profile_number = profile_number;
            this.datefrom = datefrom;
            this.dateto = dateto;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetProfileTransactionDataResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetProfileTransactionDataResponse", Namespace="http://tempuri.org/", Order=0)]
        public Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataResponseBody Body;
        
        public GetProfileTransactionDataResponse() {
        }
        
        public GetProfileTransactionDataResponse(Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class GetProfileTransactionDataResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public System.Xml.XmlElement GetProfileTransactionDataResult;
        
        public GetProfileTransactionDataResponseBody() {
        }
        
        public GetProfileTransactionDataResponseBody(System.Xml.XmlElement GetProfileTransactionDataResult) {
            this.GetProfileTransactionDataResult = GetProfileTransactionDataResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface wsNucardSoapChannel : Atlas.Server.NuCard.TutukaNuCardService.wsNucardSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class wsNucardSoapClient : System.ServiceModel.ClientBase<Atlas.Server.NuCard.TutukaNuCardService.wsNucardSoap>, Atlas.Server.NuCard.TutukaNuCardService.wsNucardSoap {
        
        public wsNucardSoapClient() {
        }
        
        public wsNucardSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public wsNucardSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public wsNucardSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public wsNucardSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataForPrevDayResponse Atlas.Server.NuCard.TutukaNuCardService.wsNucardSoap.GetProfileTransactionDataForPrevDay(Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataForPrevDayRequest request) {
            return base.Channel.GetProfileTransactionDataForPrevDay(request);
        }
        
        public System.Xml.XmlElement GetProfileTransactionDataForPrevDay(string username, string password, string profile_number) {
            Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataForPrevDayRequest inValue = new Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataForPrevDayRequest();
            inValue.Body = new Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataForPrevDayRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            inValue.Body.profile_number = profile_number;
            Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataForPrevDayResponse retVal = ((Atlas.Server.NuCard.TutukaNuCardService.wsNucardSoap)(this)).GetProfileTransactionDataForPrevDay(inValue);
            return retVal.Body.GetProfileTransactionDataForPrevDayResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataResponse Atlas.Server.NuCard.TutukaNuCardService.wsNucardSoap.GetProfileTransactionData(Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataRequest request) {
            return base.Channel.GetProfileTransactionData(request);
        }
        
        public System.Xml.XmlElement GetProfileTransactionData(string username, string password, string profile_number, string datefrom, string dateto) {
            Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataRequest inValue = new Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataRequest();
            inValue.Body = new Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            inValue.Body.profile_number = profile_number;
            inValue.Body.datefrom = datefrom;
            inValue.Body.dateto = dateto;
            Atlas.Server.NuCard.TutukaNuCardService.GetProfileTransactionDataResponse retVal = ((Atlas.Server.NuCard.TutukaNuCardService.wsNucardSoap)(this)).GetProfileTransactionData(inValue);
            return retVal.Body.GetProfileTransactionDataResult;
        }
    }
}
