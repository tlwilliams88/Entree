using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.WebApi.Repository.Reports
{
	public class ReportServiceRepositoryImpl : IReportServiceRepository
	{		
		private com.benekeith.ReportService.IReportService serviceClient;

		public ReportServiceRepositoryImpl(com.benekeith.ReportService.IReportService serviceClient)
		{
			this.serviceClient = serviceClient;
		}

        public IEnumerable<ItemUsageReportItemModel> GetItemUsage(ItemUsageReportQueryModel usageQuery)
        {
            return serviceClient.GetItemUsageForCustomer(usageQuery);
        }
    }
}
