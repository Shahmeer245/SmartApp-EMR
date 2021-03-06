﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MazikCareService.BiztalkRepository.RISADTA04Service {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://birch.bupa.com/", ConfigurationName="RISADTA04Service.RISADTA04")]
    public interface RISADTA04 {
        
        // CODEGEN: Generating message contract since the operation GetPatientDetail is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="GetPatientDetail", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        MazikCareService.BiztalkRepository.RISADTA04Service.GetPatientDetailResponse GetPatientDetail(MazikCareService.BiztalkRepository.RISADTA04Service.GetPatientDetailRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="GetPatientDetail", ReplyAction="*")]
        System.Threading.Tasks.Task<MazikCareService.BiztalkRepository.RISADTA04Service.GetPatientDetailResponse> GetPatientDetailAsync(MazikCareService.BiztalkRepository.RISADTA04Service.GetPatientDetailRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1586.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://BTAHL7Schemas.Api.GetPatient")]
    public partial class GetPatient : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string patientIdField;
        
        private string appointmentRecIdField;
        
        private string caseIdField;
        
        private string mergePatientIdField;
        
        private string messageControlIdField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string PatientId {
            get {
                return this.patientIdField;
            }
            set {
                this.patientIdField = value;
                this.RaisePropertyChanged("PatientId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string AppointmentRecId {
            get {
                return this.appointmentRecIdField;
            }
            set {
                this.appointmentRecIdField = value;
                this.RaisePropertyChanged("AppointmentRecId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string CaseId {
            get {
                return this.caseIdField;
            }
            set {
                this.caseIdField = value;
                this.RaisePropertyChanged("CaseId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string MergePatientId {
            get {
                return this.mergePatientIdField;
            }
            set {
                this.mergePatientIdField = value;
                this.RaisePropertyChanged("MergePatientId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string MessageControlId {
            get {
                return this.messageControlIdField;
            }
            set {
                this.messageControlIdField = value;
                this.RaisePropertyChanged("MessageControlId");
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetPatientDetailRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://BTAHL7Schemas.Api.GetPatient", Order=0)]
        public MazikCareService.BiztalkRepository.RISADTA04Service.GetPatient GetPatient;
        
        public GetPatientDetailRequest() {
        }
        
        public GetPatientDetailRequest(MazikCareService.BiztalkRepository.RISADTA04Service.GetPatient GetPatient) {
            this.GetPatient = GetPatient;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetPatientDetailResponse {
        
        public GetPatientDetailResponse() {
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface RISADTA04Channel : MazikCareService.BiztalkRepository.RISADTA04Service.RISADTA04, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RISADTA04Client : System.ServiceModel.ClientBase<MazikCareService.BiztalkRepository.RISADTA04Service.RISADTA04>, MazikCareService.BiztalkRepository.RISADTA04Service.RISADTA04 {
        
        public RISADTA04Client() {
        }
        
        public RISADTA04Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public RISADTA04Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RISADTA04Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RISADTA04Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        MazikCareService.BiztalkRepository.RISADTA04Service.GetPatientDetailResponse MazikCareService.BiztalkRepository.RISADTA04Service.RISADTA04.GetPatientDetail(MazikCareService.BiztalkRepository.RISADTA04Service.GetPatientDetailRequest request) {
            return base.Channel.GetPatientDetail(request);
        }
        
        public void GetPatientDetail(MazikCareService.BiztalkRepository.RISADTA04Service.GetPatient GetPatient) {
            MazikCareService.BiztalkRepository.RISADTA04Service.GetPatientDetailRequest inValue = new MazikCareService.BiztalkRepository.RISADTA04Service.GetPatientDetailRequest();
            inValue.GetPatient = GetPatient;
            MazikCareService.BiztalkRepository.RISADTA04Service.GetPatientDetailResponse retVal = ((MazikCareService.BiztalkRepository.RISADTA04Service.RISADTA04)(this)).GetPatientDetail(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<MazikCareService.BiztalkRepository.RISADTA04Service.GetPatientDetailResponse> MazikCareService.BiztalkRepository.RISADTA04Service.RISADTA04.GetPatientDetailAsync(MazikCareService.BiztalkRepository.RISADTA04Service.GetPatientDetailRequest request) {
            return base.Channel.GetPatientDetailAsync(request);
        }
        
        public System.Threading.Tasks.Task<MazikCareService.BiztalkRepository.RISADTA04Service.GetPatientDetailResponse> GetPatientDetailAsync(MazikCareService.BiztalkRepository.RISADTA04Service.GetPatient GetPatient) {
            MazikCareService.BiztalkRepository.RISADTA04Service.GetPatientDetailRequest inValue = new MazikCareService.BiztalkRepository.RISADTA04Service.GetPatientDetailRequest();
            inValue.GetPatient = GetPatient;
            return ((MazikCareService.BiztalkRepository.RISADTA04Service.RISADTA04)(this)).GetPatientDetailAsync(inValue);
        }
    }
}
