﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KeithLink.Svc.WebApi.com.benekeith.OrderService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="com.benekeith.OrderService.IOrderService")]
    public interface IOrderService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/ReadLatestOrderModifiedDateForCustomer", ReplyAction="http://tempuri.org/IOrderService/ReadLatestOrderModifiedDateForCustomerResponse")]
        System.Nullable<System.DateTime> ReadLatestOrderModifiedDateForCustomer(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/ReadLatestOrderModifiedDateForCustomer", ReplyAction="http://tempuri.org/IOrderService/ReadLatestOrderModifiedDateForCustomerResponse")]
        System.Threading.Tasks.Task<System.Nullable<System.DateTime>> ReadLatestOrderModifiedDateForCustomerAsync(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetLastFiveOrderHistory", ReplyAction="http://tempuri.org/IOrderService/GetLastFiveOrderHistoryResponse")]
        KeithLink.Svc.Core.Models.Orders.History.OrderHistoryFile[] GetLastFiveOrderHistory(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, string itemNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetLastFiveOrderHistory", ReplyAction="http://tempuri.org/IOrderService/GetLastFiveOrderHistoryResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.History.OrderHistoryFile[]> GetLastFiveOrderHistoryAsync(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, string itemNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetCustomerOrders", ReplyAction="http://tempuri.org/IOrderService/GetCustomerOrdersResponse")]
        KeithLink.Svc.Core.Models.Orders.Order[] GetCustomerOrders(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetCustomerOrders", ReplyAction="http://tempuri.org/IOrderService/GetCustomerOrdersResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.Order[]> GetCustomerOrdersAsync(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetUserActiveCart", ReplyAction="http://tempuri.org/IOrderService/GetUserActiveCartResponse")]
        KeithLink.Svc.Core.Models.Orders.UserActiveCartModel GetUserActiveCart(System.Guid userId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetUserActiveCart", ReplyAction="http://tempuri.org/IOrderService/GetUserActiveCartResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.UserActiveCartModel> GetUserActiveCartAsync(System.Guid userId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/SaveUserActiveCart", ReplyAction="http://tempuri.org/IOrderService/SaveUserActiveCartResponse")]
        void SaveUserActiveCart(System.Guid userId, System.Guid cartId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/SaveUserActiveCart", ReplyAction="http://tempuri.org/IOrderService/SaveUserActiveCartResponse")]
        System.Threading.Tasks.Task SaveUserActiveCartAsync(System.Guid userId, System.Guid cartId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IOrderServiceChannel : KeithLink.Svc.WebApi.com.benekeith.OrderService.IOrderService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class OrderServiceClient : System.ServiceModel.ClientBase<KeithLink.Svc.WebApi.com.benekeith.OrderService.IOrderService>, KeithLink.Svc.WebApi.com.benekeith.OrderService.IOrderService {
        
        public OrderServiceClient() {
        }
        
        public OrderServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public OrderServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OrderServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OrderServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Nullable<System.DateTime> ReadLatestOrderModifiedDateForCustomer(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.ReadLatestOrderModifiedDateForCustomer(catalogInfo);
        }
        
        public System.Threading.Tasks.Task<System.Nullable<System.DateTime>> ReadLatestOrderModifiedDateForCustomerAsync(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.ReadLatestOrderModifiedDateForCustomerAsync(catalogInfo);
        }
        
        public KeithLink.Svc.Core.Models.Orders.History.OrderHistoryFile[] GetLastFiveOrderHistory(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, string itemNumber) {
            return base.Channel.GetLastFiveOrderHistory(catalogInfo, itemNumber);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.History.OrderHistoryFile[]> GetLastFiveOrderHistoryAsync(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, string itemNumber) {
            return base.Channel.GetLastFiveOrderHistoryAsync(catalogInfo, itemNumber);
        }
        
        public KeithLink.Svc.Core.Models.Orders.Order[] GetCustomerOrders(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.GetCustomerOrders(catalogInfo);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.Order[]> GetCustomerOrdersAsync(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.GetCustomerOrdersAsync(catalogInfo);
        }
        
        public KeithLink.Svc.Core.Models.Orders.UserActiveCartModel GetUserActiveCart(System.Guid userId) {
            return base.Channel.GetUserActiveCart(userId);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.UserActiveCartModel> GetUserActiveCartAsync(System.Guid userId) {
            return base.Channel.GetUserActiveCartAsync(userId);
        }
        
        public void SaveUserActiveCart(System.Guid userId, System.Guid cartId) {
            base.Channel.SaveUserActiveCart(userId, cartId);
        }
        
        public System.Threading.Tasks.Task SaveUserActiveCartAsync(System.Guid userId, System.Guid cartId) {
            return base.Channel.SaveUserActiveCartAsync(userId, cartId);
        }
    }
}
