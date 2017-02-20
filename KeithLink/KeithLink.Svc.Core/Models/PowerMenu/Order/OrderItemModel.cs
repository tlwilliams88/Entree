using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.PowerMenu.Order
{
    public class OrderItem
    {
        public int CustomerLineSequenceNumber { get; set; }
        public string VendorProductNumber { get; set; }
        public int CaseQuantityOrdered { get; set; }
        public int EachQuantityOrdered { get; set; }
    }

}
