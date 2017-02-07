using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.PowerMenu.Order
{
    public class Order
    {
        [XmlElement("OrderHeader")]
        public OrderHeader OrderHeader { get; set; }

        [XmlElement("OrderItem")]
        public List<OrderItem> OrderItem { get; set; }
    }
}
