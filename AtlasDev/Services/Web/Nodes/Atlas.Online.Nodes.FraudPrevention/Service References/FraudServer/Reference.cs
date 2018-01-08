﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Atlas.Online.Node.FraudPreventionNode.FraudServer {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FraudResult", Namespace="http://schemas.datacontract.org/2004/07/Atlas.ThirdParty.Fraud.TransUnion")]
    [System.SerializableAttribute()]
    public partial class FraudResult : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Atlas.Online.Node.FraudPreventionNode.FraudServer.EnquiryStatus EnquiryStatusField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long FraudScoreIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int RatingField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<string> ReasonCodesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Atlas.Enumerators.Account.AccountStatus StatusField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Atlas.Enumerators.Account.AccountStatusReason StatusReasonField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Atlas.Enumerators.Account.AccountStatusSubReason SubStatusReasonField;
        
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
        public Atlas.Online.Node.FraudPreventionNode.FraudServer.EnquiryStatus EnquiryStatus {
            get {
                return this.EnquiryStatusField;
            }
            set {
                if ((this.EnquiryStatusField.Equals(value) != true)) {
                    this.EnquiryStatusField = value;
                    this.RaisePropertyChanged("EnquiryStatus");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long FraudScoreId {
            get {
                return this.FraudScoreIdField;
            }
            set {
                if ((this.FraudScoreIdField.Equals(value) != true)) {
                    this.FraudScoreIdField = value;
                    this.RaisePropertyChanged("FraudScoreId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Rating {
            get {
                return this.RatingField;
            }
            set {
                if ((this.RatingField.Equals(value) != true)) {
                    this.RatingField = value;
                    this.RaisePropertyChanged("Rating");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<string> ReasonCodes {
            get {
                return this.ReasonCodesField;
            }
            set {
                if ((object.ReferenceEquals(this.ReasonCodesField, value) != true)) {
                    this.ReasonCodesField = value;
                    this.RaisePropertyChanged("ReasonCodes");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Atlas.Enumerators.Account.AccountStatus Status {
            get {
                return this.StatusField;
            }
            set {
                if ((this.StatusField.Equals(value) != true)) {
                    this.StatusField = value;
                    this.RaisePropertyChanged("Status");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Atlas.Enumerators.Account.AccountStatusReason StatusReason {
            get {
                return this.StatusReasonField;
            }
            set {
                if ((this.StatusReasonField.Equals(value) != true)) {
                    this.StatusReasonField = value;
                    this.RaisePropertyChanged("StatusReason");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Atlas.Enumerators.Account.AccountStatusSubReason SubStatusReason {
            get {
                return this.SubStatusReasonField;
            }
            set {
                if ((this.SubStatusReasonField.Equals(value) != true)) {
                    this.SubStatusReasonField = value;
                    this.RaisePropertyChanged("SubStatusReason");
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="EnquiryStatus", Namespace="http://schemas.datacontract.org/2004/07/Atlas.ThirdParty.Fraud.TransUnion")]
    public enum EnquiryStatus : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Unknown = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Success = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Error = 2,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="FraudServer.IFraudServer")]
    public interface IFraudServer {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFraudServer/FraudEnquiry", ReplyAction="http://tempuri.org/IFraudServer/FraudEnquiryResponse")]
        Atlas.Online.Node.FraudPreventionNode.FraudServer.FraudResult FraudEnquiry(
                    System.Nullable<long> accountId, 
                    string idNo, 
                    string firstName, 
                    string lastName, 
                    string addressLine1, 
                    string addressLine2, 
                    string suburb, 
                    string city, 
                    string postalCode, 
                    string provinceCode, 
                    string homeTelCode, 
                    string homeTelNo, 
                    string workTelCode, 
                    string workTelNo, 
                    string cellNo, 
                    string bankAccountNo, 
                    string bankName, 
                    string bankBranchCode, 
                    string employer);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFraudServer/FraudEnquiry", ReplyAction="http://tempuri.org/IFraudServer/FraudEnquiryResponse")]
        System.Threading.Tasks.Task<Atlas.Online.Node.FraudPreventionNode.FraudServer.FraudResult> FraudEnquiryAsync(
                    System.Nullable<long> accountId, 
                    string idNo, 
                    string firstName, 
                    string lastName, 
                    string addressLine1, 
                    string addressLine2, 
                    string suburb, 
                    string city, 
                    string postalCode, 
                    string provinceCode, 
                    string homeTelCode, 
                    string homeTelNo, 
                    string workTelCode, 
                    string workTelNo, 
                    string cellNo, 
                    string bankAccountNo, 
                    string bankName, 
                    string bankBranchCode, 
                    string employer);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFraudServer/GetEnquiryForAccount", ReplyAction="http://tempuri.org/IFraudServer/GetEnquiryForAccountResponse")]
        Atlas.Online.Node.FraudPreventionNode.FraudServer.FraudResult GetEnquiryForAccount(long accountId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFraudServer/GetEnquiryForAccount", ReplyAction="http://tempuri.org/IFraudServer/GetEnquiryForAccountResponse")]
        System.Threading.Tasks.Task<Atlas.Online.Node.FraudPreventionNode.FraudServer.FraudResult> GetEnquiryForAccountAsync(long accountId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IFraudServerChannel : Atlas.Online.Node.FraudPreventionNode.FraudServer.IFraudServer, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class FraudServerClient : System.ServiceModel.ClientBase<Atlas.Online.Node.FraudPreventionNode.FraudServer.IFraudServer>, Atlas.Online.Node.FraudPreventionNode.FraudServer.IFraudServer {
        
        public FraudServerClient() {
        }
        
        public FraudServerClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public FraudServerClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FraudServerClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FraudServerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Atlas.Online.Node.FraudPreventionNode.FraudServer.FraudResult FraudEnquiry(
                    System.Nullable<long> accountId, 
                    string idNo, 
                    string firstName, 
                    string lastName, 
                    string addressLine1, 
                    string addressLine2, 
                    string suburb, 
                    string city, 
                    string postalCode, 
                    string provinceCode, 
                    string homeTelCode, 
                    string homeTelNo, 
                    string workTelCode, 
                    string workTelNo, 
                    string cellNo, 
                    string bankAccountNo, 
                    string bankName, 
                    string bankBranchCode, 
                    string employer) {
            return base.Channel.FraudEnquiry(accountId, idNo, firstName, lastName, addressLine1, addressLine2, suburb, city, postalCode, provinceCode, homeTelCode, homeTelNo, workTelCode, workTelNo, cellNo, bankAccountNo, bankName, bankBranchCode, employer);
        }
        
        public System.Threading.Tasks.Task<Atlas.Online.Node.FraudPreventionNode.FraudServer.FraudResult> FraudEnquiryAsync(
                    System.Nullable<long> accountId, 
                    string idNo, 
                    string firstName, 
                    string lastName, 
                    string addressLine1, 
                    string addressLine2, 
                    string suburb, 
                    string city, 
                    string postalCode, 
                    string provinceCode, 
                    string homeTelCode, 
                    string homeTelNo, 
                    string workTelCode, 
                    string workTelNo, 
                    string cellNo, 
                    string bankAccountNo, 
                    string bankName, 
                    string bankBranchCode, 
                    string employer) {
            return base.Channel.FraudEnquiryAsync(accountId, idNo, firstName, lastName, addressLine1, addressLine2, suburb, city, postalCode, provinceCode, homeTelCode, homeTelNo, workTelCode, workTelNo, cellNo, bankAccountNo, bankName, bankBranchCode, employer);
        }
        
        public Atlas.Online.Node.FraudPreventionNode.FraudServer.FraudResult GetEnquiryForAccount(long accountId) {
            return base.Channel.GetEnquiryForAccount(accountId);
        }
        
        public System.Threading.Tasks.Task<Atlas.Online.Node.FraudPreventionNode.FraudServer.FraudResult> GetEnquiryForAccountAsync(long accountId) {
            return base.Channel.GetEnquiryForAccountAsync(accountId);
        }
    }
}