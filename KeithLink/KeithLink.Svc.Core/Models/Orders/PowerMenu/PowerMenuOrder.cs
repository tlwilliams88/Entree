using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.Orders.PowerMenu
{
    [Serializable, XmlRoot("VendorOrderRequest")]
    public class VendorPurchaseOrderRequest
    {
        public Login Login { get; set; }
        public Order Order { get; set; }
    }

    public partial class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public partial class Order
    {
        public OrderHeader OrderHeader { get; set; }
        public OrderItem OrderItem { get; set; }
    }

    public partial class OrderHeader
    {
        public string CustomerName { get; set; }
        public string CustomerNumber { get; set; }
        public string StoreName { get; set; }
        public string StoreNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
    }

    public partial class OrderItem
    {
        public string CustomerLineSequenceNumber { get; set; }
        public string VendorProductNumber { get; set; }
        public string CaseQuantityOrdered { get; set; }
        public string EachQuantityOrdered { get; set; }
    }

}
