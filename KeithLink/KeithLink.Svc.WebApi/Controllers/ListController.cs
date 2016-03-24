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
        private readonly IListServiceRepository _listServiceRepository;
        private readonly IExportSettingLogic _exportLogic;
        private readonly IEventLogRepository _elRepo;
        #endregion

        #region ctor
        public ListController(IUserProfileLogic profileLogic, IListServiceRepository listServiceRepository, IExportSettingLogic exportSettingsLogic,
                              IEventLogRepository elRepo)
            : base(profileLogic) {
            this._listServiceRepository = listServiceRepository;
            this._exportLogic = exportSettingsLogic;
            this._elRepo = elRepo;
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
            var list = _listServiceRepository.ReadList(this.AuthenticatedUser, this.SelectedUserContext, listId);

            if (exportRequest.Fields != null)
                _exportLogic.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.List, list.Type,
                                                               exportRequest.Fields, exportRequest.SelectedType);
            return ExportModel<ListItemModel>(list.Items, exportRequest);
        }

        /// <summary>
        /// Retrieve export options for specific list
        /// </summary>
        /// <param name="listId">List Id</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("list/export/{listId}")]
        public ExportOptionsModel ExportList(long listId) {
            return _exportLogic.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.List, listId);
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
                ret.SuccessResponse = _listServiceRepository.ReadRecommendedItemsList(this.SelectedUserContext);
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
        public List<ListModel> List(bool header = false) {
            return _listServiceRepository.ReadUserList(this.AuthenticatedUser, this.SelectedUserContext, header);
        }

        /// <summary>
        /// Retrieve list by type for the authenticated user
        /// </summary>
        /// <param name="type">List type</param>
        /// <param name="headerOnly">Header only or details?</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("list/type/{type}")]
        public List<ListModel> List(ListType type, bool headerOnly = false) {
            return _listServiceRepository.ReadListByType(this.AuthenticatedUser, this.SelectedUserContext, type, headerOnly);
        }

        /// <summary>
        /// Retrieve a specific list
        /// </summary>
        /// <param name="listId">Lsit id</param>
        /// <param name="includePrice">Include item prices?</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("list/{listId}")]
        public ListModel List(long listId, bool includePrice = true) {
            var list = _listServiceRepository.ReadList(this.AuthenticatedUser, this.SelectedUserContext, listId, includePrice);

            if (list != null)
                list.ReadOnly = (!this.AuthenticatedUser.IsInternalUser && list.Type == ListType.RecommendedItems) ||
                    (!this.AuthenticatedUser.IsInternalUser && list.Type == ListType.Mandatory);

            return list;

        }

        /// <summary>
        /// Retrieve all list labels for the authenticated user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("list/labels")]
        public List<string> ListLabels() {
            return _listServiceRepository.ReadListLabels(this.AuthenticatedUser, this.SelectedUserContext);
        }

        /// <summary>
        /// Retrieve reminders list for the authenticated user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("list/reminders")]
        public List<ListModel> ListReminders() {
            return _listServiceRepository.ReadReminders(this.AuthenticatedUser, this.SelectedUserContext);
        }

        /// <summary>
        /// Create a new list for the authenticated user
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="isMandatory">Is mandatory list?</param>
        /// <param name="isRecommended">Is recommended list?</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("list/")]
        public NewListItem List(ListModel list, [FromUri] ListType type = ListType.Custom) {
            return new NewListItem() { Id = _listServiceRepository.CreateList(this.AuthenticatedUser.UserId, this.SelectedUserContext, list, type) };
        }


        /// <summary>
        /// Add item to a specific list
        /// </summary>
        /// <param name="listId">List Id</param>
        /// <param name="newItem">New item</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("list/{listId}/item")]
        public NewListItem AddItem(long listId, ListItemModel newItem) {
            return new NewListItem() { Id = _listServiceRepository.AddItem(listId, newItem) };
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
        public ListModel AddItems(long listId, List<ListItemModel> newItems, bool allowDuplicates = false) {
            return _listServiceRepository.AddItems(this.AuthenticatedUser, this.SelectedUserContext, listId, newItems);
        }

        /// <summary>
        /// Copy list
        /// </summary>
        /// <param name="copyListModel">Copy options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("list/copy")]
        public List<ListCopyResultModel> CopyList(ListCopyShareModel copyListModel) {
            return _listServiceRepository.CopyList(copyListModel);
        }

        /// <summary>
        /// Share list
        /// </summary>
        /// <param name="copyListModel">Share options</param>
        [HttpPost]
        [ApiKeyedRoute("list/share")]
        public void ShareList(ListCopyShareModel copyListModel) {
            _listServiceRepository.ShareList(copyListModel);
        }

        /// <summary>
        /// Retrieve paged list details for specific list
        /// </summary>
        /// <param name="listId">List Id</param>
        /// <param name="paging">Paging options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("list/{listId}")]
        public PagedListModel pagedList(long listId, PagingModel paging) {
            if (!string.IsNullOrEmpty(paging.Terms)) {
                //Build filter
                paging.Filter = new FilterInfo() {
                    Field = "ItemNumber",
                    FilterType = "contains",
                    Value = paging.Terms,
                    Condition = "||",
                    Filters = new List<FilterInfo>() { new FilterInfo() { Condition = "||", Field = "Label", Value = paging.Terms, FilterType = "contains" }, new FilterInfo() { Condition = "||", Field = "Name", Value = paging.Terms, FilterType = "contains" } }
                };
            }
            //var stopWatch = new System.Diagnostics.Stopwatch(); //Temp: Remove
            //stopWatch.Start();
            var list = _listServiceRepository.ReadPagedList(this.AuthenticatedUser, this.SelectedUserContext, listId, paging);
            //stopWatch.Stop();
            //elRepo.WriteInformationLog(string.Format("Total time to retrieve List {0}: {1}ms", listId, stopWatch.ElapsedMilliseconds));

            return list;
        }

        /// <summary>
        /// Update list item
        /// </summary>
        /// <param name="updatedItem">Updated item</param>
        [HttpPut]
        [ApiKeyedRoute("list/item")]
        public void UpdateItem(ListItemModel updatedItem) {
            _listServiceRepository.UpdateItem(updatedItem);
        }

        /// <summary>
        /// Update list
        /// </summary>
        /// <param name="updatedList">Updated list</param>
        [HttpPut]
        [ApiKeyedRoute("list/")]
        public void Put(ListModel updatedList) {
            _listServiceRepository.UpdateList(updatedList);
        }

        /// <summary>
        /// Delete list
        /// </summary>
        /// <param name="listId">List Id to delete</param>
        [HttpDelete]
        [ApiKeyedRoute("list/{listId}")]
        public void DeleteList(long listId) {
            _listServiceRepository.DeleteList(listId);
        }

        /// <summary>
        /// Delete multiple lists
        /// </summary>
        /// <param name="listIds">Array of list ids to delete</param>
        [HttpDelete]
        [ApiKeyedRoute("list/")]
        public void DeleteList(List<long> listIds) {
            _listServiceRepository.DeleteLists(listIds);
        }

        /// <summary>
        /// Delete list item
        /// </summary>
        /// <param name="itemId">Item id to delete</param>
        [HttpDelete]
        [ApiKeyedRoute("list/item/{itemId}")]
        public void DeleteItem(long itemId) {
            _listServiceRepository.DeleteItem(itemId);
        }

        /// <summary>
        /// Delete multiple list items
        /// </summary>
        /// <param name="itemIds">Array of item ids to delete</param>
        [HttpDelete]
        [ApiKeyedRoute("list/item")]
        public void DeleteItem(List<long> itemIds) {
            _listServiceRepository.DeleteItems(itemIds);
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
            _listServiceRepository.DeleteItemNumberFromList(Id, itemNumber);
            return new OperationReturnModel<bool>() { SuccessResponse = true };
        }

        /// <summary>
        /// Retrieve the list Barcode report (PDF)
        /// </summary>
        /// <param name="listId">List Id</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("list/barcode/{listId}")]
        public HttpResponseMessage Barcode(long listId) {
            try {
                var list = _listServiceRepository.GetBarcodeForList(this.AuthenticatedUser, this.SelectedUserContext, listId);

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

                return result;



            } catch (Exception ex) {
                //TODO: This is test code to determine issue in dev. This should be removed.
                _elRepo.WriteErrorLog("e", ex);
                if (ex.InnerException != null) {
                    _elRepo.WriteErrorLog("b", ex.InnerException);
                    if (ex.InnerException.InnerException != null)
                        _elRepo.WriteErrorLog("c", ex.InnerException.InnerException);
                }
            }
            return null;


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
            try {
                Stream stream = _listServiceRepository.BuildReportFromList(options, listId, this.SelectedUserContext, this.AuthenticatedUser);

                if (stream == null)
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.Gone };

                HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

                return result;
            } catch (Exception ex) {
                //TODO: This is test code to determine issue in dev. This should be removed.
                _elRepo.WriteErrorLog("e", ex);
                if (ex.InnerException != null) {
                    _elRepo.WriteErrorLog("b", ex.InnerException);
                    if (ex.InnerException.InnerException != null)
                        _elRepo.WriteErrorLog("c", ex.InnerException.InnerException);
                }
            }
            return null;
        }
        #endregion
    }
}
