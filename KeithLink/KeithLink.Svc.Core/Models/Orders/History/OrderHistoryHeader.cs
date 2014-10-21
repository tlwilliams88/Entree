using KeithLink.Svc.Core.Enumerations.Order;
using System;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Orders.History {
    [DataContract()]
    public class OrderHistoryHeader {
        #region properties
        [DataMember()]
        public OrderSource OrderSystem { get; set; }

        [DataMember()]
        public string BranchId { get; set; }

        [DataMember()]
        public string CustomerNumber { get; set; }

        [DataMember()]
        public string InvoiceNumber { get; set; }

        [DataMember()]
        public DateTime DeliveryDate { get; set; }

        [DataMember()]
        public string PONumber { get; set; }

        [DataMember()]
        public string ControlNumber { get; set; }

        [DataMember()]
        public string OrderStatus { get; set; }

        [DataMember()]
        public bool FutureItems { get; set; }

        [DataMember()]
        public bool ErrorStatus { get; set; }

        [DataMember()]
        public string RouteNumber { get; set; }

        [DataMember()]
        public string StopNumber { get; set; }
        #endregion
    }
}
