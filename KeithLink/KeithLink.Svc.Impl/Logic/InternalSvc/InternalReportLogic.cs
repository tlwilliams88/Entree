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
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Paging;

namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
    public class InternalReportLogic : IInternalReportLogic
    {
        ICatalogRepository catalogRepository;
        IReportRepository reportRepository;
        IEventLogRepository eventLogRepository;
		IInternalOrderHistoryLogic orderHistoryLogic;
		ICatalogLogic catalogLogic;

		public InternalReportLogic(ICatalogRepository catalogRepository, IReportRepository reportRepository, IEventLogRepository eventLogRepository, IInternalOrderHistoryLogic orderHistoryLogic, ICatalogLogic catalogLogic)
        {
            this.catalogRepository = catalogRepository;
            this.reportRepository = reportRepository;
            this.eventLogRepository = eventLogRepository;
			this.orderHistoryLogic = orderHistoryLogic;
			this.catalogLogic = catalogLogic;
        }

        public List<Core.Models.Reports.ItemUsageReportItemModel> GetItemUsage(ItemUsageReportQueryModel usageQuery)
        {
			var orderLines = reportRepository.GetOrderLinesForItemUsageReport(usageQuery.UserSelectedContext.BranchId,
					usageQuery.UserSelectedContext.CustomerId,
					usageQuery.fromDate.Value,
					usageQuery.toDate.Value.AddDays(1),
					usageQuery.sortDir,
					usageQuery.sortField).ToList();

			LookupProductInfo(orderLines, usageQuery.UserSelectedContext.BranchId);


			var reportItems = orderLines.GroupBy(x => new { x.ItemNumber, x.Each }).Select(g => new ItemUsageReportItemModel()
                {
                    ItemNumber = g.Key.ItemNumber,
                    TotalQuantityOrdered = g.Sum(a => a.QuantityOrdered),
                    TotalQuantityShipped = g.Sum(a => a.QantityShipped),
					AveragePrice = g.Average(a => a.Price),
					TotalCost = g.Sum(a => a.LineTotal),
					Name = g.Select(a => a.Name).FirstOrDefault(),
					Each = g.Key.Each ? "Y" : "N",
					PackSize = g.Select(a => a.PackSize).FirstOrDefault(),
					Brand = g.Select(a => a.Brand).FirstOrDefault(),
					ManufacturerName = g.Select(a => a.ManufacturerName).FirstOrDefault(),
					UPC = g.Select(a => a.UPC).FirstOrDefault(),
					VendorItemNumber = g.Select(a => a.VendorItemNumber).FirstOrDefault()
				}).ToList();

			var sortInfo = new SortInfo() { Field = "ItemNumber"};
			if(!string.IsNullOrEmpty(usageQuery.sortField))
			{
				sortInfo.Field = usageQuery.sortField;
				sortInfo.Order = usageQuery.sortDir;
			}

			return reportItems.AsQueryable().Sort(new List<SortInfo>() { sortInfo }).ToList();

			//List<ItemUsageReportItemModel> itemUsageReports = 
			//	reportRepository.GetItemUsageForCustomer(
			//		usageQuery.UserSelectedContext.BranchId, 
			//		usageQuery.UserSelectedContext.CustomerId, 
			//		usageQuery.fromDate.Value, 
			//		usageQuery.toDate.Value.AddDays(1),
			//		usageQuery.sortDir,
			//		usageQuery.sortField) // add 1 day so it is inclusing of the end date selected
			//		.ToList();

			//ProductsReturn ret = catalogRepository.GetProductsByIds(
			//	usageQuery.UserSelectedContext.BranchId,
			//	itemUsageReports.Select(i => i.ItemNumber).ToList());

			//Parallel.ForEach(itemUsageReports, item => 
			//	{
			//		FillProductInfo(item, ret.Products, usageQuery.UserSelectedContext.CustomerId);
			//	});
            
			//// handle name sort in code...
			//if (usageQuery.sortField != null && usageQuery.sortField.Equals("name", StringComparison.InvariantCultureIgnoreCase))
			//{
			//	if (usageQuery.sortDir.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
			//		itemUsageReports.Sort((i1, i2) => i1.Name.CompareTo(i2.Name));
			//	else
			//		itemUsageReports.Sort((i1, i2) => i2.Name.CompareTo(i1.Name));
			//}
			//return itemUsageReports;
        }

		private void LookupProductInfo(List<OrderLine> orderLines, string branchId)
		{
			ProductsReturn products = new ProductsReturn() { Products = new List<Product>() };
			int totalProcessed = 0;

			List<string> distinctItemNumbers = orderLines.Distinct().Select(o => o.ItemNumber).ToList();

			while (totalProcessed < distinctItemNumbers.Count)
			{
				var batch = distinctItemNumbers.Skip(totalProcessed).Take(50).ToList();

				var tempProducts = catalogLogic.GetProductsByIds(branchId, batch);

				products.Products.AddRange(tempProducts.Products);
				totalProcessed += 50;
			}

			var productHash = products.Products.GroupBy(p => p.ItemNumber).Select(i => i.First()).ToDictionary(p => p.ItemNumber);

			Parallel.ForEach(orderLines, orderLine =>
			{
				var prod = productHash.ContainsKey(orderLine.ItemNumber) ? productHash[orderLine.ItemNumber] : null;
				if (prod != null)
				{
					orderLine.Name = prod.Name;
					orderLine.Pack = prod.Pack;
					orderLine.Size = prod.Size;
					orderLine.CatchWeight = prod.CatchWeight;
					orderLine.AverageWeight = prod.AverageWeight;
					orderLine.Brand = prod.BrandExtendedDescription;
					orderLine.ManufacturerName = prod.ManufacturerName;
					orderLine.UPC = prod.UPC;
					orderLine.VendorItemNumber = prod.VendorItemNumber;
				}

			});

		}


        protected void FillProductInfo(ItemUsageReportItemModel reportItem, List<Product> products, string customerNumber)
        {
            Product prod = products.Where(x => x.ItemNumber == reportItem.ItemNumber).FirstOrDefault();
            if (prod != null)
            {
                reportItem.Name = prod.Name;
                reportItem.PackSize = prod.PackSize;
				reportItem.Brand = prod.BrandExtendedDescription;
				reportItem.ManufacturerName = prod.ManufacturerName;
				reportItem.UPC = prod.UPC;
				reportItem.VendorItemNumber = prod.VendorItemNumber;
				
            }
            else
            {
                reportItem.Name = "Special Item";
                eventLogRepository.WriteInformationLog("Unable to load product details for customer " + customerNumber + " and item " + reportItem.ItemNumber);
            }
        }
    }
}
