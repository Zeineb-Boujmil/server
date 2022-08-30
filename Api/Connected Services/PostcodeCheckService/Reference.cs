﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Api.PostcodeCheckService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="RequestBase", Namespace="http://schemas.datacontract.org/2004/07/CED.PostcodeCheckService.Service")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Api.PostcodeCheckService.GetPostcodeByAddressRequest))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Api.PostcodeCheckService.GetAddressByPostcodeRequest))]
    public partial class RequestBase : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="GetPostcodeByAddressRequest", Namespace="http://schemas.datacontract.org/2004/07/CED.PostcodeCheckService.Service")]
    [System.SerializableAttribute()]
    public partial class GetPostcodeByAddressRequest : Api.PostcodeCheckService.RequestBase {
        
        private string CityField;
        
        private int HousenumberField;
        
        private string StreetField;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string City {
            get {
                return this.CityField;
            }
            set {
                if ((object.ReferenceEquals(this.CityField, value) != true)) {
                    this.CityField = value;
                    this.RaisePropertyChanged("City");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int Housenumber {
            get {
                return this.HousenumberField;
            }
            set {
                if ((this.HousenumberField.Equals(value) != true)) {
                    this.HousenumberField = value;
                    this.RaisePropertyChanged("Housenumber");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Street {
            get {
                return this.StreetField;
            }
            set {
                if ((object.ReferenceEquals(this.StreetField, value) != true)) {
                    this.StreetField = value;
                    this.RaisePropertyChanged("Street");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="GetAddressByPostcodeRequest", Namespace="http://schemas.datacontract.org/2004/07/CED.PostcodeCheckService.Service")]
    [System.SerializableAttribute()]
    public partial class GetAddressByPostcodeRequest : Api.PostcodeCheckService.RequestBase {
        
        private int HousenumberField;
        
        private string PostcodeField;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int Housenumber {
            get {
                return this.HousenumberField;
            }
            set {
                if ((this.HousenumberField.Equals(value) != true)) {
                    this.HousenumberField = value;
                    this.RaisePropertyChanged("Housenumber");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Postcode {
            get {
                return this.PostcodeField;
            }
            set {
                if ((object.ReferenceEquals(this.PostcodeField, value) != true)) {
                    this.PostcodeField = value;
                    this.RaisePropertyChanged("Postcode");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ResponseBase", Namespace="http://schemas.datacontract.org/2004/07/CED.PostcodeCheckService.Service")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Api.PostcodeCheckService.GetPostcodeByAddressResponse))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Api.PostcodeCheckService.GetAddressByPostcodeResponse))]
    public partial class ResponseBase : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorMessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long ExecutionTimeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool HasErrorsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<System.Guid> LookupIdField;
        
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
        public string ErrorCode {
            get {
                return this.ErrorCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorCodeField, value) != true)) {
                    this.ErrorCodeField = value;
                    this.RaisePropertyChanged("ErrorCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorMessage {
            get {
                return this.ErrorMessageField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorMessageField, value) != true)) {
                    this.ErrorMessageField = value;
                    this.RaisePropertyChanged("ErrorMessage");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long ExecutionTime {
            get {
                return this.ExecutionTimeField;
            }
            set {
                if ((this.ExecutionTimeField.Equals(value) != true)) {
                    this.ExecutionTimeField = value;
                    this.RaisePropertyChanged("ExecutionTime");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool HasErrors {
            get {
                return this.HasErrorsField;
            }
            set {
                if ((this.HasErrorsField.Equals(value) != true)) {
                    this.HasErrorsField = value;
                    this.RaisePropertyChanged("HasErrors");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.Guid> LookupId {
            get {
                return this.LookupIdField;
            }
            set {
                if ((this.LookupIdField.Equals(value) != true)) {
                    this.LookupIdField = value;
                    this.RaisePropertyChanged("LookupId");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="GetPostcodeByAddressResponse", Namespace="http://schemas.datacontract.org/2004/07/CED.PostcodeCheckService.Service")]
    [System.SerializableAttribute()]
    public partial class GetPostcodeByAddressResponse : Api.PostcodeCheckService.ResponseBase {
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PostcodeField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Postcode {
            get {
                return this.PostcodeField;
            }
            set {
                if ((object.ReferenceEquals(this.PostcodeField, value) != true)) {
                    this.PostcodeField = value;
                    this.RaisePropertyChanged("Postcode");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="GetAddressByPostcodeResponse", Namespace="http://schemas.datacontract.org/2004/07/CED.PostcodeCheckService.Service")]
    [System.SerializableAttribute()]
    public partial class GetAddressByPostcodeResponse : Api.PostcodeCheckService.ResponseBase {
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CityField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string StreetField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string City {
            get {
                return this.CityField;
            }
            set {
                if ((object.ReferenceEquals(this.CityField, value) != true)) {
                    this.CityField = value;
                    this.RaisePropertyChanged("City");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Street {
            get {
                return this.StreetField;
            }
            set {
                if ((object.ReferenceEquals(this.StreetField, value) != true)) {
                    this.StreetField = value;
                    this.RaisePropertyChanged("Street");
                }
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://ced.com/PostcodeCheckService", ConfigurationName="PostcodeCheckService.IPostcodeCheckService")]
    public interface IPostcodeCheckService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://ced.com/PostcodeCheckService/IPostcodeCheckService/GetAddressByPostcode", ReplyAction="http://ced.com/PostcodeCheckService/IPostcodeCheckService/GetAddressByPostcodeRes" +
            "ponse")]
        Api.PostcodeCheckService.GetAddressByPostcodeResponse GetAddressByPostcode(Api.PostcodeCheckService.GetAddressByPostcodeRequest getAddressByPostcodeRequest);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://ced.com/PostcodeCheckService/IPostcodeCheckService/GetAddressByPostcode", ReplyAction="http://ced.com/PostcodeCheckService/IPostcodeCheckService/GetAddressByPostcodeRes" +
            "ponse")]
        System.Threading.Tasks.Task<Api.PostcodeCheckService.GetAddressByPostcodeResponse> GetAddressByPostcodeAsync(Api.PostcodeCheckService.GetAddressByPostcodeRequest getAddressByPostcodeRequest);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://ced.com/PostcodeCheckService/IPostcodeCheckService/GetPostcodeByAddress", ReplyAction="http://ced.com/PostcodeCheckService/IPostcodeCheckService/GetPostcodeByAddressRes" +
            "ponse")]
        Api.PostcodeCheckService.GetPostcodeByAddressResponse GetPostcodeByAddress(Api.PostcodeCheckService.GetPostcodeByAddressRequest getPostcodeByAddressRequest);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://ced.com/PostcodeCheckService/IPostcodeCheckService/GetPostcodeByAddress", ReplyAction="http://ced.com/PostcodeCheckService/IPostcodeCheckService/GetPostcodeByAddressRes" +
            "ponse")]
        System.Threading.Tasks.Task<Api.PostcodeCheckService.GetPostcodeByAddressResponse> GetPostcodeByAddressAsync(Api.PostcodeCheckService.GetPostcodeByAddressRequest getPostcodeByAddressRequest);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPostcodeCheckServiceChannel : Api.PostcodeCheckService.IPostcodeCheckService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PostcodeCheckServiceClient : System.ServiceModel.ClientBase<Api.PostcodeCheckService.IPostcodeCheckService>, Api.PostcodeCheckService.IPostcodeCheckService {
        
        public PostcodeCheckServiceClient() {
        }
        
        public PostcodeCheckServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PostcodeCheckServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PostcodeCheckServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PostcodeCheckServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Api.PostcodeCheckService.GetAddressByPostcodeResponse GetAddressByPostcode(Api.PostcodeCheckService.GetAddressByPostcodeRequest getAddressByPostcodeRequest) {
            return base.Channel.GetAddressByPostcode(getAddressByPostcodeRequest);
        }
        
        public System.Threading.Tasks.Task<Api.PostcodeCheckService.GetAddressByPostcodeResponse> GetAddressByPostcodeAsync(Api.PostcodeCheckService.GetAddressByPostcodeRequest getAddressByPostcodeRequest) {
            return base.Channel.GetAddressByPostcodeAsync(getAddressByPostcodeRequest);
        }
        
        public Api.PostcodeCheckService.GetPostcodeByAddressResponse GetPostcodeByAddress(Api.PostcodeCheckService.GetPostcodeByAddressRequest getPostcodeByAddressRequest) {
            return base.Channel.GetPostcodeByAddress(getPostcodeByAddressRequest);
        }
        
        public System.Threading.Tasks.Task<Api.PostcodeCheckService.GetPostcodeByAddressResponse> GetPostcodeByAddressAsync(Api.PostcodeCheckService.GetPostcodeByAddressRequest getPostcodeByAddressRequest) {
            return base.Channel.GetPostcodeByAddressAsync(getPostcodeByAddressRequest);
        }
    }
}
