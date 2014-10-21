using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Models.Orders.History {
    public class OrderHistoryHeaderReturn {
        #region ctor
        public OrderHistoryHeaderReturn() {
            Headers = new List<OrderHistoryHeader>();
        }
        #endregion

        #region properties
        public List<OrderHistoryHeader> Headers { get; set; }
        #endregion
    }
}
