using KeithLink.Common.Core.Logging;

using KeithLink.Svc.Core.Extensions;

using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Interface.Reports;

using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace KeithLink.Svc.Impl.Logic.Reports {
    public class ReportLogic : IReportLogic {
        #region attributes
        public readonly ICatalogRepository _catalogRepository;
        public readonly IReportRepository _reportRepository;
        public readonly IEventLogRepository _log;
        public readonly ICatalogLogic _catalogLogic;
        #endregion

        #region ctor
        public ReportLogic(ICatalogRepository catalogRepository, IReportRepository reportRepository, IEventLogRepository eventLogRepository, 
                           ICatalogLogic catalogLogic) {
            _catalogLogic = catalogLogic;
            _catalogRepository = catalogRepository;
            _log = eventLogRepository;
            _reportRepository = reportRepository;
        }
        #endregion

        #region methods
        protected void FillProductInfo(ItemUsageReportItemModel reportItem, List<Product> products, string customerNumber)
        {
            Product prod = products.Where(x => x.ItemNumber == reportItem.ItemNumber).FirstOrDefault();
            if (prod != null)
            {
                reportItem.IsValid = true;
                reportItem.Name = prod.Name;
                reportItem.ItemClass = prod.ItemClass;
                reportItem.PackSize = prod.PackSize;
                reportItem.Brand = prod.BrandExtendedDescription;
                reportItem.ManufacturerName = prod.ManufacturerName;
                reportItem.UPC = prod.UPC;
                reportItem.VendorItemNumber = prod.VendorItemNumber;

            }
            else
            {
                reportItem.Name = "Special Item";
                _log.WriteInformationLog("Unable to load product details for customer " + customerNumber + " and item " + reportItem.ItemNumber);
            }
        }

        public List<Core.Models.Reports.ItemUsageReportItemModel> GetItemUsage(ItemUsageReportQueryModel usageQuery)
        {
            var orderLines = _reportRepository.GetOrderLinesForItemUsageReport(usageQuery.UserSelectedContext.BranchId,
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
                AveragePrice = Math.Round(g.Average(a => a.Price), 2).ToString("F2"),
                TotalCost = Math.Round(g.Sum(a => a.LineTotal), 2).ToString("F2"),
                Name = g.Select(a => a.Name).FirstOrDefault(),
                Each = g.Key.Each ? "Y" : "N",
                Class = g.Select(a => a.ItemClass).FirstOrDefault(),
                CategoryId = g.Select(a => a.CategoryId).FirstOrDefault(),
                CategoryName = g.Select(a => a.CategoryName).FirstOrDefault(),
                PackSize = g.Select(a => a.PackSize).FirstOrDefault(),
                Brand = g.Select(a => a.Brand).FirstOrDefault(),
                ManufacturerName = g.Select(a => a.ManufacturerName).FirstOrDefault(),
                UPC = g.Select(a => a.UPC).FirstOrDefault(),
                VendorItemNumber = g.Select(a => a.VendorItemNumber).FirstOrDefault()
            }).ToList();
            foreach (var item in reportItems)
            {
                item.Pack = item.PackSize.Substring(0, item.PackSize.IndexOf('/'));
                item.Size = item.PackSize.Substring(item.PackSize.IndexOf('/') + 1);
            }

            var sortInfo = new SortInfo() { Field = "ItemNumber" };
            if (!string.IsNullOrEmpty(usageQuery.sortField))
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

                var tempProducts = _catalogLogic.GetProductsByIds(branchId, batch);

                products.Products.AddRange(tempProducts.Products);
                totalProcessed += 50;
            }

            var productHash = products.Products.GroupBy(p => p.ItemNumber).Select(i => i.First()).ToDictionary(p => p.ItemNumber);

            Parallel.ForEach(orderLines, orderLine =>
            {
                var prod = productHash.ContainsKey(orderLine.ItemNumber) ? productHash[orderLine.ItemNumber] : null;
                if (prod != null)
                {
                    orderLine.IsValid = true;
                    orderLine.Name = prod.Name;
                    orderLine.ItemClass = prod.ItemClass;
                    orderLine.CategoryId = prod.CategoryId;
                    orderLine.CategoryName = prod.CategoryName;
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
        #endregion
    }
}
