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

namespace KeithLink.Svc.WebApi.Controllers
{
    [Authorize]
    public class ReportController : BaseController {
        #region attributes
        IReportServiceRepository reportServiceRepository;
        #endregion

        #region ctor
        public ReportController(IReportServiceRepository reportServiceRepository, IUserProfileLogic profileLogic)
            : base(profileLogic)
        {
            this.reportServiceRepository = reportServiceRepository;
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

        #endregion

    }
}
