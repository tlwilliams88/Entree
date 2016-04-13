using KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Orders.History {
    public interface IOrderHistoryHeaderRepsitory : IBaseEFREpository<OrderHistoryHeader> {
        IEnumerable<OrderHistoryHeader> GetCustomerOrderHistoryHeaders(string branchId, string customerNumber);
        List<OrderHistoryHeader> GetLastFiveOrdersByItem(string branchId, string customerNumber, string itemNumber);
        IEnumerable<OrderHistoryHeader> ReadByConfirmationNumber(string confirmationNumber, string orderSource);
        IEnumerable<OrderHistoryHeader> ReadForInvoice(string branchId, string invoiceNumber);
		string ReadLatestOrderDate(UserSelectedContext catalogInfo);
    }
}
