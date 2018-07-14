﻿using Entree.Core.Models.Orders.History.EF;
using Entree.Core.Models.SiteCatalog;

using System.Collections.Generic;

namespace Entree.Core.Interface.Orders.History {
    public interface IOrderHistoryHeaderRepsitory : IBaseEFREpository<OrderHistoryHeader> {
        IEnumerable<OrderHistoryHeader> GetCustomerOrderHistoryHeaders(string branchId, string customerNumber);
        List<OrderHistoryHeader> GetLastFiveOrdersByItem(string branchId, string customerNumber, string itemNumber);
        IEnumerable<OrderHistoryHeader> ReadByConfirmationNumber(string confirmationNumber, string orderSource);
        IEnumerable<OrderHistoryHeader> ReadForInvoice(string branchId, string invoiceNumber);
        IEnumerable<OrderHistoryHeader> ReadForInvoiceHeader(string branchId, string invoiceNumber);
        string ReadLatestOrderDate(UserSelectedContext catalogInfo);
    }
}
