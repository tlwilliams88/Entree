﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KeithLink.Svc.Impl.com.benekeith.PricingService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="com.benekeith.PricingService.PricingSoap")]
    public interface PricingSoap {
        
        // CODEGEN: Generating message contract since element name XmlFile from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Calculate", ReplyAction="*")]
        KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateResponse Calculate(KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Calculate", ReplyAction="*")]
        System.Threading.Tasks.Task<KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateResponse> CalculateAsync(KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class CalculateRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Calculate", Namespace="http://tempuri.org/", Order=0)]
        public KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateRequestBody Body;
        
        public CalculateRequest() {
        }
        
        public CalculateRequest(KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class CalculateRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string XmlFile;
        
        public CalculateRequestBody() {
        }
        
        public CalculateRequestBody(string XmlFile) {
            this.XmlFile = XmlFile;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class CalculateResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="CalculateResponse", Namespace="http://tempuri.org/", Order=0)]
        public KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateResponseBody Body;
        
        public CalculateResponse() {
        }
        
        public CalculateResponse(KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class CalculateResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string CalculateResult;
        
        public CalculateResponseBody() {
        }
        
        public CalculateResponseBody(string CalculateResult) {
            this.CalculateResult = CalculateResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface PricingSoapChannel : KeithLink.Svc.Impl.com.benekeith.PricingService.PricingSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PricingSoapClient : System.ServiceModel.ClientBase<KeithLink.Svc.Impl.com.benekeith.PricingService.PricingSoap>, KeithLink.Svc.Impl.com.benekeith.PricingService.PricingSoap {
        
        public PricingSoapClient() {
        }
        
        public PricingSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PricingSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PricingSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PricingSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateResponse KeithLink.Svc.Impl.com.benekeith.PricingService.PricingSoap.Calculate(KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateRequest request) {
            return base.Channel.Calculate(request);
        }
        
        public string Calculate(string XmlFile) {
            KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateRequest inValue = new KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateRequest();
            inValue.Body = new KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateRequestBody();
            inValue.Body.XmlFile = XmlFile;
            KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateResponse retVal = ((KeithLink.Svc.Impl.com.benekeith.PricingService.PricingSoap)(this)).Calculate(inValue);
            return retVal.Body.CalculateResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateResponse> KeithLink.Svc.Impl.com.benekeith.PricingService.PricingSoap.CalculateAsync(KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateRequest request) {
            return base.Channel.CalculateAsync(request);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateResponse> CalculateAsync(string XmlFile) {
            KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateRequest inValue = new KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateRequest();
            inValue.Body = new KeithLink.Svc.Impl.com.benekeith.PricingService.CalculateRequestBody();
            inValue.Body.XmlFile = XmlFile;
            return ((KeithLink.Svc.Impl.com.benekeith.PricingService.PricingSoap)(this)).CalculateAsync(inValue);
        }
    }
}
