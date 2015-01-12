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

            ProductsReturn ret = catalogRepository.GetProductsByIds(
                usageQuery.UserSelectedContext.BranchId,
                itemUsageReports.Select(i => i.ItemNumber).ToList());

            Parallel.ForEach(itemUsageReports, item => 
                {
                    FillProductInfo(item, ret.Products, usageQuery.UserSelectedContext.CustomerId);
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

        protected void FillProductInfo(ItemUsageReportItemModel reportItem, List<Product> products, string customerNumber)
        {
            Product prod = products.Where(x => x.ItemNumber == reportItem.ItemNumber).FirstOrDefault();
            if (prod != null)
            {
                reportItem.Name = prod.Name;
                reportItem.PackSize = prod.PackSize;
            }
            else
            {
                reportItem.Name = "Special Item";
                eventLogRepository.WriteInformationLog("Unable to load product details for customer " + customerNumber + " and item " + reportItem.ItemNumber);
            }
        }
    }
}
