using KeithLink.Svc.Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Orders.History {
    [DataContract()]
    public class OrderHistoryFile : BaseQueueMessage {
        #region ctor
        public OrderHistoryFile() {
            Header = new OrderHistoryHeader();
            Details = new List<OrderHistoryDetail>();
        }
        #endregion

        #region properties
        [DataMember()]
        public OrderHistoryHeader Header { get; set; }

        [DataMember()]
        public List<OrderHistoryDetail> Details { get; set; }
        #endregion
    }
}
