using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Interface.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
    public class InternalReportLogic : IInternalReportLogic
    {
        ICatalogRepository catalogRepository;
        IReportRepository reportRepository;
        IEventLogRepository eventLogRepository;

        public InternalReportLogic(ICatalogRepository catalogRepository, IReportRepository reportRepository, IEventLogRepository eventLogRepository)
        {
            this.catalogRepository = catalogRepository;
            this.reportRepository = reportRepository;
            this.eventLogRepository = eventLogRepository;
        }

        public List<Core.Models.Reports.ItemUsageReportItemModel> GetItemUsage(ItemUsageReportQueryModel usageQuery)
        {
            List<ItemUsageReportItemModel> itemUsageReports = 
                reportRepository.GetItemUsageForCustomer(
                    usageQuery.UserSelectedContext.BranchId, 
                    usageQuery.UserSelectedContext.CustomerId, 
                    usageQuery.fromDate.Value, 
                    usageQuery.toDate.Value.AddDays(1),
                    usageQuery.sortDir,
                    usageQuery.sortField) // add 1 day so it is inclusing of the end date selected
                    .ToList();

            Parallel.ForEach(itemUsageReports, item => 
                {
                    try
                    {
                        item.Name = GetProductName(usageQuery.UserSelectedContext.BranchId, item.ItemNumber);
                    }
                    catch (Exception ex)
                    {
                        item.Name = "unknown";
                        eventLogRepository.WriteErrorLog("Error in GetItemUsage loading product name for customer " + usageQuery.UserSelectedContext.CustomerId + " and item " + item.ItemNumber, ex);
                    }
                });
            
            return itemUsageReports;
        }

        protected string GetProductName(string branchId, string itemNumber)
        {
            return catalogRepository.GetProductById(branchId, itemNumber).Name;
        }
    }
}
