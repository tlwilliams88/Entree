using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Interface.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Interface.Configuration;

namespace KeithLink.Svc.WebApi.Controllers
{
    [Authorize]
    public class ReportController : BaseController {
        #region attributes
        IReportServiceRepository reportServiceRepository;
		private readonly IExportSettingServiceRepository exportSettingRepository;
       
        #endregion

        #region ctor
		public ReportController(IReportServiceRepository reportServiceRepository, IUserProfileLogic profileLogic, IExportSettingServiceRepository exportSettingRepository)
            : base(profileLogic)
        {
            this.reportServiceRepository = reportServiceRepository;
			this.exportSettingRepository = exportSettingRepository;
        }
        #endregion

        #region methods
        [HttpGet]
		[ApiKeyedRoute("report/itemusage")]
		public Models.OperationReturnModel<IEnumerable<ItemUsageReportItemModel>> ReadItemUsage([FromUri] ItemUsageReportQueryModel usageQuery) {
            if (usageQuery != null && usageQuery.fromDate.HasValue && usageQuery.toDate.HasValue)
            {
                usageQuery.UserSelectedContext = this.SelectedUserContext;
                var ret = reportServiceRepository.GetItemUsage(usageQuery);
                return new Models.OperationReturnModel<IEnumerable<Core.Models.Reports.ItemUsageReportItemModel>>() 
                {
                    SuccessResponse = ret
                };
            }
            else
                return new Models.OperationReturnModel<IEnumerable<ItemUsageReportItemModel>>() { ErrorMessage = "A valid FROM and TO date are required" };
		}

		[HttpPost]
		[ApiKeyedRoute("report/itemusage/export")]
		public HttpResponseMessage ExportList([FromUri] ItemUsageReportQueryModel usageQuery, ExportRequestModel exportRequest)
		{
			if (usageQuery != null && usageQuery.fromDate.HasValue && usageQuery.toDate.HasValue)
			{
				usageQuery.UserSelectedContext = this.SelectedUserContext;
				var ret = reportServiceRepository.GetItemUsage(usageQuery);

				if (exportRequest.Fields != null)
					exportSettingRepository.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.ItemUsage, Core.Enumerations.List.ListType.Custom, exportRequest.Fields, exportRequest.SelectedType);

				return ExportModel<ItemUsageReportItemModel>(ret.ToList(), exportRequest);
			}
			else
				return new HttpResponseMessage() { StatusCode = HttpStatusCode.NoContent };			
		}

		[HttpGet]
		[ApiKeyedRoute("report/itemusage/export")]
		public ExportOptionsModel ExportList()
		{
			return exportSettingRepository.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.ItemUsage, 0);
		}


        #endregion

    }
}
