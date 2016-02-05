﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
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
        KeithLink.Svc.Core.Models.Orders.Order[] GetCustomerOrders(System.Guid userId, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetCustomerOrders", ReplyAction="http://tempuri.org/IOrderService/GetCustomerOrdersResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.Order[]> GetCustomerOrdersAsync(System.Guid userId, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetOrder", ReplyAction="http://tempuri.org/IOrderService/GetOrderResponse")]
        KeithLink.Svc.Core.Models.Orders.Order GetOrder(string branchId, string invoiceNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetOrder", ReplyAction="http://tempuri.org/IOrderService/GetOrderResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.Order> GetOrderAsync(string branchId, string invoiceNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetOrderHeaderInDateRange", ReplyAction="http://tempuri.org/IOrderService/GetOrderHeaderInDateRangeResponse")]
        KeithLink.Svc.Core.Models.Orders.Order[] GetOrderHeaderInDateRange(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext customerInfo, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetOrderHeaderInDateRange", ReplyAction="http://tempuri.org/IOrderService/GetOrderHeaderInDateRangeResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.Order[]> GetOrderHeaderInDateRangeAsync(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext customerInfo, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetOrderTotalByMonth", ReplyAction="http://tempuri.org/IOrderService/GetOrderTotalByMonthResponse")]
        KeithLink.Svc.Core.Models.Orders.OrderTotalByMonth GetOrderTotalByMonth(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext customerInfo, int numberOfMonths);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetOrderTotalByMonth", ReplyAction="http://tempuri.org/IOrderService/GetOrderTotalByMonthResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.OrderTotalByMonth> GetOrderTotalByMonthAsync(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext customerInfo, int numberOfMonths);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetUserActiveCart", ReplyAction="http://tempuri.org/IOrderService/GetUserActiveCartResponse")]
        KeithLink.Svc.Core.Models.Orders.UserActiveCartModel GetUserActiveCart(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, System.Guid userId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetUserActiveCart", ReplyAction="http://tempuri.org/IOrderService/GetUserActiveCartResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.UserActiveCartModel> GetUserActiveCartAsync(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, System.Guid userId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/SaveUserActiveCart", ReplyAction="http://tempuri.org/IOrderService/SaveUserActiveCartResponse")]
        void SaveUserActiveCart(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, System.Guid userId, System.Guid cartId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/SaveUserActiveCart", ReplyAction="http://tempuri.org/IOrderService/SaveUserActiveCartResponse")]
        System.Threading.Tasks.Task SaveUserActiveCartAsync(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, System.Guid userId, System.Guid cartId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/SaveOrderHistory", ReplyAction="http://tempuri.org/IOrderService/SaveOrderHistoryResponse")]
        void SaveOrderHistory(KeithLink.Svc.Core.Models.Orders.History.OrderHistoryFile historyFile, bool isSpecialOrder);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/SaveOrderHistory", ReplyAction="http://tempuri.org/IOrderService/SaveOrderHistoryResponse")]
        System.Threading.Tasks.Task SaveOrderHistoryAsync(KeithLink.Svc.Core.Models.Orders.History.OrderHistoryFile historyFile, bool isSpecialOrder);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetSubmittedUnconfirmedOrders", ReplyAction="http://tempuri.org/IOrderService/GetSubmittedUnconfirmedOrdersResponse")]
        KeithLink.Svc.Core.Models.Orders.OrderHeader[] GetSubmittedUnconfirmedOrders();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetSubmittedUnconfirmedOrders", ReplyAction="http://tempuri.org/IOrderService/GetSubmittedUnconfirmedOrdersResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.OrderHeader[]> GetSubmittedUnconfirmedOrdersAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetUserIdForControlNumber", ReplyAction="http://tempuri.org/IOrderService/GetUserIdForControlNumberResponse")]
        System.Guid GetUserIdForControlNumber(int controlNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetUserIdForControlNumber", ReplyAction="http://tempuri.org/IOrderService/GetUserIdForControlNumberResponse")]
        System.Threading.Tasks.Task<System.Guid> GetUserIdForControlNumberAsync(int controlNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetPagedOrders", ReplyAction="http://tempuri.org/IOrderService/GetPagedOrdersResponse")]
        KeithLink.Svc.Core.Models.Paging.PagedResults<KeithLink.Svc.Core.Models.Orders.Order> GetPagedOrders(System.Guid userId, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext customerInfo, KeithLink.Svc.Core.Models.Paging.PagingModel paging);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOrderService/GetPagedOrders", ReplyAction="http://tempuri.org/IOrderService/GetPagedOrdersResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Paging.PagedResults<KeithLink.Svc.Core.Models.Orders.Order>> GetPagedOrdersAsync(System.Guid userId, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext customerInfo, KeithLink.Svc.Core.Models.Paging.PagingModel paging);
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
        
        public KeithLink.Svc.Core.Models.Orders.Order[] GetCustomerOrders(System.Guid userId, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.GetCustomerOrders(userId, catalogInfo);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.Order[]> GetCustomerOrdersAsync(System.Guid userId, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.GetCustomerOrdersAsync(userId, catalogInfo);
        }
        
        public KeithLink.Svc.Core.Models.Orders.Order GetOrder(string branchId, string invoiceNumber) {
            return base.Channel.GetOrder(branchId, invoiceNumber);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.Order> GetOrderAsync(string branchId, string invoiceNumber) {
            return base.Channel.GetOrderAsync(branchId, invoiceNumber);
        }
        
        public KeithLink.Svc.Core.Models.Orders.Order[] GetOrderHeaderInDateRange(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext customerInfo, System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.GetOrderHeaderInDateRange(customerInfo, startDate, endDate);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.Order[]> GetOrderHeaderInDateRangeAsync(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext customerInfo, System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.GetOrderHeaderInDateRangeAsync(customerInfo, startDate, endDate);
        }
        
        public KeithLink.Svc.Core.Models.Orders.OrderTotalByMonth GetOrderTotalByMonth(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext customerInfo, int numberOfMonths) {
            return base.Channel.GetOrderTotalByMonth(customerInfo, numberOfMonths);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.OrderTotalByMonth> GetOrderTotalByMonthAsync(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext customerInfo, int numberOfMonths) {
            return base.Channel.GetOrderTotalByMonthAsync(customerInfo, numberOfMonths);
        }
        
        public KeithLink.Svc.Core.Models.Orders.UserActiveCartModel GetUserActiveCart(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, System.Guid userId) {
            return base.Channel.GetUserActiveCart(catalogInfo, userId);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.UserActiveCartModel> GetUserActiveCartAsync(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, System.Guid userId) {
            return base.Channel.GetUserActiveCartAsync(catalogInfo, userId);
        }
        
        public void SaveUserActiveCart(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, System.Guid userId, System.Guid cartId) {
            base.Channel.SaveUserActiveCart(catalogInfo, userId, cartId);
        }
        
        public System.Threading.Tasks.Task SaveUserActiveCartAsync(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, System.Guid userId, System.Guid cartId) {
            return base.Channel.SaveUserActiveCartAsync(catalogInfo, userId, cartId);
        }
        
        public void SaveOrderHistory(KeithLink.Svc.Core.Models.Orders.History.OrderHistoryFile historyFile, bool isSpecialOrder) {
            base.Channel.SaveOrderHistory(historyFile, isSpecialOrder);
        }
        
        public System.Threading.Tasks.Task SaveOrderHistoryAsync(KeithLink.Svc.Core.Models.Orders.History.OrderHistoryFile historyFile, bool isSpecialOrder) {
            return base.Channel.SaveOrderHistoryAsync(historyFile, isSpecialOrder);
        }
        
        public KeithLink.Svc.Core.Models.Orders.OrderHeader[] GetSubmittedUnconfirmedOrders() {
            return base.Channel.GetSubmittedUnconfirmedOrders();
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Orders.OrderHeader[]> GetSubmittedUnconfirmedOrdersAsync() {
            return base.Channel.GetSubmittedUnconfirmedOrdersAsync();
        }
        
        public System.Guid GetUserIdForControlNumber(int controlNumber) {
            return base.Channel.GetUserIdForControlNumber(controlNumber);
        }
        
        public System.Threading.Tasks.Task<System.Guid> GetUserIdForControlNumberAsync(int controlNumber) {
            return base.Channel.GetUserIdForControlNumberAsync(controlNumber);
        }
        
        public KeithLink.Svc.Core.Models.Paging.PagedResults<KeithLink.Svc.Core.Models.Orders.Order> GetPagedOrders(System.Guid userId, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext customerInfo, KeithLink.Svc.Core.Models.Paging.PagingModel paging) {
            return base.Channel.GetPagedOrders(userId, customerInfo, paging);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Paging.PagedResults<KeithLink.Svc.Core.Models.Orders.Order>> GetPagedOrdersAsync(System.Guid userId, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext customerInfo, KeithLink.Svc.Core.Models.Paging.PagingModel paging) {
            return base.Channel.GetPagedOrdersAsync(userId, customerInfo, paging);
        }
    }
}
