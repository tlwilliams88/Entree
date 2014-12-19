using KeithLink.Svc.Core.Models.Common;
using System;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Orders.History {
    [DataContract(Name="OrderHistoryRequest")]
    public class OrderHistoryRequest : BaseQueueMessage {
        #region properties
        [DataMember(Name="BranchId")]
        public string BranchId { get; set; }

        [DataMember(Name = "CustomerNumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name = "InvoiceNumber")]
        public string InvoiceNumber { get; set; }
        #endregion
    }
}
