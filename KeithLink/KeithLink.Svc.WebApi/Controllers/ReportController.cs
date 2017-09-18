using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Reports;

using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.Reports;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Impl.Helpers;

namespace KeithLink.Svc.WebApi.Controllers
{
    /// <summary>
    /// controller for reports
    /// </summary>
    [Authorize]
    public class ReportController : BaseController {
        #region attributes
        private readonly IReportLogic _reportLogic;
        private readonly IListService _listService;
        private readonly IExportSettingLogic _exportLogic;
		private readonly IInventoryValuationReportLogic _inventoryValuationReportLogic;
        private readonly ICatalogLogic _catalogLogic;
        private readonly IEventLogRepository _log;

        #endregion

        #region ctor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="reportLogic"></param>
        /// <param name="profileLogic"></param>
        /// <param name="exportSettingsLogic"></param>
        /// <param name="inventoryValuationReportLogic"></param>
        /// <param name="logRepo"></param>
        public ReportController(IReportLogic reportLogic, IUserProfileLogic profileLogic, IExportSettingLogic exportSettingsLogic, ICatalogLogic catalogLogic,
                                IInventoryValuationReportLogic inventoryValuationReportLogic, IListService listService, IEventLogRepository logRepo) 
            : base(profileLogic) {
			_exportLogic = exportSettingsLogic;
			_inventoryValuationReportLogic = inventoryValuationReportLogic;
            _reportLogic = reportLogic;
            _listService = listService;
            _catalogLogic = catalogLogic;
            _log = logRepo;
        }
        #endregion

        #region methods
        /// <summary>
        /// Retrieve the item usage report
        /// </summary>
        /// <param name="usageQuery">Query/filter options</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("report/itemusage")]
        public Models.OperationReturnModel<IEnumerable<ItemUsageReportItemModel>> ReadItemUsage([FromUri] ItemUsageReportQueryModel usageQuery) {
            Models.OperationReturnModel<IEnumerable<ItemUsageReportItemModel>> retVal = new Models.OperationReturnModel<IEnumerable<ItemUsageReportItemModel>>();
            try
            {
                if (usageQuery != null && usageQuery.fromDate.HasValue && usageQuery.toDate.HasValue)
                {
                    usageQuery.UserSelectedContext = this.SelectedUserContext;
                    var ret = _reportLogic.GetItemUsage(usageQuery);
                    retVal.SuccessResponse = ret;
                    retVal.IsSuccess = true;
                }
                else
                {
                    retVal.IsSuccess = false;
                    retVal.ErrorMessage = "A valid FROM and TO date are required";
                }
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ReadItemUsage", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Export item usage report
        /// </summary>
        /// <param name="usageQuery">Query options</param>
        /// <param name="exportRequest">Export options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("report/itemusage/export")]
        public HttpResponseMessage ExportItemUsage([FromUri] ItemUsageReportQueryModel usageQuery, ExportRequestModel exportRequest) {
            HttpResponseMessage retVal;
            try
            {
                if (usageQuery != null && usageQuery.fromDate.HasValue && usageQuery.toDate.HasValue)
                {
                    usageQuery.UserSelectedContext = this.SelectedUserContext;
                    var ret = _reportLogic.GetItemUsage(usageQuery);
                    ContractInformationHelper.GetContractCategoriesFromLists(SelectedUserContext, ret, _listService);
                    ItemOrderHistoryHelper.GetItemOrderHistories(_catalogLogic, SelectedUserContext, ret);

                    if (exportRequest.Fields != null)
                        _exportLogic.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.ItemUsage, Core.Enumerations.List.ListType.Custom, exportRequest.Fields, exportRequest.SelectedType);

                    retVal = ExportModel<ItemUsageReportItemModel>(ret.ToList(), exportRequest, SelectedUserContext);
                }
                else
                    retVal = new HttpResponseMessage() { StatusCode = HttpStatusCode.NoContent };

            }
            catch(Exception ex)
            {
                _log.WriteErrorLog("ExportItemUsage", ex);
                retVal = new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError };
            }
            return retVal;
        }

        /// <summary>
        /// Retrieve item usage export options
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("report/itemusage/export")]
        public Models.OperationReturnModel<ExportOptionsModel> ExportList() {
            Models.OperationReturnModel<ExportOptionsModel> retVal = new Models.OperationReturnModel<ExportOptionsModel>();
            try
            {
                retVal.SuccessResponse = _exportLogic.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.ItemUsage, 0);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ExportList", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }


        /// <summary>
        /// Retrieve Inventory Valuation report
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("report/inventoryvalue")]
        public HttpResponseMessage GenerateInventoryValuationReport(InventoryValuationRequestModel request) {
            HttpResponseMessage retVal;
            try
            {
                request.context = this.SelectedUserContext;
                var stream = _inventoryValuationReportLogic.GenerateReport(request);

                HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");

                if (request.ReportFormat.Equals("excel", StringComparison.CurrentCultureIgnoreCase))
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                else
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                retVal = result;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("GenerateInventoryValuationReport", ex);
                retVal = new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError };
            }
            return retVal;
        }
        #endregion
    }
}
