using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Core.Extensions;
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
                .GroupBy(x => new { x.ItemNumber, x.UnitOfMeasure });
                //.AsQueryable()
                //.Sort(new List<Core.Models.Paging.SortInfo>() { new Core.Models.Paging.SortInfo() { Field = sortField, Order = sortDir } })
                //.Select(g => new ItemUsageReportItemModel()
                //{
                //    ItemNumber = g.Key,
                //    TotalQuantityOrdered = g.Sum(a => a.OrderQuantity),
                //    TotalQuantityShipped = g.Sum(a => a.ShippedQuantity)
                //}); // TODO - get sort working in db?

            // handle sorting - would be nice to do this generically but don't have an example that used 'group by'
            if (sortDir != null && sortField != null && sortDir.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
            {
                if (sortField.Equals("ItemNumber", StringComparison.InvariantCultureIgnoreCase))
                    query = query.OrderBy(x => x.Key.ItemNumber);
                else if (sortField.Equals("TotalQuantityOrdered", StringComparison.InvariantCultureIgnoreCase))
                    query = query.OrderBy(x => x.Sum(a => a.OrderQuantity));
                else if (sortField.Equals("TotalQuantityShipped", StringComparison.InvariantCultureIgnoreCase))
                    query = query.OrderBy(x => x.Sum(a => a.ShippedQuantity));
                else if (sortField.Equals("Each", StringComparison.InvariantCultureIgnoreCase))
                    query = query.OrderBy(x => x.Key.UnitOfMeasure);
            }
            else if (sortDir != null && sortField != null)
            {
                if (sortField.Equals("ItemNumber", StringComparison.InvariantCultureIgnoreCase))
                    query = query.OrderByDescending(x => x.Key.ItemNumber);
                else if (sortField.Equals("TotalQuantityOrdered", StringComparison.InvariantCultureIgnoreCase))
                    query = query.OrderByDescending(x => x.Sum(a => a.OrderQuantity));
                else if (sortField.Equals("TotalQuantityShipped", StringComparison.InvariantCultureIgnoreCase))
                    query = query.OrderByDescending(x => x.Sum(a => a.ShippedQuantity));
                else if (sortField.Equals("Each", StringComparison.InvariantCultureIgnoreCase))
                    query = query.OrderByDescending(x => x.Key.UnitOfMeasure);
            }

            return query.Select(g => new ItemUsageReportItemModel()
                {
                    ItemNumber = g.Key.ItemNumber,
                    TotalQuantityOrdered = g.Sum(a => a.OrderQuantity),
                    TotalQuantityShipped = g.Sum(a => a.ShippedQuantity),
                    Each = g.Key.UnitOfMeasure.Equals("P", StringComparison.CurrentCultureIgnoreCase) ? "Y" : "N"
                });
        }
    }
}
