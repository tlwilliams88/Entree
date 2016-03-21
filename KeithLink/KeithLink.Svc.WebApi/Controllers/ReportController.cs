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

namespace KeithLink.Svc.WebApi.Controllers
{
    /// <summary>
    /// controller for reports
    /// </summary>
    [Authorize]
    public class ReportController : BaseController {
        #region attributes
        private readonly IReportLogic _reportLogic;
		private readonly IExportSettingLogic _exportLogic;
		private readonly IInventoryValuationReportLogic _inventoryValuationReportLogic;       
        #endregion

        #region ctor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="reportLogic"></param>
        /// <param name="profileLogic"></param>
        /// <param name="exportSettingsLogic"></param>
        /// <param name="inventoryValuationReportLogic"></param>
		public ReportController(IReportLogic reportLogic, IUserProfileLogic profileLogic, IExportSettingLogic exportSettingsLogic, 
                                IInventoryValuationReportLogic inventoryValuationReportLogic) 
            : base(profileLogic) {
			_exportLogic = exportSettingsLogic;
			_inventoryValuationReportLogic = inventoryValuationReportLogic;
            _reportLogic = reportLogic;
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
            if (usageQuery != null && usageQuery.fromDate.HasValue && usageQuery.toDate.HasValue) {
                usageQuery.UserSelectedContext = this.SelectedUserContext;
                var ret = _reportLogic.GetItemUsage(usageQuery);
                return new Models.OperationReturnModel<IEnumerable<Core.Models.Reports.ItemUsageReportItemModel>>() {
                    SuccessResponse = ret
                };
            } else
                return new Models.OperationReturnModel<IEnumerable<ItemUsageReportItemModel>>() { ErrorMessage = "A valid FROM and TO date are required" };
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
            if (usageQuery != null && usageQuery.fromDate.HasValue && usageQuery.toDate.HasValue) {
                usageQuery.UserSelectedContext = this.SelectedUserContext;
                var ret = _reportLogic.GetItemUsage(usageQuery);

                if (exportRequest.Fields != null)
                    _exportLogic.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.ItemUsage, Core.Enumerations.List.ListType.Custom, exportRequest.Fields, exportRequest.SelectedType);

                return ExportModel<ItemUsageReportItemModel>(ret.ToList(), exportRequest);
            } else
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.NoContent };
        }

        /// <summary>
        /// Retrieve item usage export options
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("report/itemusage/export")]
        public ExportOptionsModel ExportList() {
            return _exportLogic.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.ItemUsage, 0);
        }


        /// <summary>
        /// Retrieve Inventory Valuation report
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("report/inventoryvalue")]
        public HttpResponseMessage GenerateInventoryValuationReport(InventoryValuationRequestModel request) {
            var stream = _inventoryValuationReportLogic.GenerateReport(request);

            HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");

            if (request.ReportFormat.Equals("excel", StringComparison.CurrentCultureIgnoreCase))
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            else
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            return result;
        }
        #endregion
    }
}
