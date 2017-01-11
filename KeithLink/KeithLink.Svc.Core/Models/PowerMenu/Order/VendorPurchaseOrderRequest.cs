using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.PowerMenu.Order
{
    [Serializable, XmlRoot("VendorOrderRequest")]
    public class VendorPurchaseOrderRequest
    {
        public Login Login { get; set; }
        public Order Order { get; set; }
    }
}
