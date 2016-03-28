using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Logging;

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

namespace KeithLink.Svc.WebApi.Controllers {
    /// <summary>
    /// User Lists
    /// </summary>
    [Authorize]
    public class ListController : BaseController {
        #region attributes
        private readonly IListLogic _listLogic;
        private readonly IExportSettingLogic _exportLogic;
        private readonly IEventLogRepository _elRepo;
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="profileLogic"></param>
        /// <param name="listServiceRepository"></param>
        /// <param name="exportSettingsLogic"></param>
        /// <param name="elRepo"></param>
        public ListController(IUserProfileLogic profileLogic, IListLogic listLogic, IExportSettingLogic exportSettingsLogic,
                              IEventLogRepository elRepo)
            : base(profileLogic) {
            _listLogic = listLogic;
            _exportLogic = exportSettingsLogic;
            _elRepo = elRepo;
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
        [ApiKeyedRoute("list/export/{listId}")]
        public HttpResponseMessage ExportList(long listId, ExportRequestModel exportRequest) {
            HttpResponseMessage ret;
            try
            {
                var list = _listLogic.ReadList(this.AuthenticatedUser, this.SelectedUserContext, listId);

                if (exportRequest.Fields != null)
                    _exportLogic.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.List, list.Type,
                                                                   exportRequest.Fields, exportRequest.SelectedType);
                ret = ExportModel<ListItemModel>(list.Items, exportRequest);
            }
            catch (Exception ex)
            {
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
                ret.ReasonPhrase = ex.Message;
                _elRepo.WriteErrorLog("List Export", ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve export options for specific list
        /// </summary>
        /// <param name="listId">List Id</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("list/export/{listId}")]
        public OperationReturnModel<ExportOptionsModel> ExportList(long listId) {
            OperationReturnModel<ExportOptionsModel> ret = new OperationReturnModel<ExportOptionsModel>();
            try
            {
                ret.SuccessResponse = _exportLogic.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.List, listId);
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
                ret.SuccessResponse = _listLogic.ReadRecommendedItemsList(this.SelectedUserContext);
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
                ret.SuccessResponse = _listLogic.ReadUserList(this.AuthenticatedUser, this.SelectedUserContext, header);
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
                ret.SuccessResponse = _listLogic.ReadListByType(this.AuthenticatedUser, this.SelectedUserContext, type, headerOnly);
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
        [ApiKeyedRoute("list/{listId}")]
        public OperationReturnModel<ListModel> List(long listId, bool includePrice = true) {
            OperationReturnModel<ListModel> ret = new OperationReturnModel<ListModel>();
            try
            {
                var list = _listLogic.ReadList(this.AuthenticatedUser, this.SelectedUserContext, listId, includePrice);

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
                var list = _listLogic.ReadListLabels(this.AuthenticatedUser, this.SelectedUserContext);
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
                var list = _listLogic.ReadReminders(this.AuthenticatedUser, this.SelectedUserContext);
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
        public OperationReturnModel<NewListItem> List(ListModel list, [FromUri] ListType type = ListType.Custom) {
            OperationReturnModel<NewListItem> ret = new OperationReturnModel<NewListItem>();
            try
            {
                var nlist = new NewListItem() { Id = _listLogic.CreateList(this.AuthenticatedUser.UserId, this.SelectedUserContext, list, type) };
                ret.SuccessResponse = nlist;
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
        /// <param name="listId">List Id</param>
        /// <param name="newItem">New item</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("list/{listId}/item")]
        public OperationReturnModel<NewListItem> AddItem(long listId, ListItemModel newItem) {
            OperationReturnModel<NewListItem> ret = new OperationReturnModel<NewListItem>();
            try
            {
                var nlist = new NewListItem() { Id = _listLogic.AddItem(listId, newItem) };
                ret.SuccessResponse = nlist;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
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
        [ApiKeyedRoute("list/{listId}/items")]
        public OperationReturnModel<ListModel> AddItems(long listId, List<ListItemModel> newItems, bool allowDuplicates = false) {
            OperationReturnModel<ListModel> ret = new OperationReturnModel<ListModel>();
            try
            {
                var list = _listLogic.AddItems(this.AuthenticatedUser, this.SelectedUserContext, listId, newItems);
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
        }

        /// <summary>
        /// Copy list
        /// </summary>
        /// <param name="copyListModel">Copy options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("list/copy")]
        public OperationReturnModel<List<ListCopyResultModel>> CopyList(ListCopyShareModel copyListModel) {
            OperationReturnModel<List<ListCopyResultModel>> ret = new OperationReturnModel<List<ListCopyResultModel>>();
            try
            {
                var list = _listLogic.CopyList(copyListModel);

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
                _listLogic.ShareList(copyListModel);
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

        /// <summary>
        /// Retrieve paged list details for specific list
        /// </summary>
        /// <param name="listId">List Id</param>
        /// <param name="paging">Paging options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("list/{listId}")]
        public OperationReturnModel<PagedListModel> pagedList(long listId, PagingModel paging) {
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
                        Filters = new List<FilterInfo>() { new FilterInfo() { Condition = "||", Field = "Label", Value = paging.Terms, FilterType = "contains" }, new FilterInfo() { Condition = "||", Field = "Name", Value = paging.Terms, FilterType = "contains" } }
                    };
                }
                //var stopWatch = new System.Diagnostics.Stopwatch(); //Temp: Remove
                //stopWatch.Start();
                var list = _listLogic.ReadPagedList(this.AuthenticatedUser, this.SelectedUserContext, listId, paging);
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
        /// Update list item
        /// </summary>
        /// <param name="updatedItem">Updated item</param>
        [HttpPut]
        [ApiKeyedRoute("list/item")]
        public OperationReturnModel<string> UpdateItem(ListItemModel updatedItem) {
            OperationReturnModel<string> ret = new OperationReturnModel<string>();
            try
            {
                _listLogic.UpdateItem(updatedItem);
                ret.SuccessResponse = null;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("UpdateItem", ex);
            }
            return ret;
        }

        /// <summary>
        /// Update list
        /// </summary>
        /// <param name="updatedList">Updated list</param>
        [HttpPut]
        [ApiKeyedRoute("list/")]
        public OperationReturnModel<string> Put(ListModel updatedList) {
            OperationReturnModel<string> ret = new OperationReturnModel<string>();
            try
            {
                _listLogic.UpdateList(updatedList);
                ret.SuccessResponse = null;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
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
        [ApiKeyedRoute("list/{listId}")]
        public OperationReturnModel<string> DeleteList(long listId) {
            OperationReturnModel<string> ret = new OperationReturnModel<string>();
            try
            {
                _listLogic.DeleteList(listId);
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
        /// Delete multiple lists
        /// </summary>
        /// <param name="listIds">Array of list ids to delete</param>
        [HttpDelete]
        [ApiKeyedRoute("list/")]
        public OperationReturnModel<string> DeleteList(List<long> listIds) {
            OperationReturnModel<string> ret = new OperationReturnModel<string>();
            try
            {
                _listLogic.DeleteLists(listIds);
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
        /// Delete list item
        /// </summary>
        /// <param name="itemId">Item id to delete</param>
        [HttpDelete]
        [ApiKeyedRoute("list/item/{itemId}")]
        public OperationReturnModel<string> DeleteItem(long itemId) {
            OperationReturnModel<string> ret = new OperationReturnModel<string>();
            try
            {
                _listLogic.DeleteItem(itemId);
                ret.SuccessResponse = null;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("DeleteItem", ex);
            }
            return ret;
        }

        /// <summary>
        /// Delete multiple list items
        /// </summary>
        /// <param name="itemIds">Array of item ids to delete</param>
        [HttpDelete]
        [ApiKeyedRoute("list/item")]
        public OperationReturnModel<string> DeleteItem(List<long> itemIds) {
            OperationReturnModel<string> ret = new OperationReturnModel<string>();
            try
            {
                _listLogic.DeleteItems(itemIds);
                ret.SuccessResponse = null;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("DeleteItem", ex);
            }
            return ret;
        }

        /// <summary>
        /// Delete itemnumbers from a specific list
        /// </summary>
        /// <param name="Id">List Id</param>
        /// <param name="itemNumber">Itemnumber to delete</param>
        /// <returns></returns>
        [HttpDelete]
        [ApiKeyedRoute("list/{Id}/item/{itemNumber}")]
        public OperationReturnModel<bool> DeleteItemNumberFromList(long Id, string itemNumber) {
            OperationReturnModel<bool> ret = new OperationReturnModel<bool>();
            try
            {
                _listLogic.DeleteItemNumberFromList(Id, itemNumber);
                ret = new OperationReturnModel<bool>() { SuccessResponse = true, IsSuccess = true };
            }
            catch (Exception ex)
            {
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
        [ApiKeyedRoute("list/barcode/{listId}")]
        public HttpResponseMessage Barcode(long listId) {
            HttpResponseMessage ret;
            try
            {
                var list = _listLogic.GetBarcodeForList(this.AuthenticatedUser, this.SelectedUserContext, listId);

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
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
                ret.ReasonPhrase = ex.Message;
                _elRepo.WriteErrorLog("Barcode", ex);
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
        [ApiKeyedRoute("list/print/{listId}")]
        public HttpResponseMessage Print(long listId, PrintListModel options) {
            HttpResponseMessage ret;
            try
            {
                Stream stream = _listLogic.BuildReportFromList(options, listId, this.SelectedUserContext, this.AuthenticatedUser);

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
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
                ret.ReasonPhrase = ex.Message;
                _elRepo.WriteErrorLog("Barcode", ex);
            }
            return ret;
        }
        #endregion
    }
}
