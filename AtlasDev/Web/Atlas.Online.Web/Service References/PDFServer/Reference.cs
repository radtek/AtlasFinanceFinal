﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Atlas.Online.Web.PDFServer {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PdfResult", Namespace="http://schemas.datacontract.org/2004/07/Atlas.PDF.Server.WCF.Interface")]
    [System.SerializableAttribute()]
    public partial class PdfResult : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte[] BytesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] Bytes {
            get {
                return this.BytesField;
            }
            set {
                if ((object.ReferenceEquals(this.BytesField, value) != true)) {
                    this.BytesField = value;
                    this.RaisePropertyChanged("Bytes");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Error {
            get {
                return this.ErrorField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorField, value) != true)) {
                    this.ErrorField = value;
                    this.RaisePropertyChanged("Error");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PDFServer.IPDFServer")]
    public interface IPDFServer {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPDFServer/GetPdf", ReplyAction="http://tempuri.org/IPDFServer/GetPdfResponse")]
        Atlas.Online.Web.PDFServer.PdfResult GetPdf(string content);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPDFServer/GetPdf", ReplyAction="http://tempuri.org/IPDFServer/GetPdfResponse")]
        System.Threading.Tasks.Task<Atlas.Online.Web.PDFServer.PdfResult> GetPdfAsync(string content);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPDFServerChannel : Atlas.Online.Web.PDFServer.IPDFServer, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PDFServerClient : System.ServiceModel.ClientBase<Atlas.Online.Web.PDFServer.IPDFServer>, Atlas.Online.Web.PDFServer.IPDFServer {
        
        public PDFServerClient() {
        }
        
        public PDFServerClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PDFServerClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PDFServerClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PDFServerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Atlas.Online.Web.PDFServer.PdfResult GetPdf(string content) {
            return base.Channel.GetPdf(content);
        }
        
        public System.Threading.Tasks.Task<Atlas.Online.Web.PDFServer.PdfResult> GetPdfAsync(string content) {
            return base.Channel.GetPdfAsync(content);
        }
    }
}
