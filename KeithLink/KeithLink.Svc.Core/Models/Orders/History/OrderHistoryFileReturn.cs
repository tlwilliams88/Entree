using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Models.Orders.History {
    public class OrderHistoryFileReturn {
        #region ctor
        public OrderHistoryFileReturn() {
            Files = new List<OrderHistoryFile>();
        }
        #endregion

        #region properties
        public List<OrderHistoryFile> Files { get; set; }
        #endregion
    }
}
