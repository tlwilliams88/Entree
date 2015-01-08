using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.InternalSvc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ListServcie" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select ListServcie.svc or ListServcie.svc.cs at the Solution Explorer and start debugging.
	[GlobalErrorBehaviorAttribute(typeof(ErrorHandler))]
	public class ReportService : IReportService
	{
		private readonly IInternalReportLogic reportLogic;

		public ReportService(IInternalReportLogic reportLogic)
		{
			this.reportLogic = reportLogic;
		}

        public List<Core.Models.Reports.ItemUsageReportItemModel> GetItemUsageForCustomer(ItemUsageReportQueryModel usageQuery)
        {
            return reportLogic.GetItemUsage(usageQuery);
        }
    }
}
