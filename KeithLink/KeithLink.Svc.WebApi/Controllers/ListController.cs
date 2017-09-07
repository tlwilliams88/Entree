using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Enumerations.List;

using KeithLink.Svc.Core.Extensions;

using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.Impl.Helpers;
using KeithLink.Svc.WebApi.Helpers;
using KeithLink.Svc.WebApi.Models;

using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

using KeithLink.Svc.Core.Models.Lists.CustomListShares;
using KeithLink.Svc.Core.Interface.Cache;

namespace KeithLink.Svc.WebApi.Controllers {
    /// <summary>
    /// User Lists
    /// </summary>
    [Authorize]
    public class ListController : BaseController {
        #region attributes
        private readonly ICacheListLogic _cacheListLogic;
        private readonly IAuditLogRepository _auditLogRepo;
        private readonly IListLogic _listLogic;
        private readonly IListService _listService;
        private readonly ICustomListSharesRepository _customListSharesRepo;
        private readonly IExportSettingLogic _exportLogic;
        private readonly IEventLogRepository _elRepo;
        private readonly IUserProfileLogic _profileLogic;
        private readonly ICatalogLogic _catalogLogic;
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="profileLogic"></param>
        /// <param name="listLogic"></param>
        /// <param name="exportSettingsLogic"></param>
        /// <param name="elRepo"></param>
        /// <param name="auditLogRepo"></param>
        /// <param name="listRepo"></param>
        public ListController(IUserProfileLogic profileLogic, IListLogic listLogic, IExportSettingLogic exportSettingsLogic, ICacheListLogic cacheListLogic, ICatalogLogic catalogLogic,
                              IEventLogRepository elRepo, IAuditLogRepository auditLogRepo, ICustomListSharesRepository customListSharesRepo, IListService listService)
            : base(profileLogic) {
            _auditLogRepo = auditLogRepo;
            _listLogic = listLogic;
            _cacheListLogic = cacheListLogic;
            _profileLogic = profileLogic;
            _exportLogic = exportSettingsLogic;
            _elRepo = elRepo;
            _listService = listService;
            _customListSharesRepo = customListSharesRepo;
            _catalogLogic = catalogLogic;
        }
        #endregion

        #region methods
        /// <summary>
        /// Export list to CSV, TAB, or Excel
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="exportRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("list/export/{type}/{listId}")]
        public HttpResponseMessage ExportList(ListType type, long listId, [FromBody]ExportRequestModel exportRequest) {
            HttpResponseMessage ret;
            try
            {
                var list = _listService.ReadList(this.AuthenticatedUser, this.SelectedUserContext, type, listId, true);
                ItemOrderHistoryHelper.GetItemOrderHistories(_catalogLogic, SelectedUserContext, list.Items);

                if (exportRequest.Sort != null) {
                    List<SortInfo> slist = new List<SortInfo>();
                    slist.Add(exportRequest.Sort);
                    list.Items = list.Items.AsQueryable()
                                     .Sort(slist)
                                     .ToList();
                }

                if (exportRequest.Fields != null)
                    _exportLogic.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.List, list.Type,
                                                                   exportRequest.Fields, exportRequest.SelectedType);
                ret = ExportModel<ListItemModel>(list.Items, exportRequest, SelectedUserContext);
            }
            catch (Exception ex)
            {
                _elRepo.WriteErrorLog("List Export", ex);
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve export options for specific list
        /// </summary>
        /// <param name="listId">List Id</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("list/export/{type}/{listId}")]
        public OperationReturnModel<ExportOptionsModel> ExportList(ListType type) {
            OperationReturnModel<ExportOptionsModel> ret = new OperationReturnModel<ExportOptionsModel>();
            try
            {
                ret.SuccessResponse = _exportLogic.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.List, type);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("Recommended List", ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve recommended items list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("list/recommended")]
        public OperationReturnModel<List<RecommendedItemModel>> ReadRecommendedItemsList() {
            OperationReturnModel<List<RecommendedItemModel>> ret = new OperationReturnModel<List<RecommendedItemModel>>();
            try
            {
                ret.SuccessResponse = _listService.ReadRecommendedItemsList(this.SelectedUserContext);
                ret.IsSuccess = true;
            }catch(Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("Recommended List", ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve all list for the authenticated user
        /// </summary>
        /// <param name="header">Headonly only or details?</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("list/")]
        public OperationReturnModel<List<ListModel>> List(bool header = false) {
            OperationReturnModel<List<ListModel>> ret = new OperationReturnModel<List<ListModel>>();
            try
            {
                ret.SuccessResponse = _listService.ReadUserList(this.AuthenticatedUser, this.SelectedUserContext, header);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("List", ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve list by type for the authenticated user
        /// </summary>
        /// <param name="type">List type</param>
        /// <param name="headerOnly">Header only or details?</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("list/type/{type}")]
        public OperationReturnModel<List<ListModel>> List(ListType type, bool headerOnly = false) {
            OperationReturnModel<List<ListModel>> ret = new OperationReturnModel<List<ListModel>>();
            try
            {
                ret.SuccessResponse = _listService.ReadListByType(this.AuthenticatedUser, this.SelectedUserContext, type, headerOnly);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("List", ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve a specific list
        /// </summary>
        /// <param name="listId">Lsit id</param>
        /// <param name="includePrice">Include item prices?</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("list/{type}/{listId}")]
        public OperationReturnModel<ListModel> List(ListType type, long listId, bool includePrice = true) {
            OperationReturnModel<ListModel> ret = new OperationReturnModel<ListModel>();
            try
            {
                var list = _listService.ReadList(this.AuthenticatedUser, this.SelectedUserContext, type, listId, includePrice);

                if (list != null)
                    list.ReadOnly = (!this.AuthenticatedUser.IsInternalUser && list.Type == ListType.RecommendedItems) ||
                        (!this.AuthenticatedUser.IsInternalUser && list.Type == ListType.Mandatory);

                ret.SuccessResponse = list;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("List", ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve all list labels for the authenticated user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("list/labels")]
        public OperationReturnModel<List<string>> ListLabels() {
            OperationReturnModel<List<string>> ret = new OperationReturnModel<List<string>>();
            try
            {
                var list = _listService.ReadLabels(this.AuthenticatedUser, this.SelectedUserContext);
                ret.SuccessResponse = list;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("List", ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve reminders list for the authenticated user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("list/reminders")]
        public OperationReturnModel<List<ListModel>> ListReminders() {
            OperationReturnModel<List<ListModel>> ret = new OperationReturnModel<List<ListModel>>();
            try
            {
                var list = _listService.ReadListByType(this.AuthenticatedUser, this.SelectedUserContext, ListType.Reminder, false);
                list.AddRange(_listService.ReadListByType(this.AuthenticatedUser, this.SelectedUserContext, ListType.Mandatory, false));
                ret.SuccessResponse = list;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("List", ex);
            }
            return ret;
        }

        /// <summary>
        /// Create a new list for the authenticated user
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="type">list type</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("list/")]
        public OperationReturnModel<ListModel> List(ListModel list, [FromUri] ListType type = ListType.Custom) {
            OperationReturnModel<ListModel> ret = new OperationReturnModel<ListModel>();
            try
            {
                ret.SuccessResponse = _listService.CreateList(this.AuthenticatedUser, this.SelectedUserContext, type, list);

                _cacheListLogic.RemoveSpecificCachedList(new ListModel()
                {
                    BranchId = SelectedUserContext.BranchId,
                    CustomerNumber = SelectedUserContext.CustomerId,
                    Type = ret.SuccessResponse.Type,
                    ListId = ret.SuccessResponse.ListId
                });

                _cacheListLogic.RemoveTypeOfListsCache(SelectedUserContext, type);

                _cacheListLogic.ClearCustomersListCaches(this.AuthenticatedUser, this.SelectedUserContext, _listService.ReadUserList(this.AuthenticatedUser, this.SelectedUserContext, true));

                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("List", ex);
            }
            return ret;
        }

        /// <summary>
        /// Add item to a specific list
        /// </summary>
        /// <param name="type">list type</param>
        /// <param name="listId">List Id</param>
        /// <param name="newItem">New item</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("list/{type}/{listId}/item")]
        public OperationReturnModel<bool> AddItem(ListType type, long listId, ListItemModel newItem) {
            OperationReturnModel<bool> ret = new OperationReturnModel<bool>();

            try {
                _listService.SaveItem(this.AuthenticatedUser, this.SelectedUserContext, type, 
                                      listId, newItem);

                var list = new ListModel()
                {
                    BranchId = SelectedUserContext.BranchId,
                    CustomerNumber = SelectedUserContext.CustomerId,
                    Type = type,
                    ListId = listId
                };

                _cacheListLogic.RemoveSpecificCachedList(list);

                _cacheListLogic.ClearCustomersListCaches(this.AuthenticatedUser, this.SelectedUserContext, _listService.ReadUserList(this.AuthenticatedUser, this.SelectedUserContext, true));

                ret.SuccessResponse = true;
                ret.IsSuccess = true;
            } catch (Exception ex) {
                ret.SuccessResponse = false;
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;

                _elRepo.WriteErrorLog("AddItem", ex);
            }

            return ret;
        }

        /// <summary>
        /// Add multiple items to a specific list
        /// </summary>
        /// <param name="listId">List Id</param>
        /// <param name="newItems">Array of new items</param>
        /// <param name="allowDuplicates">Allow duplicate item numbers?</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("list/{type}/{listId}/items")]
        public OperationReturnModel<ListModel> AddItems(ListType type, long listId, List<ListItemModel> newItems, bool allowDuplicates = false)
        {
            OperationReturnModel<ListModel> ret = new OperationReturnModel<ListModel>();
            try
            {
                _listService.SaveItems(this.AuthenticatedUser, this.SelectedUserContext, type, listId, newItems);

                var list = new ListModel() {
                                               BranchId = SelectedUserContext.BranchId,
                                               CustomerNumber = SelectedUserContext.CustomerId,
                                               Type = type,
                                               ListId = listId
                                           }; 

                _cacheListLogic.RemoveSpecificCachedList(list);

                _cacheListLogic.ClearCustomersListCaches(this.AuthenticatedUser, this.SelectedUserContext, _listService.ReadUserList(this.AuthenticatedUser, this.SelectedUserContext, true));

                ret.SuccessResponse = list;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("AddItems", ex);
            }
            return ret;
        }        /// <summary>
                 /// Add a custom inventory item to a list
                 /// </summary>
                 /// <param name="listId"></param>
                 /// <param name="customInventoryId"></param>
                 /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("list/{type}/{listId}/custominventoryitem/{customInventoryId}")]
        public OperationReturnModel<NewListItem> AddCustomInventoryItem(ListType type, long listId, long customInventoryId) {
            OperationReturnModel<NewListItem> returnValue = new OperationReturnModel<NewListItem>();

            try {
                NewListItem listItem = new NewListItem() { Id = _listService.AddCustomInventory(this.AuthenticatedUser, this.SelectedUserContext, type, listId, customInventoryId) };

                _cacheListLogic.ClearCustomersListCaches(this.AuthenticatedUser, this.SelectedUserContext, _listService.ReadUserList(this.AuthenticatedUser, this.SelectedUserContext, true));

                returnValue.SuccessResponse = listItem;
                returnValue.IsSuccess = true;
            } catch (Exception ex) {
                returnValue.IsSuccess = false;
                returnValue.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("Error adding custom inventory to list", ex);
            }

            return returnValue;
        }

        /// <summary>
        /// Add a list of custom inventory item to a list
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="customInventoryIds"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("list/{type}/{listId}/custominventoryitem")]
        public OperationReturnModel<bool> AddCustomInventoryItems(ListType type, long listId, List<long> customInventoryIds)
        {
            OperationReturnModel<bool> returnValue = new OperationReturnModel<bool>();

            try
            {
                List<long?> newListItems = _listService.AddCustomInventoryItems(this.AuthenticatedUser, this.SelectedUserContext, type, listId, customInventoryIds);

                var list = new ListModel()
                {
                    BranchId = SelectedUserContext.BranchId,
                    CustomerNumber = SelectedUserContext.CustomerId,
                    Type = type,
                    ListId = listId
                };

                _cacheListLogic.RemoveSpecificCachedList(list);

                _cacheListLogic.ClearCustomersListCaches(this.AuthenticatedUser, this.SelectedUserContext, _listService.ReadUserList(this.AuthenticatedUser, this.SelectedUserContext, true));

                returnValue.SuccessResponse = true;
                returnValue.IsSuccess = true;
            }
            catch (Exception ex)
            {
                returnValue.IsSuccess = false;
                returnValue.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("Error adding custom inventory to list", ex);
            }

            return returnValue;
        }



        /// <summary>
        /// Copy list
        /// </summary>
        /// <param name="copyListModel">Copy options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("list/copy")]
        public OperationReturnModel<List<ListModel>> CopyList(ListCopyShareModel copyListModel) {
            OperationReturnModel<List<ListModel>> ret = new OperationReturnModel<List<ListModel>>();
            try
            {
                var list = _listService.CopyList(this.AuthenticatedUser, this.SelectedUserContext, copyListModel);

                _cacheListLogic.ClearCustomersListCaches(this.AuthenticatedUser, this.SelectedUserContext, _listService.ReadUserList(this.AuthenticatedUser, this.SelectedUserContext, true));

                ret.SuccessResponse = list;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("CopyList", ex);
            }
            return ret;
        }

        /// <summary>
        /// Share list
        /// </summary>
        /// <param name="copyListModel">Share options</param>
        [HttpPost]
        [ApiKeyedRoute("list/share")]
        public OperationReturnModel<string> ShareList(ListCopyShareModel copyListModel) {
            OperationReturnModel<string> ret = new OperationReturnModel<string>();
            try
            {
                foreach (var customer in copyListModel.Customers) {
                    _customListSharesRepo.SaveCustomListShare(new CustomListShare()
                    {
                        Active = true,
                        HeaderId = copyListModel.ListId,
                        CustomerNumber = customer.CustomerNumber,
                        BranchId = customer.CustomerBranch
                    });

                    _cacheListLogic.ClearCustomersLabelsCache(new UserSelectedContext() {
                                                                                            CustomerId = customer.CustomerNumber,
                                                                                            BranchId = customer.CustomerBranch
                                                                                        });
                }
                ret.SuccessResponse = null;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("ShareList", ex);
            }
            return ret;
        }

        [HttpPost]
        [ApiKeyedRoute("list/{type}/{listId}")]
        public OperationReturnModel<PagedListModel> pagedList(long listId, ListType type, PagingModel paging) {
            OperationReturnModel<PagedListModel> ret = new OperationReturnModel<PagedListModel>();
            try
            {
                if (!string.IsNullOrEmpty(paging.Terms))
                {
                    //Build filter
                    paging.Filter = new FilterInfo()
                    {
                        Field = "ItemNumber",
                        FilterType = "contains",
                        Value = paging.Terms,
                        Condition = "||",
                        Filters = new List<FilterInfo>() { new FilterInfo() { Condition = "||", Field = "Label", Value = paging.Terms, FilterType = "contains" },
                                                           new FilterInfo() { Condition = "||", Field = "Name", Value = paging.Terms, FilterType = "contains" } }
                    };
                    if(paging.Terms.IndexOf(' ') > -1)
                    {
                        string[] words = paging.Terms.Split(' ');
                        paging.Filter = new FilterInfo()
                        {
                            Field = "ItemNumber",
                            FilterType = "contains",
                            Value = paging.Terms,
                            Condition = "||",
                            Filters = new List<FilterInfo>() { new FilterInfo() { Condition = "&&", Field = "Label", Value = words[0], FilterType = "contains" },
                                                           new FilterInfo() { Condition = "&&", Field = "Name", Value = words[0], FilterType = "contains" } }
                        };
                        foreach (string word in words)
                        {
                            paging.Filter.Filters[0].Filters = new List<FilterInfo>();
                            paging.Filter.Filters[0].Filters.Add
                                (new FilterInfo()
                                {
                                    Condition = "&&",
                                    Field = "Label",
                                    Value = word,
                                    FilterType = "contains" });
                            paging.Filter.Filters[1].Filters = new List<FilterInfo>();
                            paging.Filter.Filters[1].Filters.Add
                                (new FilterInfo()
                                {
                                    Condition = "&&",
                                    Field = "Name",
                                    Value = word,
                                    FilterType = "contains"
                                });
                        }
                    }
                }
                //var stopWatch = new System.Diagnostics.Stopwatch(); //Temp: Remove
                //stopWatch.Start();
                var list = _listService.ReadPagedList(this.AuthenticatedUser, this.SelectedUserContext, type, listId, paging);
                //stopWatch.Stop();
                //elRepo.WriteInformationLog(string.Format("Total time to retrieve List {0}: {1}ms", listId, stopWatch.ElapsedMilliseconds));

                ret.SuccessResponse = list;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("pagedList", ex);
            }
            return ret;
        }

        /// <summary>
        /// Update list
        /// </summary>
        /// <param name="updatedList">Updated list</param>
        [HttpPut]
        [ApiKeyedRoute("list/")]
        public OperationReturnModel<ListModel> Put(ListModel updatedList) {
            OperationReturnModel<ListModel> ret = new OperationReturnModel<ListModel>();
            
            try {
                ret.SuccessResponse = _listService.UpdateList(this.AuthenticatedUser, this.SelectedUserContext, updatedList.Type, updatedList);

                _cacheListLogic.RemoveSpecificCachedList(new ListModel()
                {
                    BranchId = SelectedUserContext.BranchId,
                    CustomerNumber = SelectedUserContext.CustomerId,
                    Type = updatedList.Type,
                    ListId = updatedList.ListId
                });

                _cacheListLogic.ClearCustomersListCaches(this.AuthenticatedUser, this.SelectedUserContext, _listService.ReadUserList(this.AuthenticatedUser, this.SelectedUserContext, true));

                ret.IsSuccess = true;
            } catch (Exception ex) {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("PutList", ex);
            }

            return ret;
        }

        /// <summary>
        /// Delete list
        /// </summary>
        /// <param name="listId">List Id to delete</param>
        [HttpDelete]
        [ApiKeyedRoute("list/{type}/{listId}")]
        public OperationReturnModel<string> DeleteList(ListType type, long listId) {
            OperationReturnModel<string> ret = new OperationReturnModel<string>();
            try
            {
                var list = _listService.ReadList(AuthenticatedUser, SelectedUserContext, type, listId);

                _listService.DeleteList(AuthenticatedUser, SelectedUserContext, type, list);

                _auditLogRepo.WriteToAuditLog(Common.Core.Enumerations.AuditType.ListDelete, 
                    AuthenticatedUser.Name, 
                    String.Format("List {0} ({1}) deleted for customer {2} - {3}", 
                                  list.Name, 
                                  listId, 
                                  this.SelectedUserContext.CustomerId, 
                                  this.SelectedUserContext.BranchId));

                _cacheListLogic.RemoveSpecificCachedList(list);

                _cacheListLogic.ClearCustomersListCaches(this.AuthenticatedUser, this.SelectedUserContext, _listService.ReadUserList(this.AuthenticatedUser, this.SelectedUserContext, true));

                ret.SuccessResponse = null;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("DeleteList", ex);
            }
            return ret;
        }

        /// <summary>
        /// Delete multiple list items
        /// </summary>
        /// <param name="type">list type</param>
        /// <param name="listId">the header id</param>
        /// <param name="itemNumbers">list of item numbers</param>
        [HttpDelete]
        [ApiKeyedRoute("list/{type}/{listId}/item")]
        public OperationReturnModel<string> DeleteItem(ListType type, long listId, List<string> itemNumbers) {
            OperationReturnModel<string> ret = new OperationReturnModel<string>();
            try {
                _listService.DeleteItems(AuthenticatedUser, SelectedUserContext, type, 
                                         listId, itemNumbers);

                var list = new ListModel()
                {
                    BranchId = SelectedUserContext.BranchId,
                    CustomerNumber = SelectedUserContext.CustomerId,
                    Type = type,
                    ListId = listId
                };

                _cacheListLogic.RemoveSpecificCachedList(list);

                _cacheListLogic.ClearCustomersListCaches(this.AuthenticatedUser, this.SelectedUserContext, _listService.ReadUserList(this.AuthenticatedUser, this.SelectedUserContext, true));

                ret.SuccessResponse = null;
                ret.IsSuccess = true;
            } catch(Exception ex) {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("DeleteItem", ex);
            }
            return ret;
        }

        /// <summary>
        /// Delete itemnumbers from a specific list
        /// </summary>
        /// <param name="type">list type</param>
        /// <param name="Id">List Id</param>
        /// <param name="itemNumber">Itemnumber to delete</param>
        /// <returns></returns>
        [HttpDelete]
        [ApiKeyedRoute("list/{type}/{Id}/item/{itemNumber}")]
        public OperationReturnModel<bool> DeleteItemNumberFromList(ListType type, long Id, string itemNumber) {
            OperationReturnModel<bool> ret = new OperationReturnModel<bool>();
            try {
                _listService.DeleteItem(AuthenticatedUser, SelectedUserContext, type, 
                                        Id, itemNumber);

                var list = new ListModel()
                {
                    BranchId = SelectedUserContext.BranchId,
                    CustomerNumber = SelectedUserContext.CustomerId,
                    Type = type,
                    ListId = Id
                };

                _cacheListLogic.RemoveSpecificCachedList(list);

                _cacheListLogic.ClearCustomersListCaches(this.AuthenticatedUser, this.SelectedUserContext, _listService.ReadUserList(this.AuthenticatedUser, this.SelectedUserContext, true));

                ret = new OperationReturnModel<bool>() { SuccessResponse = true, IsSuccess = true };
            } catch(Exception ex) {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("DeleteItemNumberFromList", ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve the list Barcode report (PDF)
        /// </summary>
        /// <param name="listId">List Id</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("list/barcode/{type}/{listId}")]
        public HttpResponseMessage Barcode(ListType type, long listId) {
            HttpResponseMessage ret;
            try
            {
                var list = _listService.GetBarcodeForList(this.AuthenticatedUser, this.SelectedUserContext, type, listId);

                if (list == null)
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.Gone };


                ReportViewer rv = new ReportViewer();

                rv.ProcessingMode = ProcessingMode.Local;
                //rv.LocalReport.ReportPath = "Report1.rdlc";

                Assembly assembly = Assembly.Load("Keithlink.Svc.Impl");
                Stream rdlcStream = assembly.GetManifestResourceStream("KeithLink.Svc.Impl.Reports.Itembarcode5160.rdlc");
                rv.LocalReport.LoadReportDefinition(rdlcStream);
                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", list));

                var bytes = rv.LocalReport.Render("PDF");

                Stream stream = new MemoryStream(bytes);

                HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");


                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                ret = result;
            }
            catch (Exception ex)
            {
                _elRepo.WriteErrorLog("Barcode", ex);
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve the list printing report (PDF)
        /// </summary>
        /// <param name="listId">List Id</param>
        /// <param name="options">Paging options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("list/print/{type}/{listId}")]
        public HttpResponseMessage Print(ListType type, long listId, PrintListModel options) {
            HttpResponseMessage ret;
            try
            {
                Stream stream = ListPrintHelper.BuildReportFromList
                    (options, type, listId, SelectedUserContext, AuthenticatedUser, _listService, _profileLogic, _elRepo);

                if (stream == null)
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.Gone };

                HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

                ret = result;
            }
            catch (Exception ex)
            {
                _elRepo.WriteErrorLog("Print", ex);
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return ret;
        }
        #endregion
    }
}
