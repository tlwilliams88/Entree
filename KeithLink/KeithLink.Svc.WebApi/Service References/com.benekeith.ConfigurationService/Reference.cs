﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KeithLink.Svc.WebApi.com.benekeith.ConfigurationService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="com.benekeith.ConfigurationService.IConfigurationService")]
    public interface IConfigurationService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConfigurationService/ReadCustomExportOptions", ReplyAction="http://tempuri.org/IConfigurationService/ReadCustomExportOptionsResponse")]
        KeithLink.Svc.Core.Models.ModelExport.ExportOptionsModel ReadCustomExportOptions(System.Guid userId, KeithLink.Svc.Core.Models.Configuration.EF.ExportType type, System.Nullable<long> ListId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConfigurationService/ReadCustomExportOptions", ReplyAction="http://tempuri.org/IConfigurationService/ReadCustomExportOptionsResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.ModelExport.ExportOptionsModel> ReadCustomExportOptionsAsync(System.Guid userId, KeithLink.Svc.Core.Models.Configuration.EF.ExportType type, System.Nullable<long> ListId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConfigurationService/SaveUserExportSettings", ReplyAction="http://tempuri.org/IConfigurationService/SaveUserExportSettingsResponse")]
        void SaveUserExportSettings(System.Guid userId, KeithLink.Svc.Core.Models.Configuration.EF.ExportType type, KeithLink.Svc.Core.Enumerations.List.ListType listType, KeithLink.Svc.Core.Models.ModelExport.ExportModelConfiguration[] configuration, string exportFormat);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConfigurationService/SaveUserExportSettings", ReplyAction="http://tempuri.org/IConfigurationService/SaveUserExportSettingsResponse")]
        System.Threading.Tasks.Task SaveUserExportSettingsAsync(System.Guid userId, KeithLink.Svc.Core.Models.Configuration.EF.ExportType type, KeithLink.Svc.Core.Enumerations.List.ListType listType, KeithLink.Svc.Core.Models.ModelExport.ExportModelConfiguration[] configuration, string exportFormat);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IConfigurationServiceChannel : KeithLink.Svc.WebApi.com.benekeith.ConfigurationService.IConfigurationService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ConfigurationServiceClient : System.ServiceModel.ClientBase<KeithLink.Svc.WebApi.com.benekeith.ConfigurationService.IConfigurationService>, KeithLink.Svc.WebApi.com.benekeith.ConfigurationService.IConfigurationService {
        
        public ConfigurationServiceClient() {
        }
        
        public ConfigurationServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ConfigurationServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConfigurationServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConfigurationServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public KeithLink.Svc.Core.Models.ModelExport.ExportOptionsModel ReadCustomExportOptions(System.Guid userId, KeithLink.Svc.Core.Models.Configuration.EF.ExportType type, System.Nullable<long> ListId) {
            return base.Channel.ReadCustomExportOptions(userId, type, ListId);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.ModelExport.ExportOptionsModel> ReadCustomExportOptionsAsync(System.Guid userId, KeithLink.Svc.Core.Models.Configuration.EF.ExportType type, System.Nullable<long> ListId) {
            return base.Channel.ReadCustomExportOptionsAsync(userId, type, ListId);
        }
        
        public void SaveUserExportSettings(System.Guid userId, KeithLink.Svc.Core.Models.Configuration.EF.ExportType type, KeithLink.Svc.Core.Enumerations.List.ListType listType, KeithLink.Svc.Core.Models.ModelExport.ExportModelConfiguration[] configuration, string exportFormat) {
            base.Channel.SaveUserExportSettings(userId, type, listType, configuration, exportFormat);
        }
        
        public System.Threading.Tasks.Task SaveUserExportSettingsAsync(System.Guid userId, KeithLink.Svc.Core.Models.Configuration.EF.ExportType type, KeithLink.Svc.Core.Enumerations.List.ListType listType, KeithLink.Svc.Core.Models.ModelExport.ExportModelConfiguration[] configuration, string exportFormat) {
            return base.Channel.SaveUserExportSettingsAsync(userId, type, listType, configuration, exportFormat);
        }
    }
}
