using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Models.Orders
{
    public class OrderFile
    {
        #region ctor
        public OrderFile()
        {
            Header = new OrderHeader();
            Details = new List<OrderDetail>();
        }
        #endregion

        #region  properties
        public OrderHeader Header { get; set; }

        public List<OrderDetail> Details { get; set; }
        #endregion
    }
}
