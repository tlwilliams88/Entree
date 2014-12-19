using KeithLink.Svc.Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Orders
{
    [DataContract(Name="OrderFile")]
    public class OrderFile : BaseQueueMessage
    {
        #region ctor
        public OrderFile() {
            Header = new OrderHeader();
            Details = new List<OrderDetail>();
        }
        #endregion

        #region  properties
        [DataMember(Name="Header")]
        public OrderHeader Header { get; set; }

        [DataMember(Name="Details")]
        public List<OrderDetail> Details { get; set; }
        #endregion
    }
}
