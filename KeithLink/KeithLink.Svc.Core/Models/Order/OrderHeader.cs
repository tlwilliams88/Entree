using System;
using System.Text;

namespace KeithLink.Svc.Core.Models.Order
{
    public class OrderHeader
    {
        #region properties
        public OrderSystems OrderingSystem { get; set; }
        public string Branch { get; set; }
        public string CustomerNumber { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string PONumber { get; set; }
        public string Specialinstructions { get; set; }
        public string ControlNumber { get; set; }
        public string OrderType { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime OrderCreateDateTime { get; set; }
        public DateTime OrderSendDateTime { get; set; }
        public string OrderStatus { get; set; }
        public string FutureItems { get; set; }
        #endregion
    }
}
