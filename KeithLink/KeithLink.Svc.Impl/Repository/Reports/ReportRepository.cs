using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Extensions.Orders.History;
using KeithLink.Svc.Core;

namespace KeithLink.Svc.Impl.Repository.Reports
{
    public class ReportRepository : IReportRepository
    {
        protected IUnitOfWork UnitOfWork; // unit of work contains our context and entities

        public ReportRepository(IUnitOfWork UnitOfWork)
        {
            this.UnitOfWork = UnitOfWork;
        }
		
		public IEnumerable<Core.Models.Orders.OrderLine> GetOrderLinesForItemUsageReport(string branchId, string customerNumber, DateTime fromDateTime, DateTime toDateTime, string sortDir, string sortField)
		{
            try {
                int numberOfDays = (toDateTime - fromDateTime).Days;
                var dates = Enumerable.Range(0, numberOfDays + 1).Select(d => fromDateTime.AddDays(d).ToLongDateFormat());

                var query = this.UnitOfWork.Context.OrderHistoryDetails
                    .Where(c => c.OrderHistoryHeader.CustomerNumber == customerNumber 
                                 && c.OrderHistoryHeader.BranchId.Equals(branchId, StringComparison.CurrentCultureIgnoreCase)
                                 && dates.Contains(c.OrderHistoryHeader.DeliveryDate)
                                 && c.OrderHistoryHeader.OrderStatus != Constants.CONFIRMATION_HEADER_DELETED_CODE
                                 && c.OrderHistoryHeader.OrderStatus != Constants.CONFIRMATION_HEADER_REJECTED_CODE
                                 && !c.ItemDeleted)
                    .ToList();

			    return query.Select(o => o.ToOrderLine("o")).ToList();
            } catch (Exception ex) {
                throw;
            }
		}
	}
}
