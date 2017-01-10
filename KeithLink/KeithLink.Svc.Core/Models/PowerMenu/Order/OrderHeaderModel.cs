using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.PowerMenu.Order
{
    public partial class OrderHeader
    {
        public string CustomerName { get; set; }
        public string CustomerNumber { get; set; }
        public string StoreName { get; set; }
        public string StoreNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
    }
}
