using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Reports
{
	public interface IReportRepository
	{
        IEnumerable<ItemUsageReportItemModel> GetItemUsageForCustomer(
            string branchId, string customerNumber, 
            DateTime fromDateTime, DateTime toDateTime,
            string sortDir, string sortField);

		IEnumerable<OrderLine> GetOrderLinesForItemUsageReport(
			string branchId, string customerNumber,
			DateTime fromDateTime, DateTime toDateTime,
			string sortDir, string sortField);
    }
}
