using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders.History {
    public interface IOrderHistoryHeaderRepsitory : IBaseEFREpository<OrderHistoryHeader> {
        IEnumerable<OrderHistoryHeader> GetCustomerOrderHistoryHeaders(string branchId, string customerNumber);
        IEnumerable<OrderHistoryHeader> GetLastFiveOrdersByItem(string branchId, string customerNumber, string itemNumber);
        IEnumerable<OrderHistoryHeader> ReadByConfirmationNumber(string confirmationNumber);
        IEnumerable<OrderHistoryHeader> ReadForInvoice(string branchId, string invoiceNumber);
		DateTime? ReadLatestOrderDate(UserSelectedContext catalogInfo);
    }
}
