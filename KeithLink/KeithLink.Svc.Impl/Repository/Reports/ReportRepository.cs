using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Reports
{
    public class ReportRepository : IReportRepository
    {
        protected IUnitOfWork UnitOfWork; // unit of work contains our context and entities

        public ReportRepository(IUnitOfWork UnitOfWork)
        {
            this.UnitOfWork = UnitOfWork;
        }

        public IEnumerable<ItemUsageReportItemModel> GetItemUsageForCustomer(
            string branchId, string customerNumber, 
            DateTime fromDateTime, DateTime toDateTime,
            string sortDir, string sortField)
        {
            var query = this.UnitOfWork.Context.OrderHistoryDetails
                .Where(c => c.OrderHistoryHeader.CustomerNumber == customerNumber
                    && c.OrderHistoryHeader.DeliveryDate >= fromDateTime
                    && c.OrderHistoryHeader.DeliveryDate <= toDateTime)
                .GroupBy(x => x.ItemNumber)
                .OrderByDescending(x => x.Sum(a => a.OrderQuantity))
                .Select(g => new ItemUsageReportItemModel()
                {
                    ItemNumber = g.Key,
                    TotalQuantityOrdered = g.Sum(a => a.OrderQuantity),
                    TotalQuantityShipped = g.Sum(a => a.ShippedQuantity)
                }); // TODO - get sort working in db?
            // handle sorting
            if (sortDir != null && sortField != null && sortDir.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
            {
                if (sortField.Equals("ItemNumber", StringComparison.InvariantCultureIgnoreCase))
                    query.OrderBy(x => x.ItemNumber);
                else if (sortField.Equals("TotalQuantityOrdered", StringComparison.InvariantCultureIgnoreCase))
                    query.OrderBy(x => x.TotalQuantityOrdered);
                else if (sortField.Equals("TotalQuantityShipped", StringComparison.InvariantCultureIgnoreCase))
                    query.OrderBy(x => x.TotalQuantityShipped);
            }
            else if (sortDir != null && sortField != null)
            {
                if (sortField.Equals("ItemNumber", StringComparison.InvariantCultureIgnoreCase))
                    query.OrderByDescending(x => x.ItemNumber);
                else if (sortField.Equals("TotalQuantityOrdered", StringComparison.InvariantCultureIgnoreCase))
                    query.OrderByDescending(x => x.TotalQuantityOrdered);
                else if (sortField.Equals("TotalQuantityShipped", StringComparison.InvariantCultureIgnoreCase))
                    query.OrderByDescending(x => x.TotalQuantityShipped);
            }
            return query;
        }
    }
}
