﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KeithLink.Svc.WebApi.com.benekeith.ProfileService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="com.benekeith.ProfileService.IProfileService")]
    public interface IProfileService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/CreateDsrAlias", ReplyAction="http://tempuri.org/IProfileService/CreateDsrAliasResponse")]
        KeithLink.Svc.Core.Models.Profile.DsrAliasModel CreateDsrAlias(System.Guid userId, string email, KeithLink.Svc.Core.Models.Profile.Dsr dsr);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/CreateDsrAlias", ReplyAction="http://tempuri.org/IProfileService/CreateDsrAliasResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Profile.DsrAliasModel> CreateDsrAliasAsync(System.Guid userId, string email, KeithLink.Svc.Core.Models.Profile.Dsr dsr);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/CreateMarketingPref", ReplyAction="http://tempuri.org/IProfileService/CreateMarketingPrefResponse")]
        void CreateMarketingPref(KeithLink.Svc.Core.Models.Profile.MarketingPreferenceModel preference);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/CreateMarketingPref", ReplyAction="http://tempuri.org/IProfileService/CreateMarketingPrefResponse")]
        System.Threading.Tasks.Task CreateMarketingPrefAsync(KeithLink.Svc.Core.Models.Profile.MarketingPreferenceModel preference);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/DeleteDsrAlias", ReplyAction="http://tempuri.org/IProfileService/DeleteDsrAliasResponse")]
        void DeleteDsrAlias(long dsrAliasId, string email);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/DeleteDsrAlias", ReplyAction="http://tempuri.org/IProfileService/DeleteDsrAliasResponse")]
        System.Threading.Tasks.Task DeleteDsrAliasAsync(long dsrAliasId, string email);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/GeneratePasswordForNewUser", ReplyAction="http://tempuri.org/IProfileService/GeneratePasswordForNewUserResponse")]
        void GeneratePasswordForNewUser(string email);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/GeneratePasswordForNewUser", ReplyAction="http://tempuri.org/IProfileService/GeneratePasswordForNewUserResponse")]
        System.Threading.Tasks.Task GeneratePasswordForNewUserAsync(string email);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/GeneratePasswordResetRequest", ReplyAction="http://tempuri.org/IProfileService/GeneratePasswordResetRequestResponse")]
        void GeneratePasswordResetRequest(string email);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/GeneratePasswordResetRequest", ReplyAction="http://tempuri.org/IProfileService/GeneratePasswordResetRequestResponse")]
        System.Threading.Tasks.Task GeneratePasswordResetRequestAsync(string email);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/GetAllDsrAliasesByUserId", ReplyAction="http://tempuri.org/IProfileService/GetAllDsrAliasesByUserIdResponse")]
        KeithLink.Svc.Core.Models.Profile.DsrAliasModel[] GetAllDsrAliasesByUserId(System.Guid userId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/GetAllDsrAliasesByUserId", ReplyAction="http://tempuri.org/IProfileService/GetAllDsrAliasesByUserIdResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Profile.DsrAliasModel[]> GetAllDsrAliasesByUserIdAsync(System.Guid userId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/IsTokenValid", ReplyAction="http://tempuri.org/IProfileService/IsTokenValidResponse")]
        bool IsTokenValid(string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/IsTokenValid", ReplyAction="http://tempuri.org/IProfileService/IsTokenValidResponse")]
        System.Threading.Tasks.Task<bool> IsTokenValidAsync(string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/ReadMarketingPreferences", ReplyAction="http://tempuri.org/IProfileService/ReadMarketingPreferencesResponse")]
        KeithLink.Svc.Core.Models.Profile.MarketingPreferenceModel[] ReadMarketingPreferences(System.DateTime from, System.DateTime to);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/ReadMarketingPreferences", ReplyAction="http://tempuri.org/IProfileService/ReadMarketingPreferencesResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Profile.MarketingPreferenceModel[]> ReadMarketingPreferencesAsync(System.DateTime from, System.DateTime to);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/ResetPassword", ReplyAction="http://tempuri.org/IProfileService/ResetPasswordResponse")]
        bool ResetPassword([System.ServiceModel.MessageParameterAttribute(Name="resetPassword")] KeithLink.Svc.Core.Models.Profile.ResetPasswordModel resetPassword1);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/ResetPassword", ReplyAction="http://tempuri.org/IProfileService/ResetPasswordResponse")]
        System.Threading.Tasks.Task<bool> ResetPasswordAsync(KeithLink.Svc.Core.Models.Profile.ResetPasswordModel resetPassword);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IProfileServiceChannel : KeithLink.Svc.WebApi.com.benekeith.ProfileService.IProfileService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ProfileServiceClient : System.ServiceModel.ClientBase<KeithLink.Svc.WebApi.com.benekeith.ProfileService.IProfileService>, KeithLink.Svc.WebApi.com.benekeith.ProfileService.IProfileService {
        
        public ProfileServiceClient() {
        }
        
        public ProfileServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ProfileServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProfileServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProfileServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public KeithLink.Svc.Core.Models.Profile.DsrAliasModel CreateDsrAlias(System.Guid userId, string email, KeithLink.Svc.Core.Models.Profile.Dsr dsr) {
            return base.Channel.CreateDsrAlias(userId, email, dsr);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Profile.DsrAliasModel> CreateDsrAliasAsync(System.Guid userId, string email, KeithLink.Svc.Core.Models.Profile.Dsr dsr) {
            return base.Channel.CreateDsrAliasAsync(userId, email, dsr);
        }
        
        public void CreateMarketingPref(KeithLink.Svc.Core.Models.Profile.MarketingPreferenceModel preference) {
            base.Channel.CreateMarketingPref(preference);
        }
        
        public System.Threading.Tasks.Task CreateMarketingPrefAsync(KeithLink.Svc.Core.Models.Profile.MarketingPreferenceModel preference) {
            return base.Channel.CreateMarketingPrefAsync(preference);
        }
        
        public void DeleteDsrAlias(long dsrAliasId, string email) {
            base.Channel.DeleteDsrAlias(dsrAliasId, email);
        }
        
        public System.Threading.Tasks.Task DeleteDsrAliasAsync(long dsrAliasId, string email) {
            return base.Channel.DeleteDsrAliasAsync(dsrAliasId, email);
        }
        
        public void GeneratePasswordForNewUser(string email) {
            base.Channel.GeneratePasswordForNewUser(email);
        }
        
        public System.Threading.Tasks.Task GeneratePasswordForNewUserAsync(string email) {
            return base.Channel.GeneratePasswordForNewUserAsync(email);
        }
        
        public void GeneratePasswordResetRequest(string email) {
            base.Channel.GeneratePasswordResetRequest(email);
        }
        
        public System.Threading.Tasks.Task GeneratePasswordResetRequestAsync(string email) {
            return base.Channel.GeneratePasswordResetRequestAsync(email);
        }
        
        public KeithLink.Svc.Core.Models.Profile.DsrAliasModel[] GetAllDsrAliasesByUserId(System.Guid userId) {
            return base.Channel.GetAllDsrAliasesByUserId(userId);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Profile.DsrAliasModel[]> GetAllDsrAliasesByUserIdAsync(System.Guid userId) {
            return base.Channel.GetAllDsrAliasesByUserIdAsync(userId);
        }
        
        public bool IsTokenValid(string token) {
            return base.Channel.IsTokenValid(token);
        }
        
        public System.Threading.Tasks.Task<bool> IsTokenValidAsync(string token) {
            return base.Channel.IsTokenValidAsync(token);
        }
        
        public KeithLink.Svc.Core.Models.Profile.MarketingPreferenceModel[] ReadMarketingPreferences(System.DateTime from, System.DateTime to) {
            return base.Channel.ReadMarketingPreferences(from, to);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Profile.MarketingPreferenceModel[]> ReadMarketingPreferencesAsync(System.DateTime from, System.DateTime to) {
            return base.Channel.ReadMarketingPreferencesAsync(from, to);
        }
        
        public bool ResetPassword(KeithLink.Svc.Core.Models.Profile.ResetPasswordModel resetPassword1) {
            return base.Channel.ResetPassword(resetPassword1);
        }
        
        public System.Threading.Tasks.Task<bool> ResetPasswordAsync(KeithLink.Svc.Core.Models.Profile.ResetPasswordModel resetPassword) {
            return base.Channel.ResetPasswordAsync(resetPassword);
        }
    }
}
