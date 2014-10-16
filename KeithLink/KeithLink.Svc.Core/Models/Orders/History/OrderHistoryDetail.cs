using KeithLink.Svc.Core.Enumerations.Order;
using System;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Orders.History {
    [DataContract()]
    public class OrderHistoryDetail {
        #region properties
        [DataMember()]
        public int LineNumber { get; set; }

        [DataMember()]
        public string ItemNumber { get; set; }

        [DataMember()]
        public int OrderQuantity { get; set; }

        [DataMember()]
        public int ShippedQuantity { get; set; }

        [DataMember()]
        public UnitOfMeasure UnitOfMeasure { get; set; }

        [DataMember()]
        public bool CatchWeight { get; set; }

        [DataMember()]
        public bool ItemDeleted { get; set; }

        [DataMember()]
        public string SubbedOriginalItemNumber { get; set; }

        [DataMember()]
        public string ReplacedOriginalItemNumber { get; set; }

        [DataMember()]
        public string ItemStatus { get; set; }
        #endregion
    }
}
