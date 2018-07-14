using Entree.Core.Models.Orders;
using Entree.Core.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Reports
{
	public interface IReportRepository
	{
        IEnumerable<OrderLine> GetOrderLinesForItemUsageReport( string branchId, string customerNumber, DateTime fromDateTime, 
                                                                                                    DateTime toDateTime, string sortDir, string sortField);
    }
}
