﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KeithLink.Svc.WebApi.com.benekeith.ListService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="com.benekeith.ListService.IListServcie")]
    public interface IListServcie {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/AddItems", ReplyAction="http://tempuri.org/IListServcie/AddItemsResponse")]
        KeithLink.Svc.Core.Models.Lists.ListModel AddItems(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, long listId, KeithLink.Svc.Core.Models.Lists.ListItemModel[] items);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/AddItems", ReplyAction="http://tempuri.org/IListServcie/AddItemsResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Lists.ListModel> AddItemsAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, long listId, KeithLink.Svc.Core.Models.Lists.ListItemModel[] items);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/AddItem", ReplyAction="http://tempuri.org/IListServcie/AddItemResponse")]
        System.Nullable<long> AddItem(long listId, KeithLink.Svc.Core.Models.Lists.ListItemModel item);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/AddItem", ReplyAction="http://tempuri.org/IListServcie/AddItemResponse")]
        System.Threading.Tasks.Task<System.Nullable<long>> AddItemAsync(long listId, KeithLink.Svc.Core.Models.Lists.ListItemModel item);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/AddNote", ReplyAction="http://tempuri.org/IListServcie/AddNoteResponse")]
        void AddNote(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, KeithLink.Svc.Core.Models.SiteCatalog.ItemNote newNote);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/AddNote", ReplyAction="http://tempuri.org/IListServcie/AddNoteResponse")]
        System.Threading.Tasks.Task AddNoteAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, KeithLink.Svc.Core.Models.SiteCatalog.ItemNote newNote);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/AddRecentlyViewedItem", ReplyAction="http://tempuri.org/IListServcie/AddRecentlyViewedItemResponse")]
        void AddRecentlyViewedItem(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, string itemNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/AddRecentlyViewedItem", ReplyAction="http://tempuri.org/IListServcie/AddRecentlyViewedItemResponse")]
        System.Threading.Tasks.Task AddRecentlyViewedItemAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, string itemNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/CreateList", ReplyAction="http://tempuri.org/IListServcie/CreateListResponse")]
        long CreateList(System.Guid userId, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, KeithLink.Svc.Core.Models.Lists.ListModel list, KeithLink.Svc.Core.Enumerations.List.ListType type);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/CreateList", ReplyAction="http://tempuri.org/IListServcie/CreateListResponse")]
        System.Threading.Tasks.Task<long> CreateListAsync(System.Guid userId, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, KeithLink.Svc.Core.Models.Lists.ListModel list, KeithLink.Svc.Core.Enumerations.List.ListType type);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/DeleteItem", ReplyAction="http://tempuri.org/IListServcie/DeleteItemResponse")]
        void DeleteItem(long Id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/DeleteItem", ReplyAction="http://tempuri.org/IListServcie/DeleteItemResponse")]
        System.Threading.Tasks.Task DeleteItemAsync(long Id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/DeleteList", ReplyAction="http://tempuri.org/IListServcie/DeleteListResponse")]
        void DeleteList(long Id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/DeleteList", ReplyAction="http://tempuri.org/IListServcie/DeleteListResponse")]
        System.Threading.Tasks.Task DeleteListAsync(long Id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/DeleteNote", ReplyAction="http://tempuri.org/IListServcie/DeleteNoteResponse")]
        void DeleteNote(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, string ItemNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/DeleteNote", ReplyAction="http://tempuri.org/IListServcie/DeleteNoteResponse")]
        System.Threading.Tasks.Task DeleteNoteAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, string ItemNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadFavorites", ReplyAction="http://tempuri.org/IListServcie/ReadFavoritesResponse")]
        string[] ReadFavorites(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadFavorites", ReplyAction="http://tempuri.org/IListServcie/ReadFavoritesResponse")]
        System.Threading.Tasks.Task<string[]> ReadFavoritesAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadList", ReplyAction="http://tempuri.org/IListServcie/ReadListResponse")]
        KeithLink.Svc.Core.Models.Lists.ListModel ReadList(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, long Id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadList", ReplyAction="http://tempuri.org/IListServcie/ReadListResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Lists.ListModel> ReadListAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, long Id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadListByType", ReplyAction="http://tempuri.org/IListServcie/ReadListByTypeResponse")]
        KeithLink.Svc.Core.Models.Lists.ListModel[] ReadListByType(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, KeithLink.Svc.Core.Enumerations.List.ListType type);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadListByType", ReplyAction="http://tempuri.org/IListServcie/ReadListByTypeResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Lists.ListModel[]> ReadListByTypeAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, KeithLink.Svc.Core.Enumerations.List.ListType type);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadListLabels", ReplyAction="http://tempuri.org/IListServcie/ReadListLabelsResponse")]
        string[] ReadListLabels(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadListLabels", ReplyAction="http://tempuri.org/IListServcie/ReadListLabelsResponse")]
        System.Threading.Tasks.Task<string[]> ReadListLabelsAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadNotes", ReplyAction="http://tempuri.org/IListServcie/ReadNotesResponse")]
        KeithLink.Svc.Core.Models.Lists.ListItemModel[] ReadNotes(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadNotes", ReplyAction="http://tempuri.org/IListServcie/ReadNotesResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Lists.ListItemModel[]> ReadNotesAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadRecent", ReplyAction="http://tempuri.org/IListServcie/ReadRecentResponse")]
        KeithLink.Svc.Core.Models.Lists.RecentItem[] ReadRecent(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadRecent", ReplyAction="http://tempuri.org/IListServcie/ReadRecentResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Lists.RecentItem[]> ReadRecentAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadReminders", ReplyAction="http://tempuri.org/IListServcie/ReadRemindersResponse")]
        KeithLink.Svc.Core.Models.Lists.ListModel[] ReadReminders(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadReminders", ReplyAction="http://tempuri.org/IListServcie/ReadRemindersResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Lists.ListModel[]> ReadRemindersAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadUserList", ReplyAction="http://tempuri.org/IListServcie/ReadUserListResponse")]
        KeithLink.Svc.Core.Models.Lists.ListModel[] ReadUserList(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, bool headerOnly);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadUserList", ReplyAction="http://tempuri.org/IListServcie/ReadUserListResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Lists.ListModel[]> ReadUserListAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, bool headerOnly);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/UpdateItem", ReplyAction="http://tempuri.org/IListServcie/UpdateItemResponse")]
        void UpdateItem(KeithLink.Svc.Core.Models.Lists.ListItemModel item);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/UpdateItem", ReplyAction="http://tempuri.org/IListServcie/UpdateItemResponse")]
        System.Threading.Tasks.Task UpdateItemAsync(KeithLink.Svc.Core.Models.Lists.ListItemModel item);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/UpdateList", ReplyAction="http://tempuri.org/IListServcie/UpdateListResponse")]
        void UpdateList(KeithLink.Svc.Core.Models.Lists.ListModel userList);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/UpdateList", ReplyAction="http://tempuri.org/IListServcie/UpdateListResponse")]
        System.Threading.Tasks.Task UpdateListAsync(KeithLink.Svc.Core.Models.Lists.ListModel userList);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/CopyList", ReplyAction="http://tempuri.org/IListServcie/CopyListResponse")]
        void CopyList(KeithLink.Svc.Core.Models.Lists.ListCopyShareModel copyListModel);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/CopyList", ReplyAction="http://tempuri.org/IListServcie/CopyListResponse")]
        System.Threading.Tasks.Task CopyListAsync(KeithLink.Svc.Core.Models.Lists.ListCopyShareModel copyListModel);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ShareList", ReplyAction="http://tempuri.org/IListServcie/ShareListResponse")]
        void ShareList(KeithLink.Svc.Core.Models.Lists.ListCopyShareModel shareListModel);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ShareList", ReplyAction="http://tempuri.org/IListServcie/ShareListResponse")]
        System.Threading.Tasks.Task ShareListAsync(KeithLink.Svc.Core.Models.Lists.ListCopyShareModel shareListModel);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadRecommendedItemsList", ReplyAction="http://tempuri.org/IListServcie/ReadRecommendedItemsListResponse")]
        KeithLink.Svc.Core.Models.Lists.RecommendedItemModel[] ReadRecommendedItemsList(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IListServcie/ReadRecommendedItemsList", ReplyAction="http://tempuri.org/IListServcie/ReadRecommendedItemsListResponse")]
        System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Lists.RecommendedItemModel[]> ReadRecommendedItemsListAsync(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IListServcieChannel : KeithLink.Svc.WebApi.com.benekeith.ListService.IListServcie, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ListServcieClient : System.ServiceModel.ClientBase<KeithLink.Svc.WebApi.com.benekeith.ListService.IListServcie>, KeithLink.Svc.WebApi.com.benekeith.ListService.IListServcie {
        
        public ListServcieClient() {
        }
        
        public ListServcieClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ListServcieClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ListServcieClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ListServcieClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public KeithLink.Svc.Core.Models.Lists.ListModel AddItems(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, long listId, KeithLink.Svc.Core.Models.Lists.ListItemModel[] items) {
            return base.Channel.AddItems(user, catalogInfo, listId, items);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Lists.ListModel> AddItemsAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, long listId, KeithLink.Svc.Core.Models.Lists.ListItemModel[] items) {
            return base.Channel.AddItemsAsync(user, catalogInfo, listId, items);
        }
        
        public System.Nullable<long> AddItem(long listId, KeithLink.Svc.Core.Models.Lists.ListItemModel item) {
            return base.Channel.AddItem(listId, item);
        }
        
        public System.Threading.Tasks.Task<System.Nullable<long>> AddItemAsync(long listId, KeithLink.Svc.Core.Models.Lists.ListItemModel item) {
            return base.Channel.AddItemAsync(listId, item);
        }
        
        public void AddNote(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, KeithLink.Svc.Core.Models.SiteCatalog.ItemNote newNote) {
            base.Channel.AddNote(user, catalogInfo, newNote);
        }
        
        public System.Threading.Tasks.Task AddNoteAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, KeithLink.Svc.Core.Models.SiteCatalog.ItemNote newNote) {
            return base.Channel.AddNoteAsync(user, catalogInfo, newNote);
        }
        
        public void AddRecentlyViewedItem(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, string itemNumber) {
            base.Channel.AddRecentlyViewedItem(user, catalogInfo, itemNumber);
        }
        
        public System.Threading.Tasks.Task AddRecentlyViewedItemAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, string itemNumber) {
            return base.Channel.AddRecentlyViewedItemAsync(user, catalogInfo, itemNumber);
        }
        
        public long CreateList(System.Guid userId, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, KeithLink.Svc.Core.Models.Lists.ListModel list, KeithLink.Svc.Core.Enumerations.List.ListType type) {
            return base.Channel.CreateList(userId, catalogInfo, list, type);
        }
        
        public System.Threading.Tasks.Task<long> CreateListAsync(System.Guid userId, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, KeithLink.Svc.Core.Models.Lists.ListModel list, KeithLink.Svc.Core.Enumerations.List.ListType type) {
            return base.Channel.CreateListAsync(userId, catalogInfo, list, type);
        }
        
        public void DeleteItem(long Id) {
            base.Channel.DeleteItem(Id);
        }
        
        public System.Threading.Tasks.Task DeleteItemAsync(long Id) {
            return base.Channel.DeleteItemAsync(Id);
        }
        
        public void DeleteList(long Id) {
            base.Channel.DeleteList(Id);
        }
        
        public System.Threading.Tasks.Task DeleteListAsync(long Id) {
            return base.Channel.DeleteListAsync(Id);
        }
        
        public void DeleteNote(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, string ItemNumber) {
            base.Channel.DeleteNote(user, catalogInfo, ItemNumber);
        }
        
        public System.Threading.Tasks.Task DeleteNoteAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, string ItemNumber) {
            return base.Channel.DeleteNoteAsync(user, catalogInfo, ItemNumber);
        }
        
        public string[] ReadFavorites(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.ReadFavorites(user, catalogInfo);
        }
        
        public System.Threading.Tasks.Task<string[]> ReadFavoritesAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.ReadFavoritesAsync(user, catalogInfo);
        }
        
        public KeithLink.Svc.Core.Models.Lists.ListModel ReadList(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, long Id) {
            return base.Channel.ReadList(user, catalogInfo, Id);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Lists.ListModel> ReadListAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, long Id) {
            return base.Channel.ReadListAsync(user, catalogInfo, Id);
        }
        
        public KeithLink.Svc.Core.Models.Lists.ListModel[] ReadListByType(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, KeithLink.Svc.Core.Enumerations.List.ListType type) {
            return base.Channel.ReadListByType(user, catalogInfo, type);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Lists.ListModel[]> ReadListByTypeAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, KeithLink.Svc.Core.Enumerations.List.ListType type) {
            return base.Channel.ReadListByTypeAsync(user, catalogInfo, type);
        }
        
        public string[] ReadListLabels(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.ReadListLabels(user, catalogInfo);
        }
        
        public System.Threading.Tasks.Task<string[]> ReadListLabelsAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.ReadListLabelsAsync(user, catalogInfo);
        }
        
        public KeithLink.Svc.Core.Models.Lists.ListItemModel[] ReadNotes(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.ReadNotes(user, catalogInfo);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Lists.ListItemModel[]> ReadNotesAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.ReadNotesAsync(user, catalogInfo);
        }
        
        public KeithLink.Svc.Core.Models.Lists.RecentItem[] ReadRecent(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.ReadRecent(user, catalogInfo);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Lists.RecentItem[]> ReadRecentAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.ReadRecentAsync(user, catalogInfo);
        }
        
        public KeithLink.Svc.Core.Models.Lists.ListModel[] ReadReminders(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.ReadReminders(user, catalogInfo);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Lists.ListModel[]> ReadRemindersAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.ReadRemindersAsync(user, catalogInfo);
        }
        
        public KeithLink.Svc.Core.Models.Lists.ListModel[] ReadUserList(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, bool headerOnly) {
            return base.Channel.ReadUserList(user, catalogInfo, headerOnly);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Lists.ListModel[]> ReadUserListAsync(KeithLink.Svc.Core.Models.Profile.UserProfile user, KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo, bool headerOnly) {
            return base.Channel.ReadUserListAsync(user, catalogInfo, headerOnly);
        }
        
        public void UpdateItem(KeithLink.Svc.Core.Models.Lists.ListItemModel item) {
            base.Channel.UpdateItem(item);
        }
        
        public System.Threading.Tasks.Task UpdateItemAsync(KeithLink.Svc.Core.Models.Lists.ListItemModel item) {
            return base.Channel.UpdateItemAsync(item);
        }
        
        public void UpdateList(KeithLink.Svc.Core.Models.Lists.ListModel userList) {
            base.Channel.UpdateList(userList);
        }
        
        public System.Threading.Tasks.Task UpdateListAsync(KeithLink.Svc.Core.Models.Lists.ListModel userList) {
            return base.Channel.UpdateListAsync(userList);
        }
        
        public void CopyList(KeithLink.Svc.Core.Models.Lists.ListCopyShareModel copyListModel) {
            base.Channel.CopyList(copyListModel);
        }
        
        public System.Threading.Tasks.Task CopyListAsync(KeithLink.Svc.Core.Models.Lists.ListCopyShareModel copyListModel) {
            return base.Channel.CopyListAsync(copyListModel);
        }
        
        public void ShareList(KeithLink.Svc.Core.Models.Lists.ListCopyShareModel shareListModel) {
            base.Channel.ShareList(shareListModel);
        }
        
        public System.Threading.Tasks.Task ShareListAsync(KeithLink.Svc.Core.Models.Lists.ListCopyShareModel shareListModel) {
            return base.Channel.ShareListAsync(shareListModel);
        }
        
        public KeithLink.Svc.Core.Models.Lists.RecommendedItemModel[] ReadRecommendedItemsList(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.ReadRecommendedItemsList(catalogInfo);
        }
        
        public System.Threading.Tasks.Task<KeithLink.Svc.Core.Models.Lists.RecommendedItemModel[]> ReadRecommendedItemsListAsync(KeithLink.Svc.Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
            return base.Channel.ReadRecommendedItemsListAsync(catalogInfo);
        }
    }
}
