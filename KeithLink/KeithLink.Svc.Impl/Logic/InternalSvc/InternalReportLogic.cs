using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Extensions;
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
                        item.Name = "Special Item";
                        eventLogRepository.WriteErrorLog("Error in GetItemUsage loading product name for customer " + usageQuery.UserSelectedContext.CustomerId + " and item " + item.ItemNumber, ex);
                    }
                });
            
            // handle name sort in code...
            if (usageQuery.sortField != null && usageQuery.sortField.Equals("name", StringComparison.InvariantCultureIgnoreCase))
            {
                if (usageQuery.sortDir.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
                    itemUsageReports.Sort((i1, i2) => i1.Name.CompareTo(i2.Name));
                else
                    itemUsageReports.Sort((i1, i2) => i2.Name.CompareTo(i1.Name));
            }
            return itemUsageReports;
        }

        protected string GetProductName(string branchId, string itemNumber)
        {
            return catalogRepository.GetProductById(branchId, itemNumber).Name;
        }
    }
}
