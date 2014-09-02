using System;
using System.Text;

namespace KeithLink.Svc.Core.Models.Orders
{
    public class OrderHeader
    {
        #region properties
        public OrderSource OrderingSystem { get; set; }
        public string Branch { get; set; }
        public string CustomerNumber { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string PONumber { get; set; }
        public string Specialinstructions { get; set; }
        public string ControlNumber { get; set; }
        public OrderType OrderType { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime OrderCreateDateTime { get; set; }
        public DateTime OrderSendDateTime { get; set; }
        public bool OrderFilled { get; set; }
        public bool FutureOrder { get; set; }
        #endregion
    }
}
