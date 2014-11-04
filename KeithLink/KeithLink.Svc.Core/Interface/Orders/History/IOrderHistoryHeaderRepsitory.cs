﻿using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Orders.History.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders.History {
    public interface IOrderHistoryHeaderRepsitory : IBaseEFREpository<OrderHistoryHeader> {
        IEnumerable<OrderHistoryHeader> ReadForInvoice(string branchId, string invoiceNumber);
        IEnumerable<OrderHistoryHeader> GetLastFiveOrdersByItem(string branchId, string customerNumber, string itemNumber);
    }
}
